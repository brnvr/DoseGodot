using Godot;

namespace Dose.Core.Library.UI
{
    public static class UIScreenLayout
    {
        public const float FrameBarThickness = 20f;
        public const float RightBarWidthRatio = 0.12f;
        public const float BottomBarHeightRatio = 0.09f;

        static float ViewportWidth => Mathf.Max(
            ProjectSettings.GetSetting("display/window/size/viewport_width").AsInt32(),
            1
        );

        static float ViewportHeight => Mathf.Max(
            ProjectSettings.GetSetting("display/window/size/viewport_height").AsInt32(),
            1
        );

        public static float LeftBarWidthRatio => FrameBarThickness / ViewportWidth;
        public static float TopBarHeightRatio => FrameBarThickness / ViewportHeight;

        public static float GameplayLeftRatio => LeftBarWidthRatio;
        public static float GameplayTopRatio => TopBarHeightRatio;
        public const float GameplayRightRatio = 1f - RightBarWidthRatio;
        public const float GameplayBottomRatio = 1f - BottomBarHeightRatio;

        public static Vector2 GameplayCenterRatio => new(
            (GameplayLeftRatio + GameplayRightRatio) * 0.5f,
            (GameplayTopRatio + GameplayBottomRatio) * 0.5f
        );
    }
}
