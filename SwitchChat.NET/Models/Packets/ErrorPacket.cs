using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchChatNet.Models.Packets;

public class ErrorPacket : Data
{
    public string Error { get; set; }
    public string Message { get; set; }
}
