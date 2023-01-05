using Microsoft.JScript;
using Newtonsoft.Json;
using SCKRM.Resource;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SCKRM.Json
{
    public static class JsonManager
    {
        /// <summary>
        /// 텍스트 파일에서 Json을 읽고 반환합니다
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">
        /// 텍스트 파일 경로
        /// </param>
        /// <param name="pathExtensionUse">
        /// 경로에 확장자 사용
        /// </param>
        /// <returns></returns>
        [WikiDescription("텍스트 파일에서 Json을 읽고 반환합니다")]
        public static T JsonRead<T>(string path, bool pathExtensionUse = false)
        {
            string json;
            json = ResourceManager.GetText(path, pathExtensionUse);

            if (json != "")
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(json);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Debug.LogError("Path: " + path);

                    return default;
                }
            }
            else
                return default;
        }

        /// <summary>
        /// 리소스팩에서 딕셔너리로 된 Json을 찾고 반환합니다
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key">
        /// 키
        /// </param>
        /// <param name="path">
        /// 텍스트 파일의 경로
        /// </param>
        /// <param name="nameSpace">
        /// 네임스페이스
        /// </param>
        /// <returns></returns>
        [WikiDescription("리소스팩에서 딕셔너리로 된 Json을 찾고 반환합니다")]
        public static TValue JsonReadDictionary<TKey, TValue>(TKey key, string path, string nameSpace)
        {
            if (path == null)
                path = "";
            if (nameSpace == null)
                nameSpace = "";

            if (nameSpace == "")
                nameSpace = ResourceManager.defaultNameSpace;

            path = path.Replace("%NameSpace%", nameSpace);

            for (int i = 0; i < ResourceManager.SaveData.resourcePacks.Count; i++)
            {
                string resourcePack = ResourceManager.SaveData.resourcePacks[i];
                Dictionary<TKey, TValue> dictionary = JsonRead<Dictionary<TKey, TValue>>(PathTool.Combine(resourcePack, path), true);
                if (dictionary != null && dictionary.ContainsKey(key))
                    return dictionary[key];
            }

            return default;
        }

        [WikiDescription("Json을 오브젝트로 변환합니다")] public static T JsonToObject<T>(string json) => JsonConvert.DeserializeObject<T>(json);
        [WikiDescription("오브젝트를 Json으로 변환합니다")] public static string ObjectToJson(object value) => JsonConvert.SerializeObject(value, Formatting.Indented);
        [WikiIgnore] public static string ObjectToJson(params object[] value) => JsonConvert.SerializeObject(value, Formatting.Indented);
    }

    public struct JVector2 : IEquatable<JVector2>
    {
        public float x;
        public float y;

        public static JVector2 zero { get; } = new JVector2();
        public static JVector2 one { get; } = new JVector2(1);

        public JVector2(Vector2 value) : this(value.x, value.y)
        {

        }

        public JVector2(float value) => x = y = value;
        public JVector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static implicit operator JVector3(JVector2 value) => new JVector3(value.x, value.y);
        public static implicit operator JVector4(JVector2 value) => new JVector4(value.x, value.y);

        public static implicit operator JVector2(JRect value) => new JVector2(value.x, value.y);

        public static implicit operator JVector2(Vector2 value) => new JVector2(value);
        public static implicit operator JVector2(Vector3 value) => new JVector2(value);
        public static implicit operator JVector2(Vector4 value) => new JVector2(value);
        public static implicit operator Vector2(JVector2 value) => new Vector3(value.x, value.y);
        public static implicit operator Vector3(JVector2 value) => new Vector3(value.x, value.y);
        public static implicit operator Vector4(JVector2 value) => new Vector4(value.x, value.y);

        public override string ToString() => $"(x: {x}, y: {y})";
        public bool Equals(JVector2 other) => x == other.x && y == other.y;
    }

    public struct JVector3 : IEquatable<JVector3>
    {
        public float x;
        public float y;
        public float z;

        public static JVector3 zero { get; } = new JVector3();
        public static JVector3 one { get; } = new JVector3(1);

        public JVector3(Vector3 value) : this(value.x, value.y, value.z)
        {

        }

        public JVector3(float value) => x = y = z = value;

        public JVector3(float x, float y) : this(x, y, 0)
        {

        }

        public JVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static explicit operator JVector2(JVector3 value) => new JVector2(value.x, value.y);
        public static implicit operator JVector4(JVector3 value) => new JVector4(value.x, value.y, value.z);

        public static implicit operator JVector3(JRect value) => new JVector3(value.x, value.y, value.width);

        public static implicit operator JVector3(Vector2 value) => new JVector3(value);
        public static implicit operator JVector3(Vector3 value) => new JVector3(value);
        public static implicit operator JVector3(Vector4 value) => new JVector3(value);
        public static explicit operator Vector2(JVector3 value) => new Vector3(value.x, value.y);
        public static implicit operator Vector3(JVector3 value) => new Vector3(value.x, value.y, value.z);
        public static implicit operator Vector4(JVector3 value) => new Vector4(value.x, value.y, value.z);

        public override string ToString() => $"({x}, {y}, {z})";
        public bool Equals(JVector3 other) => x == other.x && y == other.y && z == other.z;
    }
    public struct JVector4 : IEquatable<JVector4>
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public static JVector4 zero { get; } = new JVector4();
        public static JVector4 one { get; } = new JVector4(1);

        public JVector4(Vector4 value) : this(value.x, value.y, value.z, value.w)
        {

        }

        public JVector4(float value) => x = y = z = w = value;

        public JVector4(float x, float y) : this(x, y, 0, 0)
        {

        }

        public JVector4(float x, float y, float z) : this(x, y, z, 0)
        {

        }

        public JVector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static explicit operator JVector2(JVector4 value) => new JVector2(value.x, value.y);
        public static explicit operator JVector3(JVector4 value) => new JVector3(value.x, value.y, value.z);

        public static implicit operator JVector4(JRect value) => new JVector4(value.x, value.y, value.width, value.height);
        public static implicit operator JVector4(Rect value) => new JVector4(value.x, value.y, value.width, value.height);

        public static implicit operator JVector4(Vector2 value) => new JVector4(value);
        public static implicit operator JVector4(Vector3 value) => new JVector4(value);
        public static implicit operator JVector4(Vector4 value) => new JVector4(value);
        public static explicit operator Vector2(JVector4 value) => new Vector2(value.x, value.y);
        public static explicit operator Vector3(JVector4 value) => new Vector3(value.x, value.y, value.z);
        public static implicit operator Vector4(JVector4 value) => new Vector4(value.x, value.y, value.z, value.w);

        public override string ToString() => $"({x}, {y}, {z}, {w})";
        public bool Equals(JVector4 other) => x == other.x && y == other.y && z == other.z && w == other.w;
    }

    public struct JRect : IEquatable<JRect>
    {
        public float x;
        public float y;
        public float width;
        public float height;

        public static JRect zero { get; } = new JRect();
        public static JRect one { get; } = new JRect(1);

        public JRect(Rect value) : this(value.x, value.y, value.width, value.height)
        {

        }

        public JRect(float value) => x = y = width = height = value;

        public JRect(float x, float y) : this(x, y, 0, 0)
        {

        }

        public JRect(float x, float y, float width) : this(x, y, width, 0)
        {

        }

        public JRect(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public static explicit operator JRect(JVector2 value) => new JRect(value.x, value.y);
        public static explicit operator JRect(JVector3 value) => new JRect(value.x, value.y, value.z);
        public static implicit operator JRect(JVector4 value) => new JRect(value.x, value.y, value.z, value.w);
        public static implicit operator JRect(JColor value) => new JRect(value.r, value.g, value.b, value.a);

        public static explicit operator JRect(Vector2 value) => new JRect(value.x, value.y);
        public static explicit operator JRect(Vector3 value) => new JRect(value.x, value.y, value.z);
        public static implicit operator JRect(Vector4 value) => new JRect(value.x, value.y, value.z, value.w);
        public static implicit operator JRect(Color value) => new JRect(value.r, value.g, value.b, value.a);

        public static explicit operator Vector2(JRect value) => new Vector2(value.x, value.y);
        public static explicit operator Vector3(JRect value) => new Vector3(value.x, value.y, value.width);
        public static implicit operator Vector4(JRect value) => new Vector4(value.x, value.y, value.width, value.height);
        public static implicit operator Color(JRect value) => new Color(value.x, value.y, value.width, value.height);

        public static implicit operator JRect(Rect value) => new JRect(value);
        public static implicit operator Rect(JRect value) => new Rect() { x = value.x, y = value.y, width = value.width, height = value.height };

        public override string ToString() => $"(x:{x}, y:{y}, width:{width}, height:{height})";
        public bool Equals(JRect other) => x == other.x && y == other.y && width == other.width && height == other.height;
    }

    public struct JColor : IEquatable<JColor>
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public static JColor zero { get; } = new JColor();
        public static JColor one { get; } = new JColor(1);

        public JColor(Color value) : this(value.r, value.g, value.b, value.a)
        {

        }

        public JColor(Color32 value) : this(value.r / 255f, value.g / 255f, value.b / 255f, value.a / 255f)
        {

        }

        public JColor(JColor32 value) : this(value.r / 255f, value.g / 255f, value.b / 255f, value.a / 255f)
        {

        }

        public JColor(float value) => r = g = b = a = value;

        public JColor(float r, float g, float b) : this(r, g, b, 1)
        {

        }

        public JColor(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public static explicit operator JColor(Vector3 value) => new JColor(value.x, value.y, value.z);
        public static implicit operator JColor(Vector4 value) => new JColor(value.x, value.y, value.z, value.w);
        public static implicit operator JColor(Rect value) => new JColor(value.x, value.y, value.width, value.height);

        public static explicit operator JColor(JVector3 value) => new JColor(value.x, value.y, value.z);
        public static implicit operator JColor(JVector4 value) => new JColor(value.x, value.y, value.z, value.w);
        public static implicit operator JColor(JRect value) => new JColor(value.x, value.y, value.width, value.height);

        public static explicit operator Vector3(JColor value) => new Vector3(value.r, value.g, value.b);
        public static implicit operator Vector4(JColor value) => new Vector4(value.r, value.g, value.b, value.a);
        public static implicit operator Rect(JColor value) => new Rect(value.r, value.g, value.b, value.a);

        public static implicit operator JColor(Color32 value) => new JColor(value);
        public static implicit operator Color32(JColor value) => new Color32((byte)(value.r.Clamp01() * 255).Round(), (byte)(value.g.Clamp01() * 255).Round(), (byte)(value.b.Clamp01() * 255).Round(), (byte)(value.a.Clamp01() * 255).Round());

        public static implicit operator JColor(Color value) => new JColor(value);
        public static implicit operator Color(JColor value) => new Color(value.r, value.g, value.b, value.a);

        public override string ToString() => $"(r:{r}, g:{g}, b:{b}, a:{a})";
        public bool Equals(JColor other) => r == other.r && g == other.g && b == other.b && a == other.a;
    }

    public struct JColor32 : IEquatable<JColor32>
    {
        public byte r;
        public byte g;
        public byte b;
        public byte a;

        public static JColor32 zero { get; } = new JColor32();
        public static JColor32 one { get; } = new JColor32(1);

        public JColor32(Color value) : this((byte)(value.r.Clamp01() * 255).Round(), (byte)(value.g.Clamp01() * 255).Round(), (byte)(value.b.Clamp01() * 255).Round(), (byte)(value.a.Clamp01() * 255).Round())
        {

        }

        public JColor32(JColor value) : this((byte)(value.r.Clamp01() * 255).Round(), (byte)(value.g.Clamp01() * 255).Round(), (byte)(value.b.Clamp01() * 255).Round(), (byte)(value.a.Clamp01() * 255).Round())
        {

        }

        public JColor32(Color32 value) : this(value.r, value.g, value.b, value.a)
        {

        }

        public JColor32(byte value) => r = g = b = a = value;

        public JColor32(byte r, byte g, byte b) : this(r, g, b, 255)
        {

        }

        public JColor32(byte r, byte g, byte b, byte a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public static explicit operator JColor32(Vector3 value) => new JColor32((byte)value.x, (byte)value.y, (byte)value.z);
        public static implicit operator JColor32(Vector4 value) => new JColor32((byte)value.x, (byte)value.y, (byte)value.z, (byte)value.w);
        public static implicit operator JColor32(Rect value) => new JColor32((byte)value.x, (byte)value.y, (byte)value.width, (byte)value.height);

        public static explicit operator JColor32(JVector3 value) => new JColor32((byte)value.x, (byte)value.y, (byte)value.z);
        public static implicit operator JColor32(JVector4 value) => new JColor32((byte)value.x, (byte)value.y, (byte)value.z, (byte)value.w);
        public static implicit operator JColor32(JRect value) => new JColor32((byte)value.x, (byte)value.y, (byte)value.width, (byte)value.height);

        public static explicit operator Vector3(JColor32 value) => new Vector3(value.r, value.g, value.b);
        public static implicit operator Vector4(JColor32 value) => new Vector4(value.r, value.g, value.b, value.a);
        public static implicit operator Rect(JColor32 value) => new Rect(value.r, value.g, value.b, value.a);

        public static implicit operator JColor32(Color32 value) => new JColor32(value);
        public static implicit operator Color32(JColor32 value) => new Color32(value.r, value.g, value.b, value.a);

        public static implicit operator JColor32(Color value) => new JColor32(value);
        public static implicit operator Color(JColor32 value) => new Color(value.r / 255f, value.g / 255f, value.b / 255f, value.a / 255f);


        public static implicit operator JColor32(JColor value) => new JColor32(value);
        public static implicit operator JColor(JColor32 value) => new JColor(value);

        public override string ToString() => $"(r:{r}, g:{g}, b:{b}, a:{a})";
        public bool Equals(JColor32 other) => r == other.r && g == other.g && b == other.b && a == other.a;
    }
}