using System;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace Gimme.Core.Validators
{
    public static partial class All
    {
        public static Func<decimal, Validation<Error, decimal>> AtLeast(decimal minimum) =>
            value => Optional(value)
                .Where(d => d >= minimum)
                .ToValidation(Error.New($"Must be greater or equal to {minimum}"));

        public static Func<decimal, Validation<Error, decimal>> AtMost(decimal max) =>
            value => Optional(value)
                .Where(d => d <= max)
                .ToValidation(Error.New($"Must be less than or equal to {max}"));    
    }
}
