using Godot;

namespace Dose.Core.Scenes
{
    public partial class TextWithBackground : CenterContainer
    {
        const float MAX_WIDTH_RATIO = 0.7f;

        PanelContainer _background = null!;
        MarginContainer _marginContainer = null!;
        Label _label = null!;

        public override void _Ready()
        {
            _background = GetNode<PanelContainer>("Background");
            _marginContainer = GetNode<MarginContainer>("Background/MarginContainer");
            _label = GetNode<Label>("Background/MarginContainer/Text");
        }

        public void SetText(string? text)
        {
            _label.Text = text ?? "";
            UpdateWidth();
            SetTextVisible(!string.IsNullOrWhiteSpace(text));
        }

        public void SetTextVisible(bool visible)
        {
            Visible = visible && !string.IsNullOrWhiteSpace(_label.Text);
        }

        void UpdateWidth()
        {
            var font = _label.GetThemeFont("font");
            var fontSize = _label.GetThemeFontSize("font_size");
            var textWidth = font.GetStringSize(_label.Text, fontSize: fontSize).X;
            var horizontalPadding =
                _marginContainer.GetThemeConstant("margin_left") +
                _marginContainer.GetThemeConstant("margin_right");
            var maximumWidth = GetViewportRect().Size.X * MAX_WIDTH_RATIO;

            _background.CustomMinimumSize = new Vector2(
                Mathf.Ceil(Mathf.Min(textWidth + horizontalPadding, maximumWidth)),
                0
            );
        }
    }
}
