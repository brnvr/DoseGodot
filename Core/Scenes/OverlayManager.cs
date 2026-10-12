using Godot;

namespace Dose.Core.Scenes
{
    public partial class OverlayManager : CanvasLayer
    {
        public ScreenOverlay AddOverlay(
            VideoStream stream,
            OverlayBlendMode blendMode = OverlayBlendMode.Normal,
            bool loop = true,
            bool autoplay = false,
            float opacity = 1f,
            float blackThreshold = 0.05f,
            float blackSoftness = 0.2f
        )
        {
            var overlay = new ScreenOverlay
            {
                Name = "Overlay",
                AnchorRight = 1f,
                AnchorBottom = 1f,
                Expand = true,
                VolumeDb = -80f,
                Autoplay = autoplay
            }.Configure(
                stream,
                blendMode,
                loop,
                opacity,
                blackThreshold,
                blackSoftness
            );

            AddChild(overlay);
            return overlay;
        }

        public ScreenOverlay? AddOverlay(
            string resourcePath,
            OverlayBlendMode blendMode = OverlayBlendMode.Normal,
            bool loop = true,
            bool autoplay = false,
            float opacity = 1f,
            float blackThreshold = 0.05f,
            float blackSoftness = 0.2f
        )
        {
            var stream = GD.Load<VideoStream>(resourcePath);
            if (stream == null)
            {
                GD.PushError($"Could not load overlay video '{resourcePath}'.");
                return null;
            }

            return AddOverlay(
                stream,
                blendMode,
                loop,
                autoplay,
                opacity,
                blackThreshold,
                blackSoftness
            );
        }

        public void RemoveOverlay(ScreenOverlay overlay)
        {
            if (!IsInstanceValid(overlay) || overlay.GetParent() != this)
            {
                return;
            }

            overlay.Stop();
            overlay.QueueFree();
        }

        public void Clear()
        {
            foreach (var child in GetChildren())
            {
                if (child is ScreenOverlay overlay)
                {
                    RemoveOverlay(overlay);
                }
            }
        }
    }
}
