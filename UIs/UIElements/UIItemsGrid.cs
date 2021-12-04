using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.UI;

namespace TRaI.UIs.UIElements
{
    public class UIItemsGrid : UIElement
    {
        public int CountX { get; set; }
        public int CountY { get; set; }
        public IList<int> Items { get; set; }
        public float Scale { get; set; }
        public int Indent { get; set; }

        public float Size => 52 * Scale;
        public float GridSizeX => CountX * (Size + Indent) - Indent;
        public float GridSizeY => CountY * (Size + Indent) - Indent;

        public UIItemsGrid(int countX, int countY, int downPadding, float scale = 1f, int indent = 2)
        {
            Items = new int[0];
            CountX = countX;
            CountY = countY;
            Scale = Math.Max(scale, 0f);
            Indent = Math.Max(indent, 0);
            Left.Set(GridSizeX / -2, 0.5f);
            Width.Set(GridSizeX / 2, 0.5f);
            Height.Set(GridSizeY, 0);
            Top.Set(-Height.Pixels - downPadding, 1f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            var rect = GetDimensions().ToRectangle();

            var oldScale = Main.inventoryScale;
            Main.inventoryScale = Scale;

            int current = 0;
            for (int j = 0; j < CountY; j++)
            {
                for (int i = 0; i < CountX; i++)
                {
                    if (current < Items.Count)
                    {
                        int id = Items[current++];
                        ref var item = ref TRaI.AllItems[id];
                        var pos = rect.TopLeft() + new Vector2(i * (Size + Indent), j * (Size + Indent));
                        ItemSlot.Draw(spriteBatch, ref item, ItemSlot.Context.ChestItem, pos, Color.White);

                        if (IsMouseHovering && Main.mouseX > pos.X && Main.mouseX < pos.X + Size && Main.mouseY > pos.Y && Main.mouseY < pos.Y + Size)
                        {
                            Main.LocalPlayer.mouseInterface = true;
                            TRaIUtils.HoverItem = item.Clone();

                            if (Main.mouseLeft && Main.mouseLeftRelease)
                                TRaIUI.OpenRecipes(item, true);
                            else if (Main.mouseRight && Main.mouseRightRelease)
                                TRaIUI.OpenRecipes(item, false);
                        }
                    }
                }
            }

            Main.inventoryScale = oldScale;
        }
    }
}
