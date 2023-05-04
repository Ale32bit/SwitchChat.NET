using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchChatNet.Models.Events;

public class ClosingEvent
{
    public string CloseReason { get; set; }
    public string Reason { get; set; }
}
