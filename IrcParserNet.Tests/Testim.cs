using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IrcParserNet.Irc;

namespace IrcParserNet.Tests;

public class Testim
{
    [Fact]
    public void TestFromString1()
    {
        string testString = "@key=value;key2=value2;key3= :prefix command one two :three";
        RawIrcMessage message = IrcParser.Parse(testString);

        Assert.Equal("value", message.tags["key"]);
        Assert.Equal("value2", message.tags["key2"]);
        Assert.Null(message.tags["key3"]);

        Assert.Equal("prefix", message.prefix);
        Assert.Equal("command", message.command);
        Assert.Equal("one", message.parameters[0]);
        Assert.Equal("two", message.parameters[1]);
        Assert.Equal("three", message.parameters[2]);
    }

    [Fact]
    public void TestFromString2()
    {
        string testString = "@key=value;key2=value2;key3= :prefix command one two :three four";
        RawIrcMessage message = IrcParser.Parse(testString);

        Assert.Equal("value", message.tags["key"]);
        Assert.Equal("value2", message.tags["key2"]);
        Assert.Null(message.tags["key3"]);

        Assert.Equal("prefix", message.prefix);
        Assert.Equal("command", message.command);
        Assert.Equal("one", message.parameters[0]);
        Assert.Equal("two", message.parameters[1]);
        Assert.Equal("three four", message.parameters[2]);
    }

    [Fact]
    public void TestToString()
    {
        RawIrcMessage message = new(new Dictionary<string, string?>() 
        {
            { "key", "value" },
            { "key2", "value2" },
            { "key3", null}
        }, "prefix", "command", new string[] { "one", "two", "three four" });

        string str = message.GenericMakeString();

        Assert.Equal("@key=value;key2=value2;key3= :prefix command one two :three four", str);
    }
}
