using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using TRaI.APIs;
using TRaI.APIs.Ingredients;
using TRaI.UIs.UIElements;

namespace TRaI.Contents.VanillaRecipes.ModBags
{
    public class ModBagsRecipeCategory : RecipeCategory
    {
        public override Texture2D TextureIcon => Main.itemTexture[ItemID.GoldenKey];
        public override LocalizedText Name => Language.GetText("Mods.TRaI.ModBags");
        public override StyleDimension Width => new StyleDimension(0, 1f);
        public override int Height => 228;

        public override void InitRecipes()
        {
            var list = new List<int>();

            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                var modItem = ItemLoader.GetItem(i);
                bool flag = false;
                try
                {
                    flag = modItem.CanRightClick();
                }
                catch { }
                if (modItem != null && flag)
                {
                    var method = modItem.GetType().GetMethod("RightClick");
                    if (method != null)
                        if (method.DeclaringType != typeof(ModItem))
                            list.Add(i);
                }
            }

            LootDropEmulation.SetLoadingName(Name.Value);

            for (int i = 0; i < list.Count; i++)
            {
                LootDropEmulation.SetLoadingProgress(i / (float)list.Count);
                Recipes.Add(new ModBagsRecipeElement(list[i]));
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
