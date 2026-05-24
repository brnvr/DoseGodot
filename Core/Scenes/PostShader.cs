using System.Collections.Generic;
using Dose.Core.Library;
using Godot;

namespace Dose.Core.Scenes
{
    public partial class PostShader : Node
    {
        public PostShader Initialize(List<Color> palette)
        {
            var colorRect = GetNode<ColorRect>(
                "CanvasLayer/ColorRect"
            );

            var shaderMaterial = (ShaderMaterial)colorRect.Material;

            var paletteTexture = GodotUtils.CreatePaletteTexture(palette);

            shaderMaterial.SetShaderParameter(
                "palette_texture",
                paletteTexture
            );

            shaderMaterial.SetShaderParameter(
                "palette_size",
                palette.Count
            );

            return this;
        }
    }
}
