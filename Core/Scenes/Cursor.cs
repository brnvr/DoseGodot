using Godot;
using Dose.Core.Library.UI;

namespace Dose.Core.Scenes
{
    public partial class Cursor : Control
    {
        public static class Types
        {
            public static CursorType Dot { get; } = new(
                GD.Load<Texture2D>("res://Media/Textures/Cursors/Dot.png"),
                Vector2.One * 0.5f,
                1f
            );

            public static CursorType Arrow { get; } = new(
                GD.Load<Texture2D>("res://Media/Textures/Cursors/Arrow.png"),
                Vector2.Zero,
                1f
            );
        }

        public CursorType? Type { get; private set; }

        public override void _Ready()
        {
            MouseFilter = MouseFilterEnum.Ignore;
            TextureFilter = TextureFilterEnum.Nearest;
            Size = GetViewportRect().Size;
            ZIndex = 1000;
        }

        public void SetCursorType(CursorType? type)
        {
            Type = type;
            Visible = type != null;
            QueueRedraw();
        }

        public override void _Draw()
        {
            if (Type == null)
            {
                return;
            }

            var scale = Type.Scale.Value;
            var cursorPosition = Input.MouseMode == Input.MouseModeEnum.Captured
                ? Size * UIScreenLayout.GameplayCenterRatio
                : GetViewport().GetMousePosition();
            var destination = new Rect2(
                cursorPosition - Type.Origin * Type.Texture.GetSize() * scale,
                Type.Texture.GetSize() * scale
            );

            DrawTextureRect(Type.Texture, destination, false);
        }

        public override void _Process(double delta)
        {
            Size = GetViewportRect().Size;
            QueueRedraw();
        }
    }
}
