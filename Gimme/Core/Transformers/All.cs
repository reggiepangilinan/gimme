using System;
using LanguageExt.Common;

namespace Gimme.Core.Transformers
{
    public static class All
    {
        public static Error ExceptionToError(Exception exception) => Error.New(exception.Message);
    }
}