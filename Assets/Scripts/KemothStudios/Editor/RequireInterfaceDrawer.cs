using UnityEditor;
using UnityEngine;

namespace KemothStudios.Utility.Attributes
{
    [CustomPropertyDrawer(typeof(RequireInterfaceAttribute))]
    public class RequireInterfaceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                RequireInterfaceAttribute attr = attribute as RequireInterfaceAttribute;
                EditorGUI.BeginProperty(position, label, property);
                property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, attr.InterfaceType, true);
                EditorGUI.EndProperty();
            }
            else
            {
                Color previousColor = GUI.color;
                GUI.color = Color.white;
                EditorGUI.LabelField(position, "Target property is not a reference type");
                GUI.color = previousColor;
            }
        }
    }
}