using Dose.Core.Library.UI;
using Godot;
using System;

namespace Dose.Core.Scenes
{
    public partial class UI : CanvasLayer, IUI
    {
        public void SetLabelValue(string labelName, string text)
        {
            var label = GetNode<Label>(labelName);
    
            label.Text = text ?? "";
        }

        public void SetLabelVisible(string labelName, bool visible)
        {
            var label = GetNode<Label>(labelName);

            label.Visible = visible;
        }   
    }
}