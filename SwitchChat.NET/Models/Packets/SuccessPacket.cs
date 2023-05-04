using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchChatNet.Models.Packets;

public class SuccessPacket : Data
{
    public string Reason { get; set; }
}
