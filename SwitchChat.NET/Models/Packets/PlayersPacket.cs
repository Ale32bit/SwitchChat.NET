using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchChatNet.Models.Packets;

public class PlayersPacket : Data
{
    public DateTime Time { get; set; }
    public IEnumerable<User> Players { get; set; }
}
