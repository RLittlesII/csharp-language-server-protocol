﻿using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Xunit;

namespace Lsp.Tests.Models
{
    public class PositionTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new Position(1, 1);
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<Position>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
