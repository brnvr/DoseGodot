using Godot;

namespace Dose.Core.Library.Common
{
    public interface IHasTexture
    {
        public Texture2D? Texture { get; }
    }
}