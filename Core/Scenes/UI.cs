using System.Collections.Generic;
using Dose.Core.Library.Common;
using Dose.Core.Library.UI;
using Godot;
using Dose.Core.Library.Control;

namespace Dose.Core.Scenes
{
    public partial class UI : CanvasLayer, IUI, INotificator
    {
        NotificationCenter? _notificationCenter;
        CenterContainer? _currentMenu;
        Input.MouseModeEnum? _mouseModePrevious;

        public override void _Ready()
        {
            ProcessMode = ProcessModeEnum.Always;
            _notificationCenter = new NotificationCenter(this);

            base._Ready();

        }

        public override void _ExitTree()
        {
            _notificationCenter?.QueueFree();

            base._ExitTree();
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

        public void DisplayMenu(IEnumerable<MenuOption> options)
        {
            HideMenu();

            _mouseModePrevious = Input.MouseMode;
            Input.MouseMode = Input.MouseModeEnum.Visible;

            var center = new CenterContainer
            {
                AnchorLeft = 0,
                AnchorTop = 0,
                AnchorRight = 1,
                AnchorBottom = 1
            };

            var menuBox = new VBoxContainer
            {
                Alignment = BoxContainer.AlignmentMode.Center
            };

            foreach (var option in options)
            {
                var button = new Button
                {
                    Text = option.Text
                };

                if (option.OnPressed != null) button.Pressed += option.OnPressed;
                if (option.OnHover != null) button.MouseEntered += option.OnHover;
                if (option.OnExit != null) button.MouseExited += option.OnExit;
                menuBox.AddChild(button);
            }

            center.AddChild(menuBox);
            AddChild(center);
            _currentMenu = center;
        }

        public void HideMenu()
        {
            if (_mouseModePrevious != null)
            {
                Input.MouseMode = (Input.MouseModeEnum)_mouseModePrevious;
            }
            
            _currentMenu?.QueueFree();
            _currentMenu = null;
        }
    }
}