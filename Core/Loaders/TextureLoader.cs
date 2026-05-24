using System;
using Dose.Core.Library.Common;
using Godot;

namespace Dose.Core.Loaders
{
    public class TextureLoader : ILoader<Texture2D?, TextureLoadArgs>
    {
        public Texture2D? Load(TextureLoadArgs loadArgs)
        {
            try
            {
                return GD.Load<Texture2D>(loadArgs.Path);
            }
            catch (Exception e)
            {
                GD.PushError($"Can't load texture {loadArgs.Path}.\n{e.Message}");
                return null;
            }
        }
    }
}