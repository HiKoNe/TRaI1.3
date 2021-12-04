using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using TRaI.APIs.Ingredients;

namespace TRaI.UIs.UIElements
{
    public class UIRecipeLayout : UIPanel
    {
        public const int Padding = 5;
        public const int ElementSize = 52;
        public const int ElementPadding = 2;
        public const int ElementSizeAndPadding = 54;
        public const int MaxIngredientsPerRow = 14;

        public Mod Mod { get; private set; }

        public UIRecipeLayout(Mod mod)
        {
            Mod = mod;
        }

        public UIImage AddImage(Texture2D texture, int i, int j)
        {
            var image = new UIImage(texture);
            image.Left.Set(i, 0);
            image.Top.Set(j, 0);
            Append(image);
            return image;
        }

        public UIItemIngredient AddItemIngredient(ItemIngredient itemIngredient, bool input, int i, int j)
        {
            itemIngredient = itemIngredient.Clone();
            if (!input)
                itemIngredient.Conditions.Add("RecipeAdded".GetLocalizeText(Mod?.Name ?? "Terraria"));
            var ingredient = new UIItemIngredient(itemIngredient);
            ingredient.Left.Set(i, 0);
            ingredient.Top.Set(j, 0);
            Append(ingredient);
            return ingredient;
        }

        public UITileIngredient AddTileIngredient(TileIngredient tileIngredient, bool input, int i, int j)
        {
            var ingredient = new UITileIngredient(tileIngredient);
            ingredient.Left.Set(i, 0);
            ingredient.Top.Set(j, 0);
            Append(ingredient);
            return ingredient;
        }

        public List<UIItemIngredient> AddItemIngredients(List<ItemIngredient> itemIngredients, bool input, int i, int j, int width, int height)
        {
            if (itemIngredients is null || itemIngredients.Count == 0)
                itemIngredients = new List<ItemIngredient>() { new ItemIngredient() };

            var ingredients = new List<UIItemIngredient>();

            int a = 0;
            bool first = true;
            while (a < itemIngredients.Count)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var item = a < itemIngredients.Count ? itemIngredients[a++] : new ItemIngredient();
                        if (first)
                            ingredients.Add(AddItemIngredient(item, input, i + x * ElementSizeAndPadding, j + y * ElementSizeAndPadding));
                        else
                            ingredients[x + y * width].AddItem(item);
                    }
                }
                first = false;
            }

            return ingredients;
        }

        public List<UITileIngredient> AddTileIngredients(List<TileIngredient> tileIngredients, bool input, int i, int j, int width, int height)
        {
            if (tileIngredients is null || tileIngredients.Count == 0)
                tileIngredients = new List<TileIngredient>() { new TileIngredient() };

            var ingredients = new List<UITileIngredient>();

            int t = 0;
            bool first = true;
            while (t < tileIngredients.Count)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var tile = t < tileIngredients.Count ? tileIngredients[t++] : new TileIngredient();
                        if (first)
                            ingredients.Add(AddTileIngredient(tile, input, i + x * ElementSizeAndPadding, j + y * ElementSizeAndPadding));
                        else
                            ingredients[x + y * width].AddTile(tile);
                    }
                }
                first = false;
            }

            return ingredients;
        }
    }
}
