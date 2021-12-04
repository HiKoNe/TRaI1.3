using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;
using TRaI.APIs;
using TRaI.APIs.Ingredients;
using TRaI.UIs.UIElements;

namespace TRaI.Contents.VanillaRecipes.VanillaBags
{
    public class VanillaBagsRecipeCategory : RecipeCategory
    {
        public override Texture2D TextureIcon => Main.itemTexture[ItemID.HerbBag];
        public override LocalizedText Name => Language.GetText("Mods.TRaI.VanillaBags");
        public override StyleDimension Width => new StyleDimension(0, 1f);
        public override int Height => 228;

        public static readonly int[] VanillaBags = new int[]
        {
            ItemID.WoodenCrate,
            ItemID.IronCrate,
            ItemID.GoldenCrate,
            ItemID.JungleFishingCrate,
            ItemID.FloatingIslandFishingCrate,
            ItemID.CorruptFishingCrate,
            ItemID.CrimsonFishingCrate,
            ItemID.HallowedFishingCrate,
            ItemID.DungeonFishingCrate,

            ItemID.HerbBag,
            ItemID.GoodieBag,
            ItemID.LockBox,
            ItemID.Present,
            ItemID.BluePresent,
            ItemID.GreenPresent,
            ItemID.YellowPresent,
        };

        public override void InitRecipes()
        {
            LootDropEmulation.SetLoadingName(Name.Value);

            for (int i = 0; i < VanillaBags.Length; i++)
            {
                LootDropEmulation.SetLoadingProgress(i / (float)VanillaBags.Length);
                Recipes.Add(new VanillaBagsRecipeElement(VanillaBags[i]));
            }
        }

        public override void InitElement(UIRecipeLayout layout, IRecipeElement recipeElement, RecipeIngredients ingredients)
        {
            var ingredient = layout.AddItemIngredient(ingredients.GetInputs<ItemIngredient>()[0], true, 0, 0);
            ingredient.HAlign = 0.5f;

            var image = layout.AddImage(TRaIAsset.Content[2], 0, 54);
            image.HAlign = 0.5f;

            layout.AddItemIngredients(ingredients.GetOutputs<ItemIngredient>(), false, 0, 54 * 2, 14, 2);
        }
    }
}
