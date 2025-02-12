using KemothStudios.Utility.Events;
using UnityEngine;
using UnityEngine.UIElements;

namespace KemothStudios
{
    public class UIMessagePopup : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;

        private EventBinding<ShowMessageEvent> _messageBinding;
        private EventBinding<HideMessageEvent> _hideBinding;
        private VisualElement _popup;
        private Label _messageText;
        private Button _hidePopupButton;
        
        private bool _isShowing;
        private bool _isInitialized;
        private HideMessageEvent _hideEvent;
        
        private void Start()
        {
            _messageBinding = new EventBinding<ShowMessageEvent>(ShowMessage);
            _hideBinding = new EventBinding<HideMessageEvent>(HideMessage);
            EventBus<ShowMessageEvent>.RegisterBinding(_messageBinding);
            EventBus<HideMessageEvent>.RegisterBinding(_hideBinding);
            Statics.Assert(()=>_uiDocument != null, "UI Document for message popup is null");
            _popup = _uiDocument.rootVisualElement.LookFor("windowContainer", "Message popup window container not found");
            _messageText = _popup.LookFor<Label>("messageText", "Text object for message popup not found");
            _hidePopupButton = _popup.LookFor("responseButton", "Response button container not found").LookFor<Button>("Button object for response button not found");
            _popup.AddToClassList("show");
            _popup.AddToClassList("hide");
            _uiDocument.rootVisualElement.pickingMode = PickingMode.Ignore;
            _hidePopupButton.clicked += HidePopupWindow;
            _isInitialized = true;
        }

        private void OnDestroy()
        {
            if(!_isInitialized) return;
            EventBus<ShowMessageEvent>.UnregisterBinding(_messageBinding);
            EventBus<HideMessageEvent>.UnregisterBinding(_hideBinding);
            _hidePopupButton.clicked -= HidePopupWindow;
        }

        private void HidePopupWindow() => EventBus<HideMessageEvent>.RaiseEvent(_hideEvent);
        
        private void ShowMessage(ShowMessageEvent messageData)
        {
            if(!_isInitialized || _isShowing) return;
            _isShowing = true;
            _messageText.text = messageData.Message;
            _uiDocument.rootVisualElement.pickingMode = PickingMode.Position;
            _popup.RemoveFromClassList("hide");
        }

        private void HideMessage()
        {
            if(!_isInitialized || !_isShowing) return;
            _isShowing = false;
            _uiDocument.rootVisualElement.pickingMode = PickingMode.Ignore;
            _popup.AddToClassList("hide");
        }
        
        
    }
}