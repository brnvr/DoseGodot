using Godot;

namespace Dose.Core.Scenes
{
    public enum OverlayBlendMode
    {
        Normal,
        Merge
    }

    public partial class ScreenOverlay : VideoStreamPlayer
    {
        const string OverlayShaderPath = "res://Core/Shaders/OverlayMerge.gdshader";

        OverlayBlendMode _blendMode = OverlayBlendMode.Normal;
        float _opacity = 1f;
        float _blackThreshold = 0.05f;
        float _blackSoftness = 0.2f;

        [Export]
        public OverlayBlendMode BlendMode
        {
            get => _blendMode;
            set
            {
                _blendMode = value;
                UpdateMaterial();
            }
        }

        [Export(PropertyHint.Range, "0,1,0.01")]
        public float Opacity
        {
            get => _opacity;
            set
            {
                _opacity = Mathf.Clamp(value, 0f, 1f);
                UpdateMaterial();
            }
        }

        [Export(PropertyHint.Range, "0,1,0.01")]
        public float BlackThreshold
        {
            get => _blackThreshold;
            set
            {
                _blackThreshold = Mathf.Clamp(value, 0f, 1f);
                UpdateMaterial();
            }
        }

        [Export(PropertyHint.Range, "0.001,1,0.001")]
        public float BlackSoftness
        {
            get => _blackSoftness;
            set
            {
                _blackSoftness = Mathf.Clamp(value, 0.001f, 1f);
                UpdateMaterial();
            }
        }

        public override void _Ready()
        {
            MouseFilter = MouseFilterEnum.Ignore;
            UpdateMaterial();
        }

        public ScreenOverlay Configure(
            VideoStream stream,
            OverlayBlendMode blendMode = OverlayBlendMode.Normal,
            bool loop = true,
            float opacity = 1f,
            float blackThreshold = 0.05f,
            float blackSoftness = 0.2f
        )
        {
            Stream = stream;
            Loop = loop;
            _blendMode = blendMode;
            _opacity = Mathf.Clamp(opacity, 0f, 1f);
            _blackThreshold = Mathf.Clamp(blackThreshold, 0f, 1f);
            _blackSoftness = Mathf.Clamp(blackSoftness, 0.001f, 1f);
            UpdateMaterial();
            return this;
        }

        void UpdateMaterial()
        {
            Modulate = Colors.White;
            if (Material is not ShaderMaterial material)
            {
                material = new ShaderMaterial
                {
                    Shader = GD.Load<Shader>(OverlayShaderPath)
                };
                Material = material;
            }

            material.SetShaderParameter("opacity", _opacity);
            material.SetShaderParameter(
                "merge_black",
                _blendMode == OverlayBlendMode.Merge
            );
            material.SetShaderParameter("black_threshold", _blackThreshold);
            material.SetShaderParameter("black_softness", _blackSoftness);
        }
    }
}
