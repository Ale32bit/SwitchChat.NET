using SwitchChatNet.Models.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchChatNet;

internal class QueueMessage
{
    public ChatRequestPacket Request { get; set; }
    public TaskCompletionSource<bool> Tcs { get; set; }
}
