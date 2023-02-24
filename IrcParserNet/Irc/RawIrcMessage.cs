using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrcParserNet.Irc;

public class RawIrcMessage
{
    /// <summary>
    /// Если тегов нет, то null.
    /// </summary>
    public readonly IReadOnlyDictionary<string, string?>? tags;

    public readonly string? prefix;

    public readonly string command;

    /// <summary>
    /// Если параметров нет, то null
    /// </summary>
    public readonly string[]? parameters;

    public RawIrcMessage(IReadOnlyDictionary<string, string?>? tags, string? prefix, string command, string[]? parameters)
    {
        this.tags = tags;
        this.prefix = prefix;
        this.command = command;
        this.parameters = parameters;
    }

    public virtual string MakeString() => GenericMakeString();

    public string GenericMakeString()
    {
        StringBuilder sb = new();

        if (tags?.Count > 0)
        {
            sb.Append('@');

            int index = 0;
            foreach (KeyValuePair<string, string?> pair in tags)
            {
                sb.Append(pair.Key);
                sb.Append('=');
                sb.Append(pair.Value);

                if (++index != tags.Count)
                {
                    sb.Append(';');
                }
            }

            sb.Append(' ');
        }

        if (prefix != null)
        {
            sb.Append(':');
            sb.Append(prefix);

            sb.Append(' ');
        }

        sb.Append(command);

        if (parameters?.Length > 0)
        {
            sb.Append(' ');
            
            foreach (string parameter in parameters.SkipLast(1))
            {
                sb.Append(parameter);
                sb.Append(' ');
            }

            // По спецификации вроде бы всегда : должна быть.
            // Но на практике нет. Хызы.
            string lastParameter = parameters.Last();
            if (lastParameter.Contains(' '))
            {
                sb.Append(':');
            }
            sb.Append(lastParameter);
        }

        return sb.ToString();
    }
}
