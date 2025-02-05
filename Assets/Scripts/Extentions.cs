using UnityEngine;
using UnityEngine.UIElements;

namespace KemothStudios
{
    public static class Extentions
    {
        public static Vector2 ConvertDirectionToVector(this Direction direction)
        {
            return direction switch
            {
                Direction.Up => Vector2.up,
                Direction.Down => Vector2.down,
                Direction.Left => Vector2.left,
                Direction.Right => Vector2.right,
                _ => Vector2.zero
            };
        }
        
        public static VisualElement GetVisualElement(this VisualElement parent, string name, string failureMessage)
        {
            VisualElement result = parent.Q(name);
            Statics.Assert(() => result != null, failureMessage);
            return result;
        }
        
        public static T GetVisualElement<T>(this VisualElement parent, string name, string failureMessage) where T : VisualElement
        {
            T result = parent.Q<T>(name);
            Statics.Assert(() => result != null, failureMessage);
            return result;
        }
        
        public static T GetVisualElement<T>(this VisualElement parent, string failureMessage) where T : VisualElement
        {
            T result = parent.Q<T>();
            Statics.Assert(() => result != null, failureMessage);
            return result;
        }
    }
}