using System;
using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using Shouldly;

namespace Gimme.Tests.FPPlayground
{
    public class All_ {
        Func<string, string, int> test => (string x, string y) => 0;

        [Fact]
        public void Apply_Sample()
        {

        }
    }
}