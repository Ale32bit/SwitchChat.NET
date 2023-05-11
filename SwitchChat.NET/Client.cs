using SwitchChatNet.Enums;
using SwitchChatNet.Models;
using SwitchChatNet.Models.Events;
using SwitchChatNet.Models.Packets;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Timers;

namespace SwitchChatNet;

public class Client
{
    public static readonly Uri ServerEndpoint = new("wss://chat.sc3.io/v2/");

    public event EventHandler<AFK> OnAfk;
    public event EventHandler<AFKReturn> OnAfkReturn;
    public event EventHandler<ChatboxChatMessage> OnChatboxChatMessage;
    public event EventHandler<ChatboxCommand> OnChatboxCommand;
    public event EventHandler<Death> OnDeath;
    public event EventHandler<DiscordChatMessage> OnDiscordChatMessage;
    public event EventHandler<IngameChatMessage> OnIngameChatMessage;
    public event EventHandler<Join> OnJoin;
    public event EventHandler<Leave> OnLeave;
    public event EventHandler<WorldChange> OnWorldChange;
    public event EventHandler<ServerRestartScheduled> OnServerRestartScheduled;
    public event EventHandler<ServerRestartCancelled> OnServerRestartCancelled;
    public event EventHandler<DataEvent> OnRaw;
    public event EventHandler OnReady;
    public event EventHandler OnPlayers;
    public event EventHandler<ClosingEvent> OnClosing;
    public event EventHandler<ErrorEvent> OnError;

    public bool Running { get; private set; } = false;
    public IEnumerable<Capability> Capabilities => _capabilities.ToArray();
    public IEnumerable<User> Players => _players.ToArray();

    public FormattingMode DefaultFormattingMode = FormattingMode.Format;
    public string? DefaultName = null;
    public string Owner { get; private set; }


    private ClientWebSocket? _wsClient;

    private readonly string? _token;
    private readonly Uri _endpoint;

    private IEnumerable<Capability> _capabilities = Array.Empty<Capability>();
    private List<User> _players = new();

    private Queue<QueueMessage> _messages = new();
    private int _queueCounter = 0;
    private readonly TimeSpan _queueDelay = TimeSpan.FromMilliseconds(500);
    private bool _processingQueue = false;

    private readonly Dictionary<int, TaskCompletionSource<bool>> _messageTcs = new();
    private readonly TimeSpan _restartDelay = TimeSpan.FromSeconds(60);

    public Client(string? token = null, Uri? endpoint = default)
    {
        _token = token;
        _endpoint = endpoint ?? ServerEndpoint;

        OnAfk += (_, e) => UpdatePlayer(e.User);
        OnAfkReturn += (_, e) => UpdatePlayer(e.User);
        OnRaw += ProcessData;
    }

    /// <summary>
    /// Say something in the in-game chat
    /// </summary>
    /// <param name="text"></param>
    /// <param name="name"></param>
    /// <param name="mode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Whether is succeeded</returns>
    public Task<bool> SayAsync(string text, string? name = null, FormattingMode? mode = null, CancellationToken cancellationToken = default)
    {
        name ??= DefaultName;
        mode ??= DefaultFormattingMode;

        var message = new QueueMessage
        {
            Request = new ChatRequestPacket
            {
                Type = ChatRequestType.Say,
                Id = _queueCounter++,
                Text = text,
                Name = name,
                Mode = mode,
            },
            Tcs = new(),
        };

        _messages.Enqueue(message);
        ProcessQueue();

        return message.Tcs.Task;
    }

    /// <summary>
    /// Tell something to a player in the in-game chat.
    /// </summary>
    /// <param name="user">User's name or UUID</param>
    /// <param name="text"></param>
    /// <param name="name"></param>
    /// <param name="mode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Whether it succeeded. Will return false if the player is not found.</returns>
    public Task<bool> TellAsync(string user, string text, string? name = null, FormattingMode? mode = null, CancellationToken cancellationToken = default)
    {
        name ??= DefaultName;
        mode ??= DefaultFormattingMode;

        var task = new Task(() => { }, cancellationToken);

        var message = new QueueMessage
        {
            Request = new ChatRequestPacket
            {
                Type = ChatRequestType.Tell,
                Id = _queueCounter++,
                User = user,
                Text = text,
                Name = name,
                Mode = mode,
            },
            Tcs = new(),
        };

        _messages.Enqueue(message);
        ProcessQueue();

        return message.Tcs.Task;
    }

    /// <summary>
    /// Connect to the Chatbox WebSocket server
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        if (_wsClient != null)
        {
            if(_wsClient.State != WebSocketState.Closed)
                await _wsClient.CloseAsync(WebSocketCloseStatus.NormalClosure, null, cancellationToken);
            _wsClient.Dispose();
        }

        _wsClient = new();

        var uri = new Uri(_endpoint, _token);
        await _wsClient.ConnectAsync(uri, cancellationToken);
        if (_wsClient.State != WebSocketState.Open)
        {
            throw new Exception(_wsClient.CloseStatusDescription);
        }
    }

    /// <summary>
    /// Start the listening loop
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        if (_wsClient == null || _wsClient.State != WebSocketState.Open)
            await ConnectAsync(cancellationToken);

        while (true)
        {
            await ListenAsync(cancellationToken);
            if (Running)
                await ReconnectAsync(true, cancellationToken);
            else
                break;
        }
    }

    /// <summary>
    /// Close connection to the server
    /// </summary>
    /// <param name="soft"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task CloseAsync(bool soft = false, CancellationToken cancellationToken = default)
    {
        if (!soft)
            Running = false;

        _processingQueue = false;
        await _wsClient.CloseAsync(WebSocketCloseStatus.NormalClosure, null, cancellationToken);
        _wsClient.Dispose();
    }

    /// <summary>
    /// Restart connections
    /// </summary>
    /// <param name="wait">Wait before starting</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task ReconnectAsync(bool wait = false, CancellationToken cancellationToken = default)
    {
        await CloseAsync(wait, cancellationToken);

        if (wait)
            await Task.Delay(_restartDelay, cancellationToken);

        await ConnectAsync(cancellationToken);
    }

    private void UpdatePlayer(User player)
    {
        var index = _players.FindIndex(q => q.UUID == player.UUID);
        if (index >= 0)
            _players[index] = player;
        else
            _players.Add(player);
    }

    private async Task ListenAsync(CancellationToken cancellationToken = default)
    {
        var buffer = new byte[4096];
        var builder = new StringBuilder();

        while (_wsClient.State == WebSocketState.Open)
        {
            var result = await _wsClient.ReceiveAsync(buffer, cancellationToken);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await CloseAsync(false, cancellationToken);
                return;
            }

            var data = Encoding.UTF8.GetString(buffer, 0, result.Count);
            builder.Append(data);

            if (result.EndOfMessage)
            {
                var rawPayload = builder.ToString();

                var payload = Parse<Data>(rawPayload);
                OnRaw?.Invoke(this, new DataEvent { Data = payload, Payload = rawPayload });
                builder.Clear();
            }
        }
    }

    private void ProcessData(object? sender, DataEvent e)
    {
        switch (e.Data.Type)
        {
            case "hello":
                var hello = Parse<HelloPacket>(e.Payload);
                Owner = hello.LicenseOwner;
                _capabilities = hello.Capabilities.Select(c => Enum.Parse<Capability>(c, true)).ToArray();

                Running = true;
                OnReady?.Invoke(this, EventArgs.Empty);
                break;

            case "players":
                var playersData = Parse<PlayersPacket>(e.Payload);

                _players = playersData.Players.ToList();
                break;

            case "event":
                var eventData = Parse<BaseEvent>(e.Payload);

                ProcessEvent(eventData.Event, e.Payload);
                break;

            case "success":
                var success = Parse<SuccessPacket>(e.Payload);
                if (!success.Id.HasValue || !_messageTcs.TryGetValue(success.Id.Value, out var stcs))
                    return;

                if (success.Reason == "message_sent")
                {
                    stcs.TrySetResult(true);
                    _messageTcs.Remove(success.Id.Value);
                }

                break;

            case "error":
                var error = Parse<ErrorPacket>(e.Payload);
                if (!error.Id.HasValue || !_messageTcs.TryGetValue(error.Id.Value, out var etcs))
                {
                    return;
                }

                etcs.TrySetResult(false);
                _messageTcs.Remove(error.Id.Value);

                OnError?.Invoke(this, new ErrorEvent()
                {
                    Error = error.Error,
                    Message = error.Message,
                });

                break;

            case "closing":
                var closing = Parse<ClosingPacket>(e.Payload);

                OnClosing?.Invoke(this, new()
                {
                    CloseReason = closing.CloseReason,
                    Reason = closing.Reason,
                });

                if (closing.CloseReason != "server_stopping")
                {
                    Running = false;
                }

                break;

            default:
                break;
        }
    }

    private void ProcessEvent(string eventName, string payload)
    {
        switch (eventName)
        {
            case "chat_ingame":
                var gcev = Parse<IngameChatMessage>(payload);
                OnIngameChatMessage?.Invoke(this, gcev);
                break;

            case "chat_discord":
                var dcev = Parse<DiscordChatMessage>(payload);
                OnDiscordChatMessage?.Invoke(this, dcev);
                break;

            case "chat_chatbox":
                var ccev = Parse<ChatboxChatMessage>(payload);
                OnChatboxChatMessage?.Invoke(this, ccev);
                break;

            case "command":
                var cmdev = Parse<ChatboxCommand>(payload);
                OnChatboxCommand?.Invoke(this, cmdev);
                break;

            case "join":
                var jev = Parse<Join>(payload);
                OnJoin?.Invoke(this, jev);
                break;

            case "leave":
                var lev = Parse<Leave>(payload);
                OnLeave?.Invoke(this, lev);
                break;

            case "death":
                var dev = Parse<Death>(payload);
                OnDeath?.Invoke(this, dev);
                break;

            case "afk":
                var aev = Parse<AFK>(payload);
                OnAfk?.Invoke(this, aev);
                break;

            case "afk_return":
                var arev = Parse<AFKReturn>(payload);
                OnAfkReturn?.Invoke(this, arev);
                break;

            case "world_change":
                var wcev = Parse<WorldChange>(payload);
                OnWorldChange?.Invoke(this, wcev);
                break;

            case "server_restart_scheduled":
                var srsev = Parse<ServerRestartScheduled>(payload);
                OnServerRestartScheduled?.Invoke(this, srsev);
                break;

            case "server_restart_cancelled":
                var srcev = Parse<ServerRestartCancelled>(payload);
                OnServerRestartCancelled?.Invoke(this, srcev);
                break;
        }
    }

    private async Task ProcessQueue()
    {
        if (_processingQueue)
            return;

        _processingQueue = true;
        while(_processingQueue && _messages.TryDequeue(out var message))
        {
            var request = JsonSerializer.Serialize(message.Request, JsonSerializerOptions);
            var payload = Encoding.UTF8.GetBytes(request);
            _messageTcs[message.Request.Id] = message.Tcs;
            await _wsClient.SendAsync(payload, WebSocketMessageType.Text, true, CancellationToken.None);
            await Task.Delay(_queueDelay);
        }
        _processingQueue = false;
    }

    private static JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
        },
    };
    private static T? Parse<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
    }
}