﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buttplug.Core;
using Xunit;

namespace ButtplugTest.Core
{
    public class ButtplugJsonMessageParserTests
    {
        [Fact]
        public void JsonConversionTest()
        {
            var m = new Buttplug.Messages.Test("ThisIsATest");
            var msg = ButtplugJsonMessageParser.Serialize(m);
            Assert.True(msg.IsSome);
            msg.IfSome((x) => Assert.Equal(x, "{\"Test\":{\"TestString\":\"ThisIsATest\"}}"));
        }

        // Not valid JSON
        [InlineData("not a json message")]
        // Valid json object but no contents
        [InlineData("{}")]
        // Valid json but not an object
        [InlineData("[]")]
        // Not a message type
        [InlineData("{\"NotAMessage\":{}}")]
        // Valid json and message type but not in correct format
        [InlineData("{\"Test\":[]}")]
        // Valid json and message type but not in correct format
        [InlineData("{\"Test\":{}}")]
        // Valid json and message type but with erroneous content
        [InlineData("{\"Test\":{\"TestString\":\"Error\"}}")]
        // Valid json and message type but with extra content
        [InlineData("{\"Test\":{\"TestString\":\"Yup\", \"NotAField\":\"NotAValue\"}}")]
        [Theory]
        public void DeserializeIncorrectMessages(string x)
        {
            var p = new ButtplugJsonMessageParser();
            Assert.True(p.Deserialize(x).IsLeft);
        }

        [Fact]
        public void DeserializeCorrectMessage()
        {
            var p = new ButtplugJsonMessageParser();
            var m = p.Deserialize("{\"Test\":{\"TestString\":\"Test\"}}");
            Assert.True(m.IsRight);
            m.IfRight(x =>
            {
                switch (x)
                {
                    case Buttplug.Messages.Test tm:
                        Assert.True(tm.TestString == "Test");
                        break;
                    default:
                        Assert.True(false);
                        break;
                }
            });
        }
    }
}