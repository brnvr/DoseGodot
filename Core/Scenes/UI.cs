using Dose.Core.Library.Common;
using Dose.Core.Library.UI;
using Godot;
using System;

namespace Dose.Core.Scenes
{
    public partial class UI : CanvasLayer, IUI, INotificator
    {
        NotificationCenter? _notificationCenter;

        public override void _Ready()
        {
            ProcessMode = ProcessModeEnum.Always;
            _notificationCenter = new NotificationCenter(this);

            base._Ready();
        }

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

        public void Notify(string message, float duration = 2f)
        {
            var notification = new Notification(message, duration);
            AddChild(notification);
            _notificationCenter?.Add(notification);
        }
    }
}