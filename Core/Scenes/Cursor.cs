using Godot;
using System;

namespace Dose.Core.Scenes
{
    public partial class Cursor : Control
    {
         public override void _Ready()
        {
            // Ensure it covers the full screen
            MouseFilter = MouseFilterEnum.Ignore;
            Size = GetViewportRect().Size;
        }

        public override void _Draw()
        {
            Vector2 center = Size / 2f;
            float radius = 3f; // size of the dot in pixels
            DrawCircle(center, radius, Colors.White);
        }

        public override void _Process(double delta)
        {
            // If the window is resized, redraw and update size
            Size = GetViewportRect().Size;
            QueueRedraw();
        }           
    }
}
