using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchChatNet.Models;

public class DiscordRole
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Colour { get; set; }
    public int Color => Colour;
}
