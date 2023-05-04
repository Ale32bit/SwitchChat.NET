using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchChatNet.Models.Events;

public class ErrorEvent
{
    public string Error { get; set; }
    public string Message { get; set; }
}
