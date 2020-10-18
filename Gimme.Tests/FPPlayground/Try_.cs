using System;
using Xunit;
using LanguageExt;
using static LanguageExt.Prelude;
using Shouldly;

namespace Gimme.Tests.FPPlayground
{
    public class Try_
    {
        const string SUCCESS_MESSAGE = "Success";
        const string FAIL_MESSAGE = "Fail";
        Func<Unit, string> succ = x => SUCCESS_MESSAGE;
        Func<Exception, string> fail = e => e.Message;
        private Unit OperationThatThrowsException()
        {
            throw new Exception(FAIL_MESSAGE);
        }

        private Unit SuccessfulOperation() => unit;

        [Fact]
        public void Try_HandleException()
        =>
            Try(() => OperationThatThrowsException())
            .Match(succ, fail)
            .ShouldBe(FAIL_MESSAGE);

        [Fact]
        public void Try_SuccessfulOperation()
        =>
            Try(() => SuccessfulOperation())
            .Match(succ, fail)
            .ShouldBe(SUCCESS_MESSAGE);
    }
}
