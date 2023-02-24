using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IrcParserNet.Irc;

/// <summary>
/// Базовый класс для создания классов конкретных сообщений.<br/>
/// Класс с информацией сообщения: <see cref="RawIrcMessage"/><br/>
/// <seealso cref="Parameter"/>, <seealso cref="Tag"/> и прочие кидают ошибки, если нужной вещи нет.
/// </summary>
public abstract class BaseIrcMessage
{
    public readonly RawIrcMessage rawIrcMessage;

    protected BaseIrcMessage(RawIrcMessage rawIrcMessage)
    {
        this.rawIrcMessage = rawIrcMessage;
    }

    protected string Parameter(int index) => rawIrcMessage.parameters[index];

    protected string FirstParameter() => rawIrcMessage.parameters.First();

    protected string LastParameter() => rawIrcMessage.parameters.Last();

    protected bool HasTag(string key) => rawIrcMessage.tags.ContainsKey(key);

    protected string Tag(string key) => rawIrcMessage.tags[key];

    protected string OptionalTag(string key) => rawIrcMessage.tags.GetValueOrDefault(key);

    protected int Int32Tag(string key) => int.Parse(Tag(key));

    protected long Int64Tag(string key) => long.Parse(Tag(key));

    protected DateTimeOffset UnixMillisecondsTag(string key) => DateTimeOffset.FromUnixTimeMilliseconds(Int64Tag(key));

    protected int? OptionalIntTag(string key)
    {
        string? value = OptionalTag(key);

        if (!string.IsNullOrEmpty(value))
            return int.Parse(value);

        return null;
    }

    /// <summary>
    /// 1 => true
    /// _ => false
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    protected bool BoolTag(string key) => OptionalTag(key) == "1";

    /// <summary>
    /// 1 => true
    /// null => null
    /// _ => false
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    protected bool? OptionalBoolTag(string key) => OptionalTag(key) switch
    {
        "1" => true,
        null => null,
        _ => false
    };
}
