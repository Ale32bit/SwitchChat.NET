using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchChatNet.Models;

public class DiscordUser
{
    public string Type { get; set; } = "discord";
    public string Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Discriminator { get; set; }
    public string Avatar { get; set; }
    public IEnumerable<DiscordRole> Roles { get; set; }

}
