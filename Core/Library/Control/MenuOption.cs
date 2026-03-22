using System;

namespace Dose.Core.Library.Control
{
    public readonly struct MenuOption(string text, Action onPressed)
    {
        public string Text { get; } = text;
        public Action OnPressed { get; } = onPressed;
    }
}