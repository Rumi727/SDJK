using UnityEngine;
using System;

#if UNITY_EDITOR
namespace SCKRM.CustomProperty
{
    using UnityEditor;
    
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
}