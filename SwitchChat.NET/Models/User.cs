using SwitchChatNet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchChatNet.Models;

public class User
{
    public string Type { get; set; } = "ingame";
    public string Group { get; set; }
    public string? Pronouns { get; set; }
    public string? World { get; set; }
    public string UUID { get; set; }
    public string DisplayName { get; set; }
    public string Name { get; set; }
    public bool? AFK { get; set; }
    public bool Alt { get; set; }
    public bool Bot { get; set; }
    public SupporterTier Suppoter { get; set; }
}
