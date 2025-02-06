using KemothStudios.Utility.Events;
using UnityEngine;
using UnityEngine.UIElements;

namespace KemothStudios
{
    public class UILoadingScreen : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        
        private VisualElement _loadingScreenVisualElement;
        private EventBinding<ShowLoadingScreenEvent> _showLoadingScreenBinding;
        private EventBinding<HideLoadingScreenEvent> _hideLoadingScreenBinding;

        private bool _isInitialized;
        
        private void Start()
        {
            Statics.Assert(()=>_uiDocument != null, "UI Document for loading screen is null");
            _loadingScreenVisualElement = _uiDocument.rootVisualElement.GetVisualElement("background", "Background object for loading screen not found");
            _loadingScreenVisualElement.AddToClassList(Statics.COMMON_CSS_HIDE_LONG);
            _loadingScreenVisualElement.AddToClassList(Statics.COMMON_CSS_SHOW_LONG);
            _showLoadingScreenBinding = new EventBinding<ShowLoadingScreenEvent>(ShowLoadingScreen);
            _hideLoadingScreenBinding = new EventBinding<HideLoadingScreenEvent>(HideLoadingScreen);
            EventBus<ShowLoadingScreenEvent>.RegisterBinding(_showLoadingScreenBinding);
            EventBus<HideLoadingScreenEvent>.RegisterBinding(_hideLoadingScreenBinding);
            _isInitialized = true;
        }

        private void OnDestroy()
        {
            if(!_isInitialized) return;
            EventBus<ShowLoadingScreenEvent>.UnregisterBinding(_showLoadingScreenBinding);
            EventBus<HideLoadingScreenEvent>.UnregisterBinding(_hideLoadingScreenBinding);
        }

        private void ShowLoadingScreen()
        {
            _loadingScreenVisualElement.RegisterCallbackOnce<TransitionEndEvent>(_ =>
            {
                _loadingScreenVisualElement.pickingMode = PickingMode.Position;
                EventBus<LoadingScreenTransitionCompleteEvent>.RaiseEvent(new LoadingScreenTransitionCompleteEvent());
            });
            _loadingScreenVisualElement.AddToClassList(Statics.COMMON_CSS_SHOW_LONG);
        }

        private void HideLoadingScreen()
        {
            _loadingScreenVisualElement.RegisterCallbackOnce<TransitionEndEvent>(_ =>
            {
                _loadingScreenVisualElement.pickingMode = PickingMode.Ignore;
                EventBus<LoadingScreenTransitionCompleteEvent>.RaiseEvent(new LoadingScreenTransitionCompleteEvent());
            });
            _loadingScreenVisualElement.RemoveFromClassList(Statics.COMMON_CSS_SHOW_LONG);
        }
    }
}