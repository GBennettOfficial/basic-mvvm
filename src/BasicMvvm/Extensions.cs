using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace BasicMvvm
{
    public static class Extensions
    {
        public static PropertyRuleBuilder<T, object?> Null<T>(this PropertyRuleBuilder<T, object?> builder) where T : UserInputBaseVm<T>
        {
            return builder.Must(obj => obj is null)
                .WithMessage("Input must be null.");
        }

        public static PropertyRuleBuilder<T, object?> NotNull<T>(this PropertyRuleBuilder<T, object?> builder) where T : UserInputBaseVm<T>
        {
            return builder.Must(obj => obj is not null)
                .WithMessage("Input must not be null.");
        }

        public static PropertyRuleBuilder<T, object?> Equal<T>(this PropertyRuleBuilder<T, object?> builder, object? arg) where T : UserInputBaseVm<T>
        {
            return builder.Must(obj => obj is null || obj.Equals(arg))
                .WithMessage($"Input must be equal to {arg}.");
        }

        public static PropertyRuleBuilder<T, string?> Matches<T>(this PropertyRuleBuilder<T, string?> builder, [StringSyntax(StringSyntaxAttribute.Regex)] string pattern, RegexOptions options = RegexOptions.None) where T : UserInputBaseVm<T>
        {
            return builder.Must(str => string.IsNullOrEmpty(str) || Regex.IsMatch(str, pattern, options));
        }

        public static PropertyRuleBuilder<T, string?> Empty<T>(this PropertyRuleBuilder<T, string?> builder) where T : UserInputBaseVm<T>
        {
            return builder.Must(str => str is null || str == "");
        }

        public static PropertyRuleBuilder<T, string?> NotEmpty<T>(this PropertyRuleBuilder<T, string?> builder) where T : UserInputBaseVm<T>
        {
            return builder.Must(str => str is null && str != "");
        }

        public static PropertyRuleBuilder<T, string?> MinimumLength<T>(this PropertyRuleBuilder<T, string?> builder, int minLength) where T : UserInputBaseVm<T>
        {
            return builder.Must(str =>
            {
                if (string.IsNullOrWhiteSpace(str))
                    return true;

                str = str.Trim();
                return str.Length >= minLength;
            }).WithMessage("Input is too short.");
        }

        public static PropertyRuleBuilder<T, string?> MaximumLength<T>(this PropertyRuleBuilder<T, string?> builder, int maxLength) where T : UserInputBaseVm<T>
        {
            return builder.Must(str =>
            {
                if (string.IsNullOrWhiteSpace(str))
                    return true;
                str = str.Trim();
                return str.Length <= maxLength;
            }).WithMessage("Input is too long.");
        }

        public static PropertyRuleBuilder<T, IEnumerable<TEnumerable>?> MinimumCount<T, TEnumerable>(this PropertyRuleBuilder<T, IEnumerable<TEnumerable>?> builder, int minLength, int maxLength) where T : UserInputBaseVm<T>
        {
            return builder.Must(collection =>
            {
                if (collection is null)
                    return true;
                var count = collection.Count();
                return count >= minLength && count <= maxLength;
            }).WithMessage("Input count it too few.");
        }

        public static PropertyRuleBuilder<T, IEnumerable<TEnumerable>?> MaximumCount<T, TEnumerable>(this PropertyRuleBuilder<T, IEnumerable<TEnumerable>?> builder, int maxLength) where T : UserInputBaseVm<T>
        {
            return builder.Must(collection =>
            {
                if (collection is null)
                    return true;
                var count = collection.Count();
                return count <= maxLength;
            }).WithMessage("Input count it too many.");
        }

        public static PropertyRuleBuilder<T, string?> ParseDateTime<T>(this PropertyRuleBuilder<T, string?> builder) where T : UserInputBaseVm<T>
        {
            return builder.Must(str =>
            {
                if (string.IsNullOrWhiteSpace(str))
                    return true;
                str = str.Trim();
                return System.DateTime.TryParse(str, out _);
            }).WithMessage("Input is not in date/time format.");
        }

        public static PropertyRuleBuilder<T, string?> ParseExactDateTime<T>(this PropertyRuleBuilder<T, string?> builder, string format) where T : UserInputBaseVm<T>
        {
            return builder.Must(str =>
            {
                if (string.IsNullOrWhiteSpace(str))
                    return true;
                str = str.Trim();
                return System.DateTime.TryParseExact(str, format, null, System.Globalization.DateTimeStyles.None, out _);
            }).WithMessage("Input is not in a valid date/time format.");
        }

        public static PropertyRuleBuilder<T, string?> MinimumDateTime<T>(this PropertyRuleBuilder<T, string?> builder, DateTime min) where T : UserInputBaseVm<T>
        {
            return builder.Must(str =>
            {
                if (string.IsNullOrWhiteSpace(str))
                    return true;

                str = str.Trim();
                if (DateTime.TryParse(str, out var dt) == false)
                    return true;

                return dt >= min;
            }).WithMessage($"Input is too early.");
        }

        public static PropertyRuleBuilder<T, string?> MaximumDateTime<T>(this PropertyRuleBuilder<T, string?> builder, DateTime max) where T : UserInputBaseVm<T>
        {
            return builder.Must(str =>
            {
                if (string.IsNullOrWhiteSpace(str))
                    return true;

                str = str.Trim();
                if (DateTime.TryParse(str, out var dt) == false)
                    return true;

                return dt <= max;
            }).WithMessage($"Input is too late.");
        }

        public static PropertyRuleBuilder<T, string?> ParseInteger<T>(this PropertyRuleBuilder<T, string?> builder) where T : UserInputBaseVm<T>
        {
            return builder.Must(str =>
            {
                if (string.IsNullOrWhiteSpace(str))
                    return true;
                str = str.Trim();

                if (int.TryParse(str, out _) == false)
                    return false;

                return true;
            }).WithMessage("Input is not a valid number.");
        }

        public static PropertyRuleBuilder<T, string?> MinimumInteger<T>(this PropertyRuleBuilder<T, string?> builder, int min) where T : UserInputBaseVm<T>
        {
            return builder.Must(str =>
            {
                if (string.IsNullOrWhiteSpace(str))
                    return true;

                str = str.Trim();
                if (int.TryParse(str, out var value) == false)
                    return true;

                return value >= min;
            }).WithMessage($"Input is below minimum.");
        }

        public static PropertyRuleBuilder<T, string?> MaximumInteger<T>(this PropertyRuleBuilder<T, string?> builder, int max) where T : UserInputBaseVm<T>
        {
            return builder.Must(str =>
            {
                if (string.IsNullOrWhiteSpace(str))
                    return true;

                str = str.Trim();
                if (int.TryParse(str, out var value) == false)
                    return true;

                return value <= max;
            }).WithMessage($"Input is above maximum.");
        }
    }
}
