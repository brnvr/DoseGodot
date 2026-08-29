using Godot;

namespace Dose.Core.Scenes
{
    public partial class MapDisplay : Control
    {
        TextureRect _map = null!;

        public override void _Ready()
        {
            ClipContents = true;
            _map = new TextureRect
            {
                ExpandMode = TextureRect.ExpandModeEnum.KeepSize,
                StretchMode = TextureRect.StretchModeEnum.Keep,
                TextureFilter = TextureFilterEnum.Nearest,
                MouseFilter = MouseFilterEnum.Ignore
            };
            AddChild(_map);

            var borderStyle = new StyleBoxFlat
            {
                BgColor = Colors.Transparent,
                BorderColor = new Color("d8d2c4"),
                BorderWidthLeft = 2,
                BorderWidthTop = 2,
                BorderWidthRight = 2,
                BorderWidthBottom = 2
            };
            var border = new Panel
            {
                AnchorRight = 1f,
                AnchorBottom = 1f,
                MouseFilter = MouseFilterEnum.Ignore
            };
            border.AddThemeStyleboxOverride("panel", borderStyle);
            AddChild(border);

            AddChild(CreateMarker(new Color("1b1820"), 7));
            AddChild(CreateMarker(new Color("fff4d2"), 5));
        }

        ColorRect CreateMarker(Color color, float size) => new()
        {
            Color = color,
            Position = Size / 2f - Vector2.One * size / 2f,
            Size = Vector2.One * size,
            MouseFilter = MouseFilterEnum.Ignore
        };

        public void Display(Image image) => _map.Texture = ImageTexture.CreateFromImage(image);

        public void CenterOn(Vector2 imagePosition) => _map.Position = Size / 2f - imagePosition;
    }
}
