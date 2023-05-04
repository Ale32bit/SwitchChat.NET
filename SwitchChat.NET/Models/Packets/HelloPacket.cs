using SwitchChatNet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchChatNet.Models.Packets;

public class HelloPacket : Data
{
    public bool Guest { get; set; }
    public string LicenseOwner { get; set; }
    public string[] Capabilities { get; set; }

}
