using Godot;

namespace Dose.Core.Scenes
{
    public partial class ScreenHls : CanvasLayer
    {
        ShaderMaterial? _material;
        float _hueRed = 1f;
        float _hueGreen = 1f;
        float _hueBlue = 1f;
        float _lightness;
        float _saturation = 1f;

        [Export(PropertyHint.Range, "0,2,0.01")]
        public float HueRed
        {
            get => _hueRed;
            set
            {
                _hueRed = Mathf.Clamp(value, 0f, 2f);
                UpdateMaterial();
            }
        }

        [Export(PropertyHint.Range, "0,2,0.01")]
        public float HueGreen
        {
            get => _hueGreen;
            set
            {
                _hueGreen = Mathf.Clamp(value, 0f, 2f);
                UpdateMaterial();
            }
        }

        [Export(PropertyHint.Range, "0,2,0.01")]
        public float HueBlue
        {
            get => _hueBlue;
            set
            {
                _hueBlue = Mathf.Clamp(value, 0f, 2f);
                UpdateMaterial();
            }
        }

        [Export(PropertyHint.Range, "-1,1,0.01")]
        public float Lightness
        {
            get => _lightness;
            set
            {
                _lightness = Mathf.Clamp(value, -1f, 1f);
                UpdateMaterial();
            }
        }

        [Export(PropertyHint.Range, "0,2,0.01")]
        public float Saturation
        {
            get => _saturation;
            set
            {
                _saturation = Mathf.Clamp(value, 0f, 2f);
                UpdateMaterial();
            }
        }

        public override void _Ready()
        {
            _material = GetNode<ColorRect>("ColorRect").Material as ShaderMaterial;
            UpdateMaterial();
        }

        void UpdateMaterial()
        {
            if (_material == null)
            {
                return;
            }

            _material.SetShaderParameter(
                "hue_rgb",
                new Vector3(_hueRed, _hueGreen, _hueBlue)
            );
            _material.SetShaderParameter("lightness", _lightness);
            _material.SetShaderParameter("saturation", _saturation);
        }
    }
}
