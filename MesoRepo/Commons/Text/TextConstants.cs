using System;
using System.Globalization;
using System.IO;

namespace Commons.Text {
    /// <summary>
    ///     a collection of constants and static methods to handle strings and string conversions.
    /// </summary>
    public static class TextConstants {

        public const string DeIntFmt = "###,##0";

        /// <summary>
        ///     a colon character ':' for use as separator.
        /// </summary>
        public const char CharAsterisk = '*';

        /// <summary>
        ///     a colon character ':'.
        /// </summary>
        public const char CharColon = ':';

        /// <summary>
        ///     a space character ' ' for use as separator.
        /// </summary>
        public const char CharSpace = ' ';

        /// <summary>
        ///     a comma character ',' for use as separator.
        /// </summary>
        public const char CharComma = ',';

        /// <summary>
        ///     a full stop character '.' for use as separator.
        /// </summary>
        public const char CharFullStop = '.';

        /// <summary>
        ///     a hyphen character '-'.
        /// </summary>
        public const char CharHyphen = '-';

        /// <summary>
        ///     a space character '\n' to enforce a new line.
        /// </summary>
        public const char CharNewLine = '\n';

        /// <summary>
        ///     a semicolon character ';'.
        /// </summary>
        public const char CharSemicolon = ';';

        /// <summary>
        ///     a semicolon character '_'.
        /// </summary>
        public const char CharUnderscore = '_';

        /// <summary>
        ///     a forward slash character '/'.
        /// </summary>
        public const char CharSlash = '/';

        /// <summary>
        ///     a backward slash character '/'.
        /// </summary>
        public const char CharBackslash = '\\';

        /// <summary>
        ///     a vertical bar character '|'.
        /// </summary>
        public const char CharVerBar = '|';

        /// <summary>
        ///     a semicolon string ";" for use as separator.
        /// </summary>
        public const string SepSemicolon = ";";

        /// <summary>
        ///     a commy string "," for use as separator.
        /// </summary>
        public const string SepComma = ",";

        /// <summary>
        ///     a forward slash string "/" for use as separator.
        /// </summary>
        public const string SepSlash = "/";

        /// <summary>
        ///     a backward slash string "\" for use as separator.
        /// </summary>
        public const string SepBackslash = @"\";

        /// <summary>
        ///     ISO date format.
        /// </summary>
        public const string IsoDayFormat = "yyyy-MM-dd";

        /// <summary>
        ///     ISO date and time format.
        /// </summary>
        public const string IsoDateFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        ///     Db search string that matches all texts.
        /// </summary>
        public const string DbLikeStringAny = "%";

        /// <summary>
        ///     date and time format for timestamps down to seconds.
        /// </summary>
        public const string TimestampFormat = "yyyyMMddHHmmss";

        /// <summary>
        ///     maximum length of 2 characters.
        /// </summary>
        public const int MaxLength2 = 2;

        /// <summary>
        ///     maximum length of 4 characters.
        /// </summary>
        public const int MaxLength4 = 4;

        /// <summary>
        ///     maximum length of 8 characters.
        /// </summary>
        public const int MaxLength8 = 8;

        /// <summary>
        ///     maximum length of 16 characters.
        /// </summary>
        public const int MaxLength16 = 16;

        /// <summary>
        ///     maximum length of 32 characters.
        /// </summary>
        public const int MaxLength32 = 32;

        /// <summary>
        ///     maximum length of 64 characters.
        /// </summary>
        public const int MaxLength64 = 64;

        /// <summary>
        ///     maximum length of 128 characters.
        /// </summary>
        public const int MaxLength128 = 128;

        /// <summary>
        ///     maximum length of 256 characters.
        /// </summary>
        public const int MaxLength256 = 256;

        /// <summary>
        ///     maximum length of 256 characters.
        /// </summary>
        public const int MaxLength1024 = 1024;

        /// <summary>
        ///     character array containing a semicolon.
        ///     <see cref="TextConstants.SepSemicolon" />
        /// </summary>
        public static readonly char[] SepSemicolonChars = SepSemicolon.ToCharArray();

        /// <summary>
        ///     character array containing a comma.
        ///     <see cref="TextConstants.SepComma" />
        /// </summary>
        public static readonly char[] SepCommaChars = SepComma.ToCharArray();

        /// <summary>
        ///     a forward slash string "/" for use as separator.
        /// </summary>
        public static string PathLocatorString = new string(Path.DirectorySeparatorChar, 1);

        /// <summary>
        ///     Format Info for Iso timestamps.
        ///     <see cref="TextConstants.IsoDateFormat" />
        /// </summary>
        public static DateTimeFormatInfo IsoDateFormatInfo =
            new DateTimeFormatInfo {FullDateTimePattern = IsoDateFormat};

        /// <summary>
        ///     the digit characters [0-9].
        /// </summary>
        public static readonly char[] Digits =
            {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};

        /// <summary>
        ///     get a string containing the specified string of random digits [0-9].
        /// </summary>
        /// <param name="length"> the requested string length. </param>
        /// <returns> a string containing the specified string of random digits [0-9]. </returns>
        public static string RandomNumberString(int length) {
            var random = new Random();
            var numChars = new char[length];
            for (var i = 0; i < length; i++) {
                var index = random.Next(0, Digits.Length);
                numChars[i] = Digits[index];
            }

            return new string(numChars);
        }
    }
}