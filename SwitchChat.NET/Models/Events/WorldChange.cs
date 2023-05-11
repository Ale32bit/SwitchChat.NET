using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchChatNet.Models.Events;

public class WorldChange : BaseEvent
{
    public User User { get; set; }
    public string Origin { get; set; }
    public string Destination { get; set; }
}
