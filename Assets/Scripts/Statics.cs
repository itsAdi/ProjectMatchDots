using System;
using UnityEngine.UIElements;

namespace KemothStudios
{
    public static class Statics
    {
        // Statics for CommonUICSS
        /// <summary>
        /// Sets <b>Opacity</b> of <b>VisualElement</b> to <b>0</b> in <b>2 seconds</b> 
        /// </summary>
        public static string COMMON_CSS_HIDE_LONG = "hideLong";
        /// <summary>
        /// Sets <b>Opacity</b> of <b>VisualElement</b> to <b>0</b> in <b>1 second</b> 
        /// </summary>
        public static string COMMON_CSS_HIDE_MEDIUM = "hideMedium";
        /// <summary>
        /// Sets <b>Opacity</b> of <b>VisualElement</b> to <b>0</b> in <b>0.5 second</b> 
        /// </summary>
        public static string COMMON_CSS_HIDE_SHORT = "hideShort";
        /// <summary>
        /// Sets <b>Opacity</b> of <b>VisualElement</b> to <b>1</b> in <b>2 seconds</b> 
        /// </summary>
        public static string COMMON_CSS_SHOW_LONG = "showLong";
        /// <summary>
        /// Sets <b>Opacity</b> of <b>VisualElement</b> to <b>1</b> in <b>1 second</b> 
        /// </summary>
        public static string COMMON_CSS_SHOW_MEDIUM = "showMedium";
        /// <summary>
        /// Sets <b>Opacity</b> of <b>VisualElement</b> to <b>1</b> in <b>0.5 second</b> 
        /// </summary>
        public static string COMMON_CSS_SHOW_SHORT = "showShort";

        public static void Assert(Func<bool> condition, string message)
        {
            if(condition()) return;
            throw new NullReferenceException(message);
        }
        
        public static VisualElement GetVisualElement(this VisualElement parent, string name, string failureMessage)
        {
            VisualElement result = parent.Q(name);
            Assert(() => result != null, failureMessage);
            return result;
        }
        
        public static T GetVisualElement<T>(this VisualElement parent, string failureMessage) where T : VisualElement
        {
            T result = parent.Q<T>();
            Assert(() => result != null, failureMessage);
            return result;
        }
    }
}