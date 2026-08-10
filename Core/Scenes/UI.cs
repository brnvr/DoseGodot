using System.Collections.Generic;
using Dose.Core.Library.Common;
using Dose.Core.Library.UI;
using Godot;
using Dose.Core.Library.Control;

namespace Dose.Core.Scenes
{
    public partial class UI : CanvasLayer, IUI, INotificator
    {
        NotificationCenter _notificationCenter = null!;
        CenterContainer? _currentMenu;
        Input.MouseModeEnum? _mouseModePrevious;
        PanelContainer _dialogueCaption = null!;
        Label _dialogueText = null!;

        public bool IsMenuDisplayed { get; private set; }

        public override void _Ready()
        {
            ProcessMode = ProcessModeEnum.Always;
            _dialogueCaption = GetNode<PanelContainer>("DialogueCaption");
            _dialogueText = GetNode<Label>("DialogueCaption/MarginContainer/Text");
            _notificationCenter = GetNode<NotificationCenter>("Notifications");

            base._Ready();
        }


        public void SetLabelValue(string labelName, string text)
        {
            var label = GetNode<Label>(labelName);
            label.Text = text;
            label.Show();
        }

        public void SetLabelVisible(string labelName, bool visible)
        {
            var label = GetNode<Label>(labelName);

            label.Visible = visible;
        }

        public void ShowDialogueLine(string text)
        {
            _dialogueText.Text = text;
            _dialogueCaption.Show();
        }

        public void HideDialogue()
        {
            _dialogueCaption.Hide();
        }

        public void Notify(string message, float duration = 2f)
        {
            _notificationCenter.ShowNotification(message, duration);
        }

        public void DisplayMenu(IEnumerable<MenuOption> options)
        {
            _currentMenu?.QueueFree();
            
            if (!IsMenuDisplayed)
            {
                _mouseModePrevious = Input.MouseMode;
            }

            IsMenuDisplayed = true;
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
                if (option.OnButtonDown != null) button.ButtonDown += option.OnButtonDown;
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
            if (!IsMenuDisplayed)
            {
                return;
            }

            IsMenuDisplayed = false;

            if (_mouseModePrevious != null)
            {
                Input.MouseMode = (Input.MouseModeEnum)_mouseModePrevious;
            }
            
            _currentMenu?.QueueFree();
            _currentMenu = null;
        }
    }
}
