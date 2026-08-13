using Godot;

namespace Dose.Core.Library.UI
{
    public static class UIScreenLayout
    {
        public const float RightBarWidthRatio = 0.15f;
        public const float BottomBarHeightRatio = 0.12f;

        public const float GameplayRightRatio = 1f - RightBarWidthRatio;
        public const float GameplayBottomRatio = 1f - BottomBarHeightRatio;

        public static Vector2 GameplayCenterRatio => new(
            GameplayRightRatio * 0.5f,
            GameplayBottomRatio * 0.5f
        );
    }
}
