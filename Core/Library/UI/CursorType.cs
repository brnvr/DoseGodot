using System;
using Dose.Core.Library.Numbers;
using Godot;

namespace Dose.Core.Library.UI
{
    public sealed class CursorType(Texture2D texture, Vector2 origin, ScaleFactor scale)
    {
        public Texture2D Texture { get; } = texture ?? throw new ArgumentNullException(nameof(texture));
        public Vector2 Origin { get; } = origin;
        public ScaleFactor Scale { get; } = scale;
    }
}
