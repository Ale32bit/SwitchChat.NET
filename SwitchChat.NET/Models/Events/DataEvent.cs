using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchChatNet.Models.Events;

public class DataEvent
{
    public Data Data { get; set; }
    public string Payload { get; set; }
}
