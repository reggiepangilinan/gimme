using System;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace Gimme.Core.Validators
{
    public static partial class All
    {
        public static Func<string, Validation<Error, string>> NotLongerThan(int maxLength) =>
            str => Optional(str)
                .Where(s => s.Length <= maxLength)
                .ToValidation(Error.New($"{str} must not be longer than {maxLength}"));

        public static Validation<Error, string> NotEmpty(string str) =>
            Optional(str)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToValidation(Error.New("Empty string"));

        public static Validation<Error, Lst<T>> NotEmpty<T>(Lst<T> list) where T: class  =>
            list.IsEmpty 
            ? Fail<Error, Lst<T>>(Error.New("Empty list")) 
            : Success<Error, Lst<T>>(list);
    }
}
