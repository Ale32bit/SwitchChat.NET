using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchChatNet.Models.Events;

public class ChatboxCommand : BaseEvent
{
    public User User { get; set; }
    public string Command { get; set; }
    public IEnumerable<string> Args { get; set; }
    public bool OwnerOnly { get; set; }
}
