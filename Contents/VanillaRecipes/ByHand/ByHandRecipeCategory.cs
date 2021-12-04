using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.UI;
using TRaI.APIs;
using TRaI.APIs.Ingredients;
using TRaI.UIs.UIElements;

namespace TRaI.Contents.VanillaRecipes.ByHand
{
    public class ByHandRecipeCategory : RecipeCategory
    {
        public override Texture2D TextureIcon => TRaIAsset.Content[0];
        public override LocalizedText Name => Language.GetText("Mods.TRaI.ByHand");
        public override StyleDimension Width => new StyleDimension(0, 1f);
        public override int Height => 116;

        public override void InitRecipes()
        {
            foreach (var recipe in Main.recipe)
                if (recipe.createItem != null && !recipe.createItem.IsAir)
                    Recipes.Add(new ByHandRecipeElement(recipe));
        }

        public override void InitElement(UIRecipeLayout layout, IRecipeElement recipeElement, RecipeIngredients ingredients)
        {
            layout.AddItemIngredients(ingredients.GetInputs<ItemIngredient>(), true, 0, 0, 12, 1);
            layout.AddTileIngredients(ingredients.GetInputs<TileIngredient>(), true, 0, 54, 12, 1);

            var image = layout.AddImage(TRaIAsset.Content[2], 54 * 12, 0);
            image.VAlign = 0.5f;
            image.NormalizedOrigin = Vector2.One * 0.5f;
            image.Rotation = -MathHelper.PiOver2;

            var ingredient = layout.AddItemIngredient(ingredients.GetOutputs<ItemIngredient>()[0], false, 54 * 13, 0);
            ingredient.VAlign = 0.5f;
        }
    }
}
