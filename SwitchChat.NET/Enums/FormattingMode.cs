using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SwitchChatNet.Enums;

public enum FormattingMode
{
    [EnumMember(Value = "format")]
    Format,

    [EnumMember(Value = "markdown")]
    Markdown
}
