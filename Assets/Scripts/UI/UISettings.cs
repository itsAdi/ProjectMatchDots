using System;
using KemothStudios.Utility;
using UnityEngine;
using UnityEngine.UIElements;
using static KemothStudios.Statics;

namespace KemothStudios
{
    [Serializable]
    public sealed class UISettings
    {
        [SerializeField] private VisualTreeAsset _settingsUIAsset;

        private bool _isInitialized;
        private VisualElement _windowBG;
        private VisualElement _window;

        public void Initialize(UIDocument settingsParent)
        {
            if (_isInitialized) return;
            try
            {
                Assert(()=>settingsParent != null,"Unable to create UI bg element");
                Assert(()=>_settingsUIAsset != null,"Settings UI asset is not assigned");
                TemplateContainer root = _settingsUIAsset.Instantiate();
                _windowBG = root.Q<VisualElement>("bg");
                Assert(() =>_windowBG != null, "Unable to create UI bg element");
                _window = root.Q<VisualElement>("settingsWindow");
                Assert(()=>_window != null,"Unable to create UI window element");
                
                // because we will instantiate and extract first child from root and move them to parent, this will not fire any event so we
                // are using scheduler to add required classes after one frame
                settingsParent.rootVisualElement.schedule.Execute(() =>
                {
                   _windowBG.AddToClassList(COMMON_CSS_HIDE_SHORT);
                   _window.AddToClassList("settingsWindowDown");
                   _windowBG.pickingMode = PickingMode.Ignore;
                }).ExecuteLater(0); // this line will call scheduler after one frame
                
                // moving child out of root will remove css links,add them to parent
                VisualElementStyleSheetSet styleSheetSet = root.styleSheets;
                int count = styleSheetSet.count;
                for (int i = 0; i < count; i++)
                {
                    settingsParent.rootVisualElement.styleSheets.Add(styleSheetSet[i]);
                }

                root.Q<VisualElement>("done").Q<Button>().clicked += HideSettings;
                
                settingsParent.rootVisualElement.Add(root.ElementAt(0));
                root.RemoveFromHierarchy();
                _isInitialized = true;
            }
            catch (Exception _)
            {
                DebugUtility.LogColored("red","Initializing SettingsUI failed !!!");
                throw;
            }
        }

        public void ShowSettings()
        {
            if (IsInitialized)
            {
                _windowBG.AddToClassList(COMMON_CSS_SHOW_SHORT);
                _window.AddToClassList("settingsWindowUp");
            }
        }
        
        public void HideSettings()
        {
            if (IsInitialized)
            {
                _windowBG.RemoveFromClassList(COMMON_CSS_SHOW_SHORT);
                _window.RemoveFromClassList("settingsWindowUp");
            }
        }

        private bool IsInitialized
        {
            get
            {
                if (!_isInitialized)
                    DebugUtility.LogColored("red", "UI settings not initialized");
                return _isInitialized;
            }
        }
    }
}