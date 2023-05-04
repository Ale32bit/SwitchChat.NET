using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchChatNet.Models.Events;

public class ServerRestartScheduled : BaseEvent
{
    public string RestartType { get; set; }
    public int RestartSeconds { get; set;}
    public DateTime RestartAt { get; set; }
    public new DateTime Time { get; set;}
}
