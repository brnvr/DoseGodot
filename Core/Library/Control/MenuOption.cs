using System;

namespace Dose.Core.Library.Control
{
    public readonly struct MenuOption(
        string text,
        Action? onPressed = null,
        Action? onHover = null,
        Action? onExit = null,
        Action? onButtonDown = null
    )
    {
        public string Text { get; } = text;
        public Action? OnPressed { get; } = onPressed;
        public Action? OnHover { get; } = onHover;
        public Action? OnExit { get; } = onExit;
        public Action? OnButtonDown { get; } = onButtonDown;
    }
}
