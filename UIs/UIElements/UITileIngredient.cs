using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.UI;
using TRaI.APIs.Ingredients;

namespace TRaI.UIs.UIElements
{
    public class UITileIngredient : UIElement
    {
        protected int current;

        public List<int> Tiles { get; set; } = new List<int>();

        public int Count => Tiles.Count;

        public UITileIngredient(TileIngredient tileIngredient) :
            this(new List<TileIngredient>() { tileIngredient }) { }
        public UITileIngredient(List<TileIngredient> tileIngredients)
        {
            Width.Set(52, 0);
            Height.Set(52, 0);

            foreach (var tileIngredient in tileIngredients)
                AddTile(tileIngredient);
        }

        public void AddTile(TileIngredient tileIngredient)
        {
            Tiles.Add(tileIngredient.TileID);
        }

        int timer;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Count > 1 && !Main.keyState.PressingShift())
            {
                if (timer++ > 60)
                {
                    if (++current >= Count)
                    {
                        current = 0;
                    }
                    timer = 0;
                }
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            spriteBatch.Draw(Main.inventoryBack2Texture, GetDimensions().Position(), Color.White);

            if (Tiles[current] > -1)
            {
                if (Tiles.Count > 0)
                {
                    var texture = TRaIUtils.GetTileTexture(Tiles[current]);
                    float scale = (GetDimensions().Width - 10f) / Math.Max(texture.Width, texture.Height);
                    spriteBatch.Draw(texture, GetDimensions().Center(), null, Color.White, 0f, texture.Size() * 0.5f, Vector2.One * scale, 0, 0);
                }

                if (IsMouseHovering)
                    TRaIUtils.GetTileName(Tiles[current]).DrawMouseText();
            }
        }
    }
}
