using System.Text;
using System;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace SCKRM
{
    public static class StringUtility
    {
        public static string ConstEnvironmentVariable(this string value)
        {
            if (value == null)
                return null;

            value = value.Replace("%DataPath%", Kernel.dataPath);
            value = value.Replace("%StreamingAssetsPath%", Kernel.streamingAssetsPath);
            value = value.Replace("%PersistentDataPath%", Kernel.persistentDataPath);

            value = value.Replace("%CompanyName%", Kernel.companyName);
            value = value.Replace("%ProductName%", Kernel.productName);
            value = value.Replace("%Version%", Kernel.version);

            return value;
        }

        /// <summary>
        /// (text = "AddSpacesToSentence") = "Add Spaces To Sentence"
        /// </summary>
        /// <param name="text">텍스트</param>
        /// <param name="preserveAcronyms">약어(준말) 보존 (true = (UnscaledFPSDeltaTime = Unscaled FPS Delta Time), false = (UnscaledFPSDeltaTime = Unscaled FPSDelta Time))</param>
        /// <returns></returns>
        public static string AddSpacesToSentence(this string text, bool preserveAcronyms = true)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);

            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) || (preserveAcronyms && char.IsUpper(text[i - 1]) && i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                        newText.Append(' ');
                newText.Append(text[i]);
            }

            return newText.ToString();
        }

        #region KeyCode to String
        /// <summary>
        /// (keyCode = KeyCode.RightArrow) = "→"
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        public static string KeyCodeToString(this KeyCode keyCode, bool simply = false)
        {
            string text;
            switch (keyCode)
            {
                case KeyCode.Escape:
                    text = "ESC";
                    break;
                case KeyCode.Return when !simply:
                    text = "Enter";
                    break;
                case KeyCode.Return when simply:
                    text = "↵";
                    break;
                case KeyCode.Alpha0:
                    text = "0";
                    break;
                case KeyCode.Alpha1:
                    text = "1";
                    break;
                case KeyCode.Alpha2:
                    text = "2";
                    break;
                case KeyCode.Alpha3:
                    text = "3";
                    break;
                case KeyCode.Alpha4:
                    text = "4";
                    break;
                case KeyCode.Alpha5:
                    text = "5";
                    break;
                case KeyCode.Alpha6:
                    text = "6";
                    break;
                case KeyCode.Alpha7:
                    text = "7";
                    break;
                case KeyCode.Alpha8:
                    text = "8";
                    break;
                case KeyCode.Alpha9:
                    text = "9";
                    break;
                case KeyCode.AltGr when simply:
                    text = "AG";
                    break;
                case KeyCode.Ampersand:
                    text = "&";
                    break;
                case KeyCode.Asterisk:
                    text = "*";
                    break;
                case KeyCode.At:
                    text = "@";
                    break;
                case KeyCode.BackQuote:
                    text = "`";
                    break;
                case KeyCode.Backslash:
                    text = "\\";
                    break;
                case KeyCode.Caret:
                    text = "^";
                    break;
                case KeyCode.Colon:
                    text = ":";
                    break;
                case KeyCode.Comma:
                    text = ",";
                    break;
                case KeyCode.Dollar:
                    text = "$";
                    break;
                case KeyCode.DoubleQuote:
                    text = "\"";
                    break;
                case KeyCode.Equals:
                    text = "=";
                    break;
                case KeyCode.Exclaim:
                    text = "!";
                    break;
                case KeyCode.Greater:
                    text = ">";
                    break;
                case KeyCode.Hash:
                    text = "#";
                    break;
                case KeyCode.Keypad0 when !simply:
                    text = "Keypad 0";
                    break;
                case KeyCode.Keypad1 when !simply:
                    text = "Keypad 1";
                    break;
                case KeyCode.Keypad2 when !simply:
                    text = "Keypad 2";
                    break;
                case KeyCode.Keypad3 when !simply:
                    text = "Keypad 3";
                    break;
                case KeyCode.Keypad4 when !simply:
                    text = "Keypad 4";
                    break;
                case KeyCode.Keypad5 when !simply:
                    text = "Keypad 5";
                    break;
                case KeyCode.Keypad6 when !simply:
                    text = "Keypad 6";
                    break;
                case KeyCode.Keypad7 when !simply:
                    text = "Keypad 7";
                    break;
                case KeyCode.Keypad8 when !simply:
                    text = "Keypad 8";
                    break;
                case KeyCode.Keypad9 when !simply:
                    text = "Keypad 9";
                    break;
                case KeyCode.KeypadDivide when !simply:
                    text = "Keypad /";
                    break;
                case KeyCode.KeypadEnter when !simply:
                    text = "Keypad ↵";
                    break;
                case KeyCode.KeypadEquals when !simply:
                    text = "Keypad =";
                    break;
                case KeyCode.KeypadMinus when !simply:
                    text = "Keypad -";
                    break;
                case KeyCode.KeypadMultiply when !simply:
                    text = "Keypad *";
                    break;
                case KeyCode.KeypadPeriod when !simply:
                    text = "Keypad .";
                    break;
                case KeyCode.KeypadPlus when !simply:
                    text = "Keypad +";
                    break;
                case KeyCode.Keypad0 when simply:
                    text = "K0";
                    break;
                case KeyCode.Keypad1 when simply:
                    text = "K1";
                    break;
                case KeyCode.Keypad2 when simply:
                    text = "K2";
                    break;
                case KeyCode.Keypad3 when simply:
                    text = "K3";
                    break;
                case KeyCode.Keypad4 when simply:
                    text = "K4";
                    break;
                case KeyCode.Keypad5 when simply:
                    text = "K5";
                    break;
                case KeyCode.Keypad6 when simply:
                    text = "K6";
                    break;
                case KeyCode.Keypad7 when simply:
                    text = "K7";
                    break;
                case KeyCode.Keypad8 when simply:
                    text = "K8";
                    break;
                case KeyCode.Keypad9 when simply:
                    text = "K9";
                    break;
                case KeyCode.KeypadDivide when simply:
                    text = "K/";
                    break;
                case KeyCode.KeypadEnter when simply:
                    text = "K↵";
                    break;
                case KeyCode.KeypadEquals when simply:
                    text = "K=";
                    break;
                case KeyCode.KeypadMinus when simply:
                    text = "K-";
                    break;
                case KeyCode.KeypadMultiply when simply:
                    text = "K*";
                    break;
                case KeyCode.KeypadPeriod when simply:
                    text = "K.";
                    break;
                case KeyCode.KeypadPlus when simply:
                    text = "K+";
                    break;
                case KeyCode.LeftApple:
                    text = "Left Command";
                    break;
                case KeyCode.LeftBracket:
                    text = "[";
                    break;
                case KeyCode.LeftCurlyBracket:
                    text = "{";
                    break;
                case KeyCode.LeftParen:
                    text = "(";
                    break;
                case KeyCode.LeftWindows when simply:
                    text = "LW";
                    break;
                case KeyCode.Less:
                    text = "<";
                    break;
                case KeyCode.Minus:
                    text = "-";
                    break;
                case KeyCode.Mouse0 when !simply:
                    text = "Left Mouse";
                    break;
                case KeyCode.Mouse1 when !simply:
                    text = "Right Mouse";
                    break;
                case KeyCode.Mouse2 when !simply:
                    text = "Middle Mouse";
                    break;
                case KeyCode.Mouse0 when simply:
                    text = "LM";
                    break;
                case KeyCode.Mouse1 when simply:
                    text = "RM";
                    break;
                case KeyCode.Mouse2 when simply:
                    text = "MM";
                    break;
                case KeyCode.Mouse3 when simply:
                    text = "M3";
                    break;
                case KeyCode.Mouse4 when simply:
                    text = "M4";
                    break;
                case KeyCode.Mouse5 when simply:
                    text = "M5";
                    break;
                case KeyCode.Mouse6 when simply:
                    text = "M6";
                    break;
                case KeyCode.Percent:
                    text = "%";
                    break;
                case KeyCode.Period:
                    text = ".";
                    break;
                case KeyCode.Pipe:
                    text = "|";
                    break;
                case KeyCode.Plus:
                    text = "+";
                    break;
                case KeyCode.Print when !simply:
                    text = "Print Screen";
                    break;
                case KeyCode.Print when simply:
                    text = "PS";
                    break;
                case KeyCode.Question:
                    text = "?";
                    break;
                case KeyCode.Quote:
                    text = "'";
                    break;
                case KeyCode.RightApple:
                    text = "Right Command";
                    break;
                case KeyCode.RightBracket:
                    text = "]";
                    break;
                case KeyCode.RightCurlyBracket:
                    text = "}";
                    break;
                case KeyCode.RightParen:
                    text = ")";
                    break;
                case KeyCode.RightWindows when simply:
                    text = "LW";
                    break;
                case KeyCode.Semicolon:
                    text = ";";
                    break;
                case KeyCode.Slash:
                    text = "/";
                    break;
                case KeyCode.Space when simply:
                    text = "␣";
                    break;
                case KeyCode.SysReq when !simply:
                    text = "Print Screen";
                    break;
                case KeyCode.SysReq when simply:
                    text = "PS";
                    break;
                case KeyCode.Tilde:
                    text = "~";
                    break;
                case KeyCode.Underscore:
                    text = "_";
                    break;
                case KeyCode.UpArrow:
                    text = "↑";
                    break;
                case KeyCode.DownArrow:
                    text = "↓";
                    break;
                case KeyCode.LeftArrow:
                    text = "←";
                    break;
                case KeyCode.RightArrow:
                    text = "→";
                    break;
                case KeyCode.LeftControl when !simply:
                    text = "Left Ctrl";
                    break;
                case KeyCode.RightControl when !simply:
                    text = "Right Ctrl";
                    break;
                case KeyCode.LeftControl when simply:
                    text = "LC";
                    break;
                case KeyCode.RightControl when simply:
                    text = "RC";
                    break;
                case KeyCode.LeftAlt when simply:
                    text = "LA";
                    break;
                case KeyCode.RightAlt when simply:
                    text = "RA";
                    break;
                case KeyCode.LeftShift when simply:
                    text = "L⇧";
                    break;
                case KeyCode.RightShift when simply:
                    text = "R⇧";
                    break;
                case KeyCode.Backspace when simply:
                    text = "B←";
                    break;
                case KeyCode.Delete when simply:
                    text = "D←";
                    break;
                case KeyCode.PageUp when simply:
                    text = "P↑";
                    break;
                case KeyCode.PageDown when simply:
                    text = "P↓";
                    break;
                default:
                    text = keyCode.ToString().AddSpacesToSentence();
                    break;
            }

            return text;
        }
        #endregion

        #region To Bar
        /// <summary>
        /// (value = 5, max = 10, length = 10) = "■■■■■□□□□□"
        /// </summary>
        /// <param name="value"></param>
        /// <param name="max"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ToBar(this int value, int max, int length, string fill = "■", string half = "▣", string empty = "□") => ToBar((double)value, max, length, fill, half, empty);

        /// <summary>
        /// (value = 5.5, max = 10, length = 10) = "■■■■■▣□□□□"
        /// </summary>
        /// <param name="value"></param>
        /// <param name="max"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [WikiIgnore] public static string ToBar(this float value, float max, int length, string fill = "■", string half = "▣", string empty = "□") => ToBar((double)value, max, length, fill, half, empty);

        /// <summary>
        /// (value = 5.5, max = 10, length = 10) = "■■■■■▣□□□□"
        /// </summary>
        /// <param name="value"></param>
        /// <param name="max"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static string ToBar(this double value, double max, int length, string fill = "■", string half = "▣", string empty = "□")
        {
            if (fill == null)
                fill = "";
            if (half == null)
                half = "";
            if (empty == null)
                empty = "";

            string text = "";

            for (double i = 0.5; i < length + 0.5; i++)
            {
                if (value / max >= i / length)
                    text += fill;
                else
                {
                    if (value / max >= (i - 0.5) / length)
                        text += half;
                    else
                        text += empty;
                }
            }
            return text;
        }
        #endregion

        public static string DataSizeToString(this long byteSize) => ByteTo(byteSize, out string space) + space;

        /// <summary>
        /// 데이터 크기를(바이트) 문자열로 바꿔줍니다 (B, KB, MB, GB, TB, PB, EB, ZB, YB)
        /// </summary>
        /// <param name="byteSize">계산할 용량</param>
        /// <param name="digits">계산된 용량에서 표시할 소수점 자리수</param>
        /// <returns>계산된 용량</returns>
        [WikiIgnore] public static string DataSizeToString(this long byteSize, int digits) => ByteTo(byteSize, out string space).Round(digits) + space;

        /// <summary>
        /// 데이터 크기를(바이트) 적절하게 바꿔줍니다 (B, KB, MB, GB, TB, PB, EB, ZB, YB)
        /// </summary>
        /// <param name="byteSize">계산할 용량</param>
        public static double ByteTo(long byteSize, out string space)
        {
            int loopCount = 0;
            double size = byteSize;

            while (size > 1024.0)
            {
                size /= 1024.0;
                loopCount++;
            }

            if (loopCount == 0)
                space = "B";
            else if (loopCount == 1)
                space = "KB";
            else if (loopCount == 2)
                space = "MB";
            else if (loopCount == 3)
                space = "GB";
            else if (loopCount == 4)
                space = "TB";
            else if (loopCount == 5)
                space = "PB";
            else if (loopCount == 6)
                space = "EB";
            else if (loopCount == 7)
                space = "ZB";
            else
                space = "YB";

            return size;
        }

        [WikiIgnore]
        public static double ByteTo(long byteSize, DataSizeType dataSizeType, out string space)
        {
            double size = byteSize / Math.Pow(1024, (int)dataSizeType);
            space = dataSizeType.ToString().ToUpper();

            return size;
        }

        public static string[] QuotedSplit(this string text, string separator) => text.QuotedSplit2(separator).ToArray();

        //https://codereview.stackexchange.com/a/166801
        static IEnumerable<string> QuotedSplit2(this string text, string separator)
        {
            const char quote = '\"';

            StringBuilder sb = new StringBuilder(text.Length);
            int counter = 0;
            while (counter < text.Length)
            {
                // if starts with delmiter if so read ahead to see if matches
                if (separator[0] == text[counter] && separator.SequenceEqual(ReadNext(text, counter, separator.Length)))
                {
                    yield return sb.ToString();

                    sb.Clear();
                    counter += separator.Length; // Move the counter past the delimiter 
                }
                else if (text[counter] == quote) // if we hit a quote read until we hit another quote or end of string
                {
                    sb.Append(text[counter++]);
                    while (counter < text.Length && text[counter] != quote)
                        sb.Append(text[counter++]);

                    // if not end of string then we hit a quote add the quote
                    if (counter < text.Length)
                        sb.Append(text[counter++]);
                }
                else
                    sb.Append(text[counter++]);
            }

            if (sb.Length > 0)
                yield return sb.ToString();
        }

        static IEnumerable<char> ReadNext(string str, int currentPosition, int count)
        {
            for (var i = 0; i < count; i++)
            {
                if (currentPosition + i >= str.Length)
                    yield break;
                else
                    yield return str[currentPosition + i];
            }
        }

        public enum DataSizeType
        {
            b,
            kb,
            mb,
            gb,
            tb,
            pb,
            eb,
            zb,
            yb
        }
    }
}
