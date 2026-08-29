using System.Collections.Generic;
using Dose.Core.Library.Actors;
using Dose.Core.Library.DataStructures;
using Godot;

namespace Dose.Core.Scenes
{
    public partial class InventoryDisplay : VBoxContainer
    {
        const float TILE_SIZE = 72f;
        const float TEXTURE_MARGIN = 8f;
        const float PICKED_ITEM_SCALE = 1.25f;
        const float PICKED_ITEM_GROW_DURATION = 0.12f;
        const float PICKED_ITEM_SHRINK_DURATION = 0.16f;

        static readonly Color SelectedColor = new("ffd319");

        VBoxContainer _itemDetails = null!;
        Label _itemName = null!;
        Label _itemDescription = null!;
        Label _equipHint = null!;
        GridContainer _items = null!;

        public override void _Ready()
        {
            CacheNodes();
        }

        void CacheNodes()
        {
            _itemDetails ??= GetNode<VBoxContainer>("ItemDetails");
            _itemName ??= GetNode<Label>("ItemDetails/Name");
            _itemDescription ??= GetNode<Label>("ItemDetails/Description");
            _equipHint ??= GetNode<Label>("ItemDetails/EquipHint");
            _items ??= GetNode<GridContainer>("Items");
        }

        public void UpdateItems(
            IEnumerable<InventoryEntry> entries,
            ItemContext? selectedItem,
            ItemContext? equipedItem,
            ItemContext? pickedItem = null
        )
        {
            CacheNodes();
            UpdateItemDetails(selectedItem);

            foreach (var child in _items.GetChildren())
            {
                _items.RemoveChild(child);
                child.QueueFree();
            }

            foreach (var entry in entries)
            {
                var tile = CreateItemTile(
                    entry,
                    entry.Item == selectedItem,
                    entry.Item == equipedItem,
                    out var texture
                );
                _items.AddChild(tile);

                if (entry.Item == pickedItem)
                {
                    PlayPickedItemFeedback(texture);
                }
            }
        }

        void UpdateItemDetails(ItemContext? selectedItem)
        {
            _itemDetails.Visible = selectedItem != null;

            if (selectedItem == null)
            {
                _itemName.Text = "";
                _itemDescription.Text = "";
                _equipHint.Text = "";
                return;
            }

            _itemName.Text = selectedItem.DisplayName;
            _itemDescription.Text = selectedItem.Description;
            _equipHint.Text = "";
        }

        static Control CreateItemTile(
            InventoryEntry entry,
            bool selected,
            bool equiped,
            out TextureRect texture
        )
        {
            var tile = new Control
            {
                CustomMinimumSize = Vector2.One * TILE_SIZE,
                MouseFilter = MouseFilterEnum.Ignore
            };

            var selectedOutline = CreateOutline(SelectedColor, 3);
            selectedOutline.Visible = selected;
            tile.AddChild(selectedOutline);

            texture = new TextureRect
            {
                Texture = entry.Item.Texture,
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
                MouseFilter = MouseFilterEnum.Ignore,
                PivotOffset = Vector2.One * ((TILE_SIZE - TEXTURE_MARGIN * 2) / 2),
                OffsetLeft = TEXTURE_MARGIN,
                OffsetTop = TEXTURE_MARGIN,
                OffsetRight = TILE_SIZE - TEXTURE_MARGIN,
                OffsetBottom = TILE_SIZE - TEXTURE_MARGIN
            };
            tile.AddChild(texture);

            if (entry.Quantity > 1)
            {
                var count = new Label
                {
                    Text = $"x{entry.Quantity}",
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    MouseFilter = MouseFilterEnum.Ignore,
                    OffsetLeft = TILE_SIZE * 0.45f,
                    OffsetTop = TILE_SIZE * 0.62f,
                    OffsetRight = TILE_SIZE - 5,
                    OffsetBottom = TILE_SIZE - 4
                };
                count.AddThemeFontSizeOverride("font_size", 12);
                count.AddThemeColorOverride("font_color", Colors.White);
                count.AddThemeColorOverride("font_shadow_color", Colors.Black);
                count.AddThemeConstantOverride("shadow_offset_x", 1);
                count.AddThemeConstantOverride("shadow_offset_y", 1);
                tile.AddChild(count);
            }

            if (equiped)
            {
                var equipedLabel = new Label
                {
                    Text = "EQUIPED",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    MouseFilter = MouseFilterEnum.Ignore,
                    OffsetTop = -7,
                    OffsetRight = TILE_SIZE,
                    OffsetBottom = 11,
                    ZIndex = 2
                };
                equipedLabel.AddThemeFontSizeOverride("font_size", 10);
                equipedLabel.AddThemeColorOverride("font_color", Colors.White);
                equipedLabel.AddThemeColorOverride("font_shadow_color", Colors.Black);
                equipedLabel.AddThemeConstantOverride("shadow_offset_x", 1);
                equipedLabel.AddThemeConstantOverride("shadow_offset_y", 1);
                equipedLabel.AddThemeConstantOverride("shadow_outline_size", 1);
                tile.AddChild(equipedLabel);
            }

            return tile;
        }

        static void PlayPickedItemFeedback(TextureRect texture)
        {
            var tween = texture.CreateTween();
            tween.TweenProperty(
                texture,
                "scale",
                Vector2.One * PICKED_ITEM_SCALE,
                PICKED_ITEM_GROW_DURATION
            ).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
            tween.TweenProperty(
                texture,
                "scale",
                Vector2.One,
                PICKED_ITEM_SHRINK_DURATION
            ).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.InOut);
        }

        static Panel CreateOutline(Color color, int width, float inset = 0)
        {
            var style = new StyleBoxFlat
            {
                BgColor = Colors.Transparent,
                BorderColor = color,
                BorderWidthLeft = width,
                BorderWidthTop = width,
                BorderWidthRight = width,
                BorderWidthBottom = width
            };

            var outline = new Panel
            {
                MouseFilter = MouseFilterEnum.Ignore,
                OffsetLeft = inset,
                OffsetTop = inset,
                OffsetRight = TILE_SIZE - inset,
                OffsetBottom = TILE_SIZE - inset
            };
            outline.AddThemeStyleboxOverride("panel", style);
            return outline;
        }
    }
}
