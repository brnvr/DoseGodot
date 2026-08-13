using System.Collections.Generic;
using Dose.Core.Library.Common;
using Dose.Core.Library.UI;
using Godot;
using Dose.Core.Library.Control;
using Dose.Core.Library.Actors;
using Dose.Core.Library.DataStructures;

namespace Dose.Core.Scenes
{
    public partial class UI : CanvasLayer, IUI, INotificator
    {
        NotificationCenter _notificationCenter = null!;
        CenterContainer? _currentMenu;
        Input.MouseModeEnum? _mouseModePrevious;
        TextWithBackground _dialogueCaption = null!;
        TextWithBackground _actorNameCaption = null!;
        EquipedItemDisplay _equipedItemDisplay = null!;
        Cursor _cursor = null!;
        InventoryDisplay _inventoryDisplay = null!;
        ColorRect _bottomBar = null!;
        ColorRect _rightBar = null!;

        public bool IsMenuDisplayed { get; private set; }

        public override void _Ready()
        {
            ProcessMode = ProcessModeEnum.Always;
            _dialogueCaption = GetNode<TextWithBackground>("BottomBar/DialogueCaption");
            _actorNameCaption = GetNode<TextWithBackground>("BottomBar/NameCaption");
            _equipedItemDisplay = GetNode<EquipedItemDisplay>("EquipedItemDisplay");
            _cursor = GetNode<Cursor>("Cursor");
            _notificationCenter = GetNode<NotificationCenter>("Notifications");
            _inventoryDisplay = GetNode<InventoryDisplay>("RightBar/CenterContainer/InventoryDisplay");
            _bottomBar = GetNode<ColorRect>("BottomBar");
            _rightBar = GetNode<ColorRect>("RightBar");

            _bottomBar.AnchorTop = UIScreenLayout.GameplayBottomRatio;
            _bottomBar.AnchorRight = UIScreenLayout.GameplayRightRatio;
            _rightBar.AnchorLeft = UIScreenLayout.GameplayRightRatio;
            _notificationCenter.AnchorRight = UIScreenLayout.GameplayRightRatio - 0.02f;

            base._Ready();
        }


        public void SetLabelValue(string labelName, string text)
        {
            var label = GetNode<Label>(labelName);
            label.Text = text;
            label.Show();
        }

        public void SetCursorVisible(bool visible)
        {
            _cursor.Visible = visible;
        }

        public void SetActorName(string? name)
        {
            _actorNameCaption.SetText(name);
        }

        public void SetActorNameVisible(bool visible)
        {
            _actorNameCaption.SetTextVisible(visible);
        }

        public void ShowDialogueLine(string text)
        {
            _dialogueCaption.SetText(text);
        }

        public void HideDialogue()
        {
            _dialogueCaption.SetTextVisible(false);
        }

        public void SetEquipedItem(Texture2D? texture)
        {
            _equipedItemDisplay.SetItem(texture);
        }

        public void SetEquipedItemTargeted(bool targeted)
        {
            _equipedItemDisplay.SetInteractionTargeted(targeted);
        }

        public void PlayRejectedItemInteractionFeedback()
        {
            _equipedItemDisplay.PlayRejectedInteractionFeedback();
        }

        public void UpdateInventory(
            IEnumerable<InventoryEntry> entries,
            ItemContext? selectedItem,
            ItemContext? equipedItem
        )
        {
            _inventoryDisplay.UpdateItems(entries, selectedItem, equipedItem);
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
                AnchorRight = UIScreenLayout.GameplayRightRatio,
                AnchorBottom = UIScreenLayout.GameplayBottomRatio
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

                foreach (var colorName in new[]
                {
                    "font_color",
                    "font_focus_color",
                    "font_hover_color",
                    "font_hover_pressed_color",
                    "font_pressed_color",
                    "font_disabled_color"
                })
                {
                    button.AddThemeColorOverride(colorName, Colors.Transparent);
                }

                var buttonText = new Label
                {
                    Text = option.Text,
                    AnchorRight = 1,
                    AnchorBottom = 1,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    MouseFilter = Control.MouseFilterEnum.Ignore
                };
                buttonText.AddThemeColorOverride("font_color", Colors.White);
                buttonText.AddThemeColorOverride("font_shadow_color", Colors.Black);
                buttonText.AddThemeConstantOverride("shadow_offset_x", 2);
                buttonText.AddThemeConstantOverride("shadow_offset_y", 2);
                buttonText.AddThemeConstantOverride("shadow_outline_size", 1);
                button.AddChild(buttonText);

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
