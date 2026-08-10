using Godot;

namespace Dose.Core.Library.UI
{
    public partial class NotificationCenter : VBoxContainer
    {
        const float ENTRY_OFFSET = 8f;
        const float ENTRY_DURATION = 0.2f;
        const float FADE_DURATION = 0.25f;

        public void ShowNotification(string message, float duration)
        {
            var entry = new MarginContainer
            {
                MouseFilter = MouseFilterEnum.Ignore,
                SizeFlagsHorizontal = SizeFlags.ExpandFill
            };

            var label = new Label
            {
                Text = message,
                AutowrapMode = TextServer.AutowrapMode.WordSmart,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                MouseFilter = MouseFilterEnum.Ignore,
                SizeFlagsHorizontal = SizeFlags.ExpandFill,
                SelfModulate = new Color(1, 1, 1, 0)
            };
            label.AddThemeFontSizeOverride("font_size", 10);

            entry.AddChild(label);
            AddChild(entry);
            MoveChild(entry, 0);

            Animate(entry, label, Mathf.Max(duration, 0f));
        }

        async void Animate(MarginContainer entry, Label label, float duration)
        {
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

            if (!GodotObject.IsInstanceValid(entry) || !entry.IsInsideTree())
            {
                return;
            }

            var finalPosition = label.Position;
            label.Position = finalPosition + new Vector2(0, ENTRY_OFFSET);
            label.SelfModulate = Colors.White;

            var tween = CreateTween().SetPauseMode(Tween.TweenPauseMode.Process);

            tween.TweenProperty(label, "position", finalPosition, ENTRY_DURATION)
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.Out);
            tween.TweenInterval(duration);
            tween.TweenProperty(entry, "modulate:a", 0f, FADE_DURATION)
                .SetTrans(Tween.TransitionType.Quad)
                .SetEase(Tween.EaseType.In);
            tween.TweenCallback(Callable.From(entry.QueueFree));
        }
    }
}
