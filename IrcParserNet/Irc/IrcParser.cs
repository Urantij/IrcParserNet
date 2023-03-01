using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IrcParserNet.Irc;

public static class IrcParser
{
    public static RawIrcMessage Parse(in string ircString)
    {
        // https://tools.ietf.org/html/rfc1459.html#section-2.3
        // https://ircv3.net/specs/extensions/message-tags.html

        int position = 0;
        int nextSpace;

        Dictionary<string, string?>? tags;
        if (ircString[0] == '@')
        {
            tags = new Dictionary<string, string?>();

            nextSpace = ircString.IndexOf(' ');
            // if (nextSpace == -1)
            //     throw new Exception($"Bad irc message tags {ircString}");

            ReadOnlySpan<char> remainingTagsSpan = ircString.AsSpan(1, nextSpace - 1);
            ReadOnlySpan<char> tagSpan;
            do
            {
                int delimiterIndex = remainingTagsSpan.IndexOf(';');
                if (delimiterIndex == -1)
                {
                    tagSpan = remainingTagsSpan;
                }
                else
                {
                    tagSpan = remainingTagsSpan[..delimiterIndex];

                    remainingTagsSpan = remainingTagsSpan[(delimiterIndex + 1)..];
                }

                int spaceDelimiterIndex = tagSpan.IndexOf('=');

                string key = new(tagSpan[..spaceDelimiterIndex]);
                string? value = new(tagSpan[(spaceDelimiterIndex + 1)..]);

                /* Implementations MUST interpret empty tag values (e.g. foo=) as equivalent to missing tag values (e.g. foo).
                 * Specifications MUST NOT differentiate meaning between tags with empty and missing values.*/

                if (string.IsNullOrEmpty(value)) value = null;

                tags[key] = value;
            }
            while (tagSpan != remainingTagsSpan);

            // ['@' <tags> <SPACE>]
            position = nextSpace + 1;
        }
        else tags = null;

        /*Each IRC message may consist of up to three main parts: the prefix
        (optional), the command, and the command parameters (of which there
        may be up to 15).  The prefix, command, and all parameters are
        separated by one (or more) ASCII space character(s) (0x20).

        The presence of a prefix is indicated with a single leading ASCII
        colon character (':', 0x3b), which must be the first character of the
        message itself.  There must be no gap (whitespace) between the colon
        and the prefix. */

        string? prefix;
        if (ircString[position] == ':')
        {
            position++;

            // [':' <prefix> <SPACE> ]
            nextSpace = ircString.IndexOf(' ', position);

            // if (nextSpace == -1)
            //     throw new Exception($"Bad irc message prefix {ircString}");

            prefix = ircString[position..nextSpace];

            position = nextSpace + 1;
        }
        else prefix = null;

        nextSpace = ircString.IndexOf(' ', position);

        string command;
        string[]? parameters;
        if (nextSpace != -1)
        {
            command = ircString[position..nextSpace];

            position = nextSpace + 1;
            // В теории можно посчитать и сделать массив, но мне лень.
            List<string> parametersList = new();

            ReadOnlySpan<char> remainingParametersSpan = ircString.AsSpan(position);
            ReadOnlySpan<char> parameterSpan;
            do
            {
                int delimiterIndex = remainingParametersSpan.IndexOf(' ');
                if (remainingParametersSpan[0] == ':')
                {
                    parameterSpan = remainingParametersSpan = remainingParametersSpan[1..];
                }
                else if (delimiterIndex == -1)
                {
                    parameterSpan = remainingParametersSpan;
                }
                else
                {
                    parameterSpan = remainingParametersSpan[..delimiterIndex];

                    remainingParametersSpan = remainingParametersSpan[(delimiterIndex + 1)..];
                }

                parametersList.Add(new string(parameterSpan));
            }
            while (parameterSpan != remainingParametersSpan);

            parameters = parametersList.ToArray();
        }
        else
        {
            command = ircString[position..];
            parameters = null;
        }

        return new RawIrcMessage(tags, prefix, command, parameters);
    }
}
