using UnityEngine;
using System;

#if UNITY_EDITOR
namespace SCKRM.CustomProperty
{
    using UnityEditor;

    [WikiIgnore]
    [CustomPropertyDrawer(typeof(SetNameAttribute), true)]
    public sealed class SetNameAttributeDrawer : PropertyDrawer
    {
        //public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUI.GetPropertyHeight(property, label, true);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label.text = ((SetNameAttribute)attribute).label;
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    [WikiIgnore]
    [CustomPropertyDrawer(typeof(NotNullAttribute), true)]
    public sealed class NotNullAttributeDrawer : PropertyDrawer
    {
        //public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUI.GetPropertyHeight(property, label, true);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue == null)
            {
                position.width *= 0.6666666666666666666666666666666666666f;
                position.width -= 4;

                EditorGUI.PropertyField(position, property, label, true);

                position.x += position.width + 4;
                position.width *= 0.5f;
                position.width += 2;

                EditorGUI.HelpBox(position, "이 필드는 null 값일수 없습니다", MessageType.Error);
            }
            else
                EditorGUI.PropertyField(position, property, label, true);
        }
    }
}
#endif

namespace SCKRM
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class SetNameAttribute : PropertyAttribute
    {
        public readonly string label;

        public SetNameAttribute(string label) => this.label = label;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class NotNullAttribute : PropertyAttribute
    {

    }

    [WikiDescription("이 어트리뷰트를 추가하면 위키에 추가하지 않습니다")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Method, AllowMultiple = true)]
    public class WikiIgnoreAttribute : Attribute
    {
        public WikiIgnoreAttribute() { }
    }

    [WikiDescription("이 어트리뷰트에 문자열을 입력하면 위키에 설명을 표시합니다")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Method, AllowMultiple = true)]
    public class WikiDescriptionAttribute : Attribute
    {
        string _description;
        public string description => _description.Contains("\r") ? _description.Replace("\r", "  \r") : _description.Replace("\n", "  \n");

        public WikiDescriptionAttribute(string description) => _description = description;
    }
}