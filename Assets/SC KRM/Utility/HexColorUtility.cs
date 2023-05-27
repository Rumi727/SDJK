using System;
using UnityEngine;

namespace SCKRM
{
    public static class HexColorUtility
    {
        public static string ToHex(this Color color) => ToHex((Color32)color);

        public static string ToHex(this Color32 color) => "#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2") + color.a.ToString("X2");


        public static bool TryHexToColor(this string hex, out Color result)
        {
            bool success = TryHexToColor(hex, out Color32 result2);

            result = result2;
            return success;
        }

        public static bool TryHexToColor(this string hex, out Color32 result)
        {
            if (hex[0] == '#')
            {
                if (hex.Length == 9)
                {
                    result = new Color32(Convert.ToByte(hex.Substring(1, 2), 16), Convert.ToByte(hex.Substring(3, 2), 16), Convert.ToByte(hex.Substring(5, 2), 16), Convert.ToByte(hex.Substring(7, 2), 16));
                    return true;
                }
                else if (hex.Length == 7)
                {
                    result = new Color32(Convert.ToByte(hex.Substring(1, 2), 16), Convert.ToByte(hex.Substring(3, 2), 16), Convert.ToByte(hex.Substring(5, 2), 16), 255);
                    return true;
                }
                else if (hex.Length == 5)
                {
                    string text = char.ToString(hex[1]);
                    string text2 = char.ToString(hex[2]);
                    string text3 = char.ToString(hex[3]);
                    string text4 = char.ToString(hex[4]);

                    result = new Color32(Convert.ToByte(text + text, 16), Convert.ToByte(text2 + text2, 16), Convert.ToByte(text3 + text3, 16), Convert.ToByte(text4 + text4, 16));
                    return true;
                }
                else if (hex.Length == 4)
                {
                    string text = char.ToString(hex[1]);
                    string text2 = char.ToString(hex[2]);
                    string text3 = char.ToString(hex[3]);

                    result = new Color32(Convert.ToByte(text + text, 16), Convert.ToByte(text2 + text2, 16), Convert.ToByte(text3 + text3, 16), 255);
                    return true;
                }
            }

            result = Color.clear;
            return false;
        }
    }
}
