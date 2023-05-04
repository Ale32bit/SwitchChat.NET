using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchChatNet.Models.Events;

public class ServerRestartCancelled : BaseEvent
{
    public string RestartType { get; set; }
    public DateTime Date { get; set; }
}
