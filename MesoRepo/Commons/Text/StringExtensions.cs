using System;
using System.Globalization;
using Serilog;

namespace Commons.Text {
    public static class StringExtensions {
        /// <summary>
        ///     parse a text string. Text gets trimmed and if empty afterwards, mapped to string.Empty.
        /// </summary>
        /// <param name="text"> the string to be parsed. </param>
        /// <returns> the trimmed input string, or String.Empty if string was empty. </returns>
        public static string ParseText(this string text) {
            var parsed = string.Empty;
            if (null != text) {
                var trimmed = text.Trim();
                if (!string.IsNullOrEmpty(trimmed)) {
                    parsed = trimmed;
                }
            }

            return parsed;
        }

        /// <summary>
        ///     parse a text string containing an int number.
        ///     Maps handle null values to int.MinValue.
        /// </summary>
        /// <param name="text"> the text string containing the int number. </param>
        /// <returns>
        ///     the int number reresented by the text string, or int.MinValue
        ///     if the text string could not be parsed.
        /// </returns>
        public static int ParseInt(this string text) {
            if (!int.TryParse(text, out var intValue)) {
                Log.Warning(TextResources.ParseInt_Failed, text);
                return int.MinValue;
            }

            return intValue;
        }

        /// <summary>
        ///     parse a text string containing a long number.
        /// </summary>
        /// <param name="text"> the text string containing the long number. </param>
        /// <returns>
        ///     the long number reresented by the text string, or Int64.MinValue if the text string is empty or could not be
        ///     parsed.
        /// </returns>
        public static long ParseLong(this string text) {
            if (!string.IsNullOrEmpty(text)) {
                try {
                    return long.Parse(text);
                } catch (ArgumentNullException anx) {
                    Log.Debug(anx, anx.Message);
                } catch (FormatException fex) {
                    Log.Debug(text, fex);
                } catch (OverflowException ofx) {
                    Log.Debug(text, ofx);
                }
            }

            return long.MinValue;
        }

        public static double ParseDouble(this string text) {
            if (!string.IsNullOrEmpty(text)) {
                try {
                    return double.Parse(text);
                } catch (Exception exception) {
                    Log.Debug(exception, exception.Message);
                }
            }

            return double.NaN;
        }

        public static bool ParseBool(this string text) {
            if (!string.IsNullOrEmpty(text)) {
                bool boolValue;
                if (bool.TryParse(text, out boolValue)) {
                    return boolValue;
                }
            }

            return false;
        }

        public static DateTime ParseDateTime(this string text, string pattern) {
            if (string.IsNullOrEmpty(pattern)) {
                return DateTime.MaxValue;
            }

            var patterns = pattern.Split(new[] {TextConstants.CharSemicolon});
            return ParseDateTime(text, patterns);
        }

        public static DateTime ParseDateTime(this string text, string[] patterns) {
            DateTime dt;
            if (DateTime.TryParseExact(text, patterns, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)) {
                return dt;
            }

            return DateTime.MaxValue;
        }

        /// <summary>
        ///     parse a text string containing an Iso formatted timestamp.
        /// </summary>
        /// <param name="text"> the Iso formatted timestamp. </param>
        /// <returns>
        ///     a DateTime struct that contains the date and time represented by the text string, or DateTime.MaxValue if the
        ///     text string is empty or could not be parsed.
        /// </returns>
        /// <see cref="TextConstants.IsoDateFormatInfo" />
        public static DateTime ParseIsoDateTime(string text) {
            var dateTime = DateTime.MaxValue;
            if (!string.IsNullOrEmpty(ParseText(text))) {
                try {
                    dateTime = DateTime.Parse(text, TextConstants.IsoDateFormatInfo);
                } catch (ArgumentNullException anx) {
                    Log.Debug(anx, anx.Message);
                } catch (FormatException fex) {
                    Log.Debug(text, fex);
                }
            }

            return dateTime;
        }

        /// <summary>
        ///     trims a string and returns a string of at maximum the specified length.
        /// </summary>
        /// <param name="text">the text to be formatted</param>
        /// <param name="maxLength">the maximum allowed string length</param>
        /// <returns> a trimmed string of at maximum the specified length</returns>
        public static string Truncate(this string text, int maxLength) {
            var truncated = ParseText(text);
            if (!string.IsNullOrEmpty(truncated) && maxLength < truncated.Length && 0 < maxLength) {
                truncated = truncated.Substring(0, maxLength);
            }

            return truncated;
        }

        public static string Truncate8(string text) {
            return Truncate(text, TextConstants.MaxLength8);
        }

        public static string Truncate16(string text) {
            return Truncate(text, TextConstants.MaxLength16);
        }

        public static string Truncate32(string text) {
            return Truncate(text, TextConstants.MaxLength32);
        }

        public static string Truncate64(string text) {
            return Truncate(text, TextConstants.MaxLength64);
        }

        public static string Truncate128(string text) {
            return Truncate(text, TextConstants.MaxLength128);
        }

        /// <summary>
        ///     Extension method for string class.
        /// </summary>
        /// <param name="text">The text string for which the first char shall be capitalized.</param>
        /// <returns>the string with first characater capitalized. Null or empty strings are returned unchanged.</returns>
        public static string FirstLetterToUpperCase(this string text) {
            if (string.IsNullOrEmpty(text)) {
                return text;
            }

            var textChars = text.ToCharArray();
            textChars[0] = char.ToUpper(textChars[0]);
            return new string(textChars);
        }

        public static string EmptyAsNull(string text) {
            return string.IsNullOrEmpty(text) ? null : text;
        }
    }
}