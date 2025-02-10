using UnityEngine;
using UnityEngine.UIElements;

namespace KemothStudios.UI
{
    [UxmlElement]
    public partial class ProgressBar : VisualElement
    {
        public enum FillDirection
        {
            RIGHT = 0,
            LEFT = 1
        }

        private FillDirection _direction = FillDirection.LEFT;
        private float _fillAmount;
        private VisualElement _progressBar;
        private bool _initialized;

        [UxmlAttribute]
        public FillDirection Direction
        {
            get => _direction;
            set
            {
                if (_direction != value)
                {
                    _direction = value;
                    UpdateProgressBar();
                }
            }
        }

        [UxmlAttribute, Range(0f, 100f)]
        public float FillAmount
        {
            get => _fillAmount;
            set
            {
                if (!Mathf.Approximately(_fillAmount, value))
                {
                    _fillAmount = value;
                    UpdateProgressBar();
                }
            }
        }

        public ProgressBar()
        {
            VisualElement container = new VisualElement
            {
                name = "container"
            };
            container.AddToClassList("unity-progress-bar__container");
            VisualElement bg = new VisualElement
            {
                name = "background"
            };
            bg.AddToClassList("unity-progress-bar__background");
            container.Add(bg);
            _progressBar = new VisualElement
            {
                name = "progressBar"
            };
            _progressBar.AddToClassList("unity-progress-bar__progress");
            bg.Add(_progressBar);
            Add(container);
            RegisterCallbackOnce<AttachToPanelEvent>(x =>
            {
                _initialized = true;
                UpdateProgressBar();
            });
        }

        private void UpdateProgressBar()
        {
            if (!_initialized) return;
            _progressBar.style.width = Length.Percent(FillAmount);
            _progressBar.style.left = _direction == FillDirection.RIGHT ? Length.Percent(100f - FillAmount) : new StyleLength(StyleKeyword.Null);
        }
    }
}