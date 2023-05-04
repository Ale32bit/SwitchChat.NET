using SwitchChatNet.Enums;
using System.Text.Json.Serialization;

namespace SwitchChatNet.Models.Packets;

public class ChatRequestPacket
{
    public int Id { get; set; } = -1;
    public ChatRequestType Type { get; set; }
    public string? User { get; set; }
    public string Text { get; set; }
    public string? Name { get; set; }
    public FormattingMode? Mode { get; set; }
}

