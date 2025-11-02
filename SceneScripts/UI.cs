using Godot;
using System;

namespace Dose.SceneScripts
{
    public partial class UI : CanvasLayer
    {
        Label? _actorName;

        public override void _Ready()
        {
            _actorName = GetNode<Label>("ActorName");
        }

        public void SetActorName(string? name)
        {
            if (_actorName == null)
            {
                GD.PushWarning($"Calling {nameof(SetActorName)} before {nameof(_actorName)} is initialized.");

                return;
            }

            _actorName.Text = name ?? "";
        }   
    }
}