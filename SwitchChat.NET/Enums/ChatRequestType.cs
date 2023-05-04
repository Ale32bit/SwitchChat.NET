using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SwitchChatNet.Enums;

public enum ChatRequestType
{
    [EnumMember(Value = "say")]
    Say,

    [EnumMember(Value = "tell")]
    Tell,
}

