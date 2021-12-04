using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ObjectData;
using Terraria.UI.Chat;
using TRaI.APIs;
using TRaI.APIs.Ingredients;

namespace TRaI
{
    public static class TRaIUtils
    {
        static readonly Dictionary<int, Texture2D> TileTextures = new Dictionary<int, Texture2D>();

        public static string GetTileName(int tileID)
        {
            var tileName = Lang.GetMapObjectName(MapHelper.TileToLookup(tileID, 0));

            if (string.IsNullOrEmpty(tileName))
                tileName = "Tile: " + tileID;

            return tileName;
        }

        public static Texture2D GetTileTexture(int tileID)
        {
            if (TileTextures.TryGetValue(tileID, out Texture2D texture))
                return texture;

            Main.instance.LoadTiles(tileID);
            var originalTexture = Main.tileTexture[tileID];
            var tileObjectData = TileObjectData.GetTileData(tileID, 0);
            int width = tileObjectData?.Width ?? 1;
            int height = tileObjectData?.Height ?? 1;
            int padding = tileObjectData?.CoordinatePadding ?? 0;

            texture = new Texture2D(Main.graphics.GraphicsDevice, width * 16, height * 16);
            var colorData = new Color[16 * 16];
            if (tileObjectData is null)
            {
                originalTexture.GetData(0, new Rectangle(162, 54, 16, 16), colorData, 0, colorData.Length);
                texture.SetData(0, new Rectangle(0, 0, 16, 16), colorData, 0, colorData.Length);
            }
            else
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        try
                        {
                            originalTexture.GetData(0, new Rectangle(x * 16 + x * padding, y * 16 + y * padding, 16, 16), colorData, 0, colorData.Length);
                            texture.SetData(0, new Rectangle(x * 16, y * 16, 16, 16), colorData, 0, colorData.Length);
                        }
                        catch
                        {
                        }
                    }
                }
            }

            return TileTextures[tileID] = texture;
        }

        static string Text;
        internal static void OnPostDraw(SpriteBatch spriteBatch)
        {
            if (HoverItem != null)
            {
                Main.HoverItem = HoverItem.Clone();
                try
                {
                    Main.instance.MouseText("");
                }
                catch
                {
                }
                
                HoverItem = null;
            }

            if (string.IsNullOrEmpty(Text))
                return;

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontMouseText, Text, new Vector2(Main.mouseX + 20, Main.mouseY + 20), Color.White, 0f, Vector2.Zero, Vector2.One);
            Text = null;
        }

        public static void DrawMouseText(this string text)
        {
            Text = text;
        }
        public static Item HoverItem { get; set; }
        public static void DrawMouseLangText(this LocalizedText text) =>
            DrawMouseText(text.Value);
        public static void DrawMouseLangText(this string textKey) =>
            DrawMouseLangText(Language.GetText("Mods.TRaI." + textKey));

        public static string GetLocalizeText(this string key, params object[] args) =>
            Language.GetTextValue("Mods.TRaI." + key, args);

        public static void SetInput(this RecipeIngredients ingredients, Item item, float chance = 1f, List<string> conditions = null) =>
            ingredients.SetInput(new ItemIngredient(item, item.stack, chance, conditions));
        public static void SetInputs(this RecipeIngredients ingredients, IEnumerable<Item> items)
        {
            foreach (var item in items)
                ingredients.SetInput(new ItemIngredient(item, item.stack));
        }

        public static void SetOutput(this RecipeIngredients ingredients, Item item, float chance = 1f, List<string> conditions = null) =>
            ingredients.SetOutput(new ItemIngredient(item, item.stack, chance, conditions));
        public static void SetOutputs(this RecipeIngredients ingredients, IEnumerable<Item> items)
        {
            foreach (var item in items)
                if (item != null)
                    ingredients.SetOutput(new ItemIngredient(item, item.stack));
        }
    }
}
