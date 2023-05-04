using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchChatNet.Models.Events;

public class BaseEvent : Data
{
    public string Event { get; set; }
    public DateTime Time { get; set; }
}
