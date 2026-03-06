using System;
using Godot;

namespace Dose.Core.Library.UI
{
    public partial class Notification(string message, float duration) : Node
    {
        public string Message { get; set; } = message;
        public float Duration { get; set; } = duration;
        public EventHandler? Expired { get; set; }

        public override async void _Ready()
        {
            await ToSignal(GetTree().CreateTimer(Duration), SceneTreeTimer.SignalName.Timeout);
            Expired?.Invoke(this, EventArgs.Empty);
        }
    }
}