using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchChatNet.Models.Packets;

public class ClosingPacket : Data
{
    public string CloseReason { get; set; }
    public string Reason { get; set; }
}
