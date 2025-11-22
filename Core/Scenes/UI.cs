using Dose.Core.Library.UI;
using Godot;
using System;

namespace Dose.Core.Scenes
{
    public partial class UI : CanvasLayer, IUI
    {
        Label? _caption;

        public override void _Ready()
        {
            _caption = GetNode<Label>("Caption");
        }

        public void SetCaption(string? text)
        {
            if (_caption == null)
            {
                GD.PushWarning($"Calling {nameof(SetCaption)} before {nameof(_caption)} is initialized.");

                return;
            }

            _caption.Text = text ?? "";
        }   
    }
}