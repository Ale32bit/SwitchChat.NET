using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchChatNet.Models.Events;

public class Leave : BaseEvent
{
    public User User { get; set; }
}
