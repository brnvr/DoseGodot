using Godot;
using System;
using System.Threading.Tasks;

namespace Dose.Core.Scenes
{
    public partial class FadeTransition : Control
    {
        private AnimationPlayer? _anim;
        private ColorRect? _rect;
        public static FadeTransition? Instance { get; private set; }

        public override void _Ready()
        {
            Instance = this;
            _anim = GetNode<AnimationPlayer>("AnimationPlayer");
            _rect = GetNode<ColorRect>("ColorRect");
        }

        public async Task FadeOut(float speed = 1f)
        {
            if (_anim == null) return;

            _anim.SpeedScale = speed;
            _anim.Play("FadeOut");
            await ToSignal(_anim, AnimationMixer.SignalName.AnimationFinished);
        }

        public async Task FadeIn(float speed = 1f)
        {
            if (_anim == null) return;

            _anim.SpeedScale = speed;
            _anim.Play("FadeIn");
            await ToSignal(_anim, AnimationMixer.SignalName.AnimationFinished);
        }

        public async Task Fade(Action midAction, float speed = 1f)
        {
            await FadeOut(speed);
            midAction?.Invoke();
            await FadeIn(speed);
        }

    }
}
