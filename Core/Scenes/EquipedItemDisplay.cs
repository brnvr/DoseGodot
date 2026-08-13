using Godot;
using Dose.Core.Library.UI;

namespace Dose.Core.Scenes
{
    public partial class EquipedItemDisplay : Control
    {
        const float EQUIP_SCALE = 2f;
        const float EQUIP_POSITION_Y = 200f;
        const float EQUIP_OFFSET = 82f;
        const float EQUIP_DURATION = 0.3f;
        const float SWAY_ROTATION = 0.04363323f;
        const float SWAY_SIDE_DURATION = 3f;
        const float TARGET_OFFSET = -150f;
        const float TARGET_SCALE = 0.58f;
        const float TARGET_TRANSITION_DURATION = 0.2f;
        const float REJECTION_SCALE = 0.85f;
        const float REJECTION_STEP_DURATION = 0.1f;

        Control _itemAnchor = null!;
        Control _equipMotion = null!;
        Control _targetMotion = null!;
        Control _feedbackMotion = null!;
        Control _swayMotion = null!;
        TextureRect _textureRect = null!;
        Tween? _equipTween;
        Tween? _targetTween;
        Tween? _feedbackTween;
        Tween? _swayTween;

        public override void _Ready()
        {
            _itemAnchor = GetNode<Control>("ItemAnchor");
            _itemAnchor.AnchorLeft = UIScreenLayout.GameplayCenterRatio.X;
            _itemAnchor.AnchorRight = UIScreenLayout.GameplayCenterRatio.X;
            _equipMotion = GetNode<Control>("ItemAnchor/EquipMotion");
            _targetMotion = GetNode<Control>("ItemAnchor/EquipMotion/TargetMotion");
            _feedbackMotion = GetNode<Control>("ItemAnchor/EquipMotion/TargetMotion/FeedbackMotion");
            _swayMotion = GetNode<Control>("ItemAnchor/EquipMotion/TargetMotion/FeedbackMotion/SwayMotion");
            _textureRect = GetNode<TextureRect>("ItemAnchor/EquipMotion/TargetMotion/FeedbackMotion/SwayMotion/Texture");
            _itemAnchor.OffsetTop = EQUIP_POSITION_Y;
            _itemAnchor.OffsetBottom = EQUIP_POSITION_Y;
            _equipMotion.Scale = Vector2.One * EQUIP_SCALE;
            Hide();
        }

        public void SetItem(Texture2D? texture)
        {
            if (texture == null)
            {
                HideItem();
                return;
            }

            var wasVisible = Visible;
            _textureRect.Texture = texture;
            Show();
            PlayEquipAnimation();

            if (!wasVisible)
            {
                PlaySwayAnimation();
            }
        }

        public void HideItem()
        {
            _equipTween?.Kill();
            _targetTween?.Kill();
            _feedbackTween?.Kill();
            _swayTween?.Kill();
            _equipTween = null;
            _targetTween = null;
            _feedbackTween = null;
            _swayTween = null;
            _equipMotion.Position = Vector2.Zero;
            _targetMotion.Position = Vector2.Zero;
            _targetMotion.Scale = Vector2.One;
            _feedbackMotion.Scale = Vector2.One;
            _swayMotion.Rotation = 0;
            _textureRect.Texture = null;
            Hide();
        }

        public void SetInteractionTargeted(bool targeted)
        {
            if (!Visible)
            {
                return;
            }

            _targetTween?.Kill();
            _targetTween = CreateTween()
                .SetParallel()
                .SetPauseMode(Tween.TweenPauseMode.Process);

            var targetPosition = targeted ? new Vector2(0, TARGET_OFFSET) : Vector2.Zero;
            var targetScale = targeted ? Vector2.One * TARGET_SCALE : Vector2.One;

            _targetTween.TweenProperty(
                _targetMotion,
                "position",
                targetPosition,
                TARGET_TRANSITION_DURATION
            ).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);

            _targetTween.TweenProperty(
                _targetMotion,
                "scale",
                targetScale,
                TARGET_TRANSITION_DURATION
            ).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);
        }

        public void PlayRejectedInteractionFeedback()
        {
            if (!Visible)
            {
                return;
            }

            _feedbackTween?.Kill();
            _feedbackTween = CreateTween().SetPauseMode(Tween.TweenPauseMode.Process);

            _feedbackTween.TweenProperty(
                _feedbackMotion,
                "scale",
                Vector2.One * REJECTION_SCALE,
                REJECTION_STEP_DURATION
            ).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.In);

            _feedbackTween.TweenProperty(
                _feedbackMotion,
                "scale",
                Vector2.One,
                REJECTION_STEP_DURATION
            ).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
        }

        void PlayEquipAnimation()
        {
            _equipTween?.Kill();
            _equipMotion.Position = new Vector2(0, EQUIP_OFFSET);

            _equipTween = CreateTween().SetPauseMode(Tween.TweenPauseMode.Process);
            _equipTween.TweenProperty(_equipMotion, "position:y", 0f, EQUIP_DURATION)
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.Out);
        }

        void PlaySwayAnimation()
        {
            _swayTween?.Kill();
            _swayMotion.Rotation = -SWAY_ROTATION;

            _swayTween = CreateTween()
                .SetLoops()
                .SetPauseMode(Tween.TweenPauseMode.Process);

            AddSwayStep(SWAY_ROTATION, SWAY_SIDE_DURATION);
            AddSwayStep(-SWAY_ROTATION, SWAY_SIDE_DURATION);
        }

        void AddSwayStep(float targetRotation, float duration)
        {
            _swayTween!.TweenProperty(_swayMotion, "rotation", targetRotation, duration)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.InOut);
        }
    }
}
