﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchChatNet.Models.Events;

public class IngameChatMessage : BaseEvent
{
    public string Text { get; set; }
    public string RawText { get; set; }
    public RenderedTextObject RenderedText { get; set; }
    public User User { get; set; }
}
