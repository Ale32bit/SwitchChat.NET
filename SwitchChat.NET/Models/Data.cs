using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SwitchChatNet.Models;

public class Data
{
    public int? Id { get; set; }
    public bool Ok { get; set; }
    public string Type { get; set; }
}
