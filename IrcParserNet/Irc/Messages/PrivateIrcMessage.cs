using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IrcParserNet.Irc.Messages;

public class PrivateIrcMessage : BaseIrcMessage
{
    public readonly string? from;
    public readonly string to;
    public readonly string text;

    public PrivateIrcMessage(RawIrcMessage rawIrcMessage) 
        : base(rawIrcMessage)
    {
        from = rawIrcMessage.prefix;
        to = FirstParameter();
        text = LastParameter();
    }
}
