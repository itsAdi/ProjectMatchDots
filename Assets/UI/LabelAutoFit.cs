using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class LabelAutoFit : VisualElement
{
    [UxmlAttribute]
    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            UpdateText();
        }
    }

    [UxmlAttribute]
    public float ratio
    {
        get => _ratio; set
        {
            _ratio = value;
            UpdateFontSize();
        }
    }

    private Label _label;
    private string _text;
    private float _ratio;
    float newRectLength;

    public LabelAutoFit()
    {
        _label = new Label();
        hierarchy.Add(_label);
        _label.RegisterValueChangedCallback(TextChanged);
        RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    void UpdateText()
    {
        if (_label != null)
            _label.text = _text;
    }

    void TextChanged(ChangeEvent<string> e) => UpdateFontSize();

    void OnGeometryChanged(GeometryChangedEvent evt)
    {
        UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        try
        {
            newRectLength = evt.newRect.width;
            UpdateFontSize();
        }
        finally
        {
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }
    }

    private void UpdateFontSize()
    {
        float oldFontSize = _label.style.fontSize.value.value;
        float newFontSize = (newRectLength / _label.text.Length) * _ratio;

        float fontSizeDelta = Mathf.Abs(oldFontSize - newFontSize);
        float fontSizeDeltaNormalized = fontSizeDelta / Mathf.Max(oldFontSize, 1);

        if (fontSizeDeltaNormalized > 0.01f)
            _label.style.fontSize = newFontSize < resolvedStyle.fontSize ? newFontSize : resolvedStyle.fontSize;
    }

}