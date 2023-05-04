using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchChatNet.Models.Events;

public class ChatboxChatMessage : BaseEvent
{
    public string Text { get; set; }
    public string RawText { get; set; }
    public RenderedTextObject RenderedText { get; set; }
    public User User { get; set; }
    public string Name { get; set; }
    public string RawName { get; set; }
}
