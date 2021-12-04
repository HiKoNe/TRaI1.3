using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TRaI.APIs;
using TRaI.APIs.Ingredients;
using Terraria.ID;

namespace TRaI.Contents.VanillaRecipes.ByHand
{
    public class ByHandRecipeElement : IRecipeElement
    {
        public Recipe Recipe { get; set; }
        public List<Item> InputItems { get; set; }
        public List<TileIngredient> InputTiles { get; set; }
        public List<string> Conditions { get; set; }
        public Mod Mod => (Recipe as ModRecipe)?.mod;

        public ByHandRecipeElement(Recipe recipe)
        {
            Recipe = recipe;

            InputItems = new List<Item>();
            foreach (var item in Recipe.requiredItem)
                if (item.type > ItemID.None && item.stack > 0)
                    InputItems.Add(item);

            InputTiles = new List<TileIngredient>();
            foreach (var tile in Recipe.requiredTile)
                if (tile > -1)
                    InputTiles.Add(new TileIngredient(tile));

            Conditions = new List<string>();
            if (Recipe.needWater)
                Conditions.Add("Need Water");
            if (Recipe.needLava)
                Conditions.Add("Need Lava");
            if (Recipe.needHoney)
                Conditions.Add("Need Honey");
            if (Recipe.needSnowBiome)
                Conditions.Add("Need Snow Biome");
        }

        public void GetIngredients(RecipeIngredients ingredients)
        {
            ingredients.SetOutput(Recipe.createItem, 1f, Conditions);
            ingredients.SetInputs(InputItems);
            ingredients.SetInputs(InputTiles);
        }
    }
}
