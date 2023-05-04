using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchChatNet.Models;

public class RenderedTextObject
{
    public string? Text { get; set; }
    public IEnumerable<RenderedTextObject> Extra { get; set; }

}
