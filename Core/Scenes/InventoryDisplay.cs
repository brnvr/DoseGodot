using System.Collections.Generic;
using Dose.Core.Library.Actors;
using Dose.Core.Library.DataStructures;
using Godot;

namespace Dose.Core.Scenes
{
    public partial class InventoryDisplay : GridContainer
    {
        const float TILE_SIZE = 72f;
        const float TEXTURE_MARGIN = 8f;

        static readonly Color SelectedColor = new("ffd319");
        static readonly Color EquipedColor = new("24d7ff");

        public void UpdateItems(
            IEnumerable<InventoryEntry> entries,
            ItemContext? selectedItem,
            ItemContext? equipedItem
        )
        {
            foreach (var child in GetChildren())
            {
                RemoveChild(child);
                child.QueueFree();
            }

            foreach (var entry in entries)
            {
                AddChild(CreateItemTile(
                    entry,
                    entry.Item == selectedItem,
                    entry.Item == equipedItem
                ));
            }
        }

        static Control CreateItemTile(
            InventoryEntry entry,
            bool selected,
            bool equiped
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

            var equipedOutline = CreateOutline(EquipedColor, 2, 5);
            equipedOutline.Visible = equiped;
            tile.AddChild(equipedOutline);

            var texture = new TextureRect
            {
                Texture = entry.Item.Texture,
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
                MouseFilter = MouseFilterEnum.Ignore,
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

            return tile;
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
