using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TRaI.APIs;
using TRaI.APIs.Ingredients;

namespace TRaI.Contents.VanillaRecipes.BossBag
{
    public class BossBagRecipeElement : IRecipeElement
    {
        public Item BossBagInput { get; set; }
        public List<ItemIngredient> BossBagOutputs { get; set; }

        public Mod Mod => BossBagInput.modItem?.mod;

        public BossBagRecipeElement(int bossBag)
        {
            BossBagInput = new Item();
            BossBagInput.SetDefaults(bossBag);
            BossBagOutputs = LootDropEmulation.Emulate(() =>
            {
                try
                {
                    Main.LocalPlayer.OpenBossBag(bossBag);
                }
                catch
                {
                }
            }, 5000);
        }

        public void GetIngredients(RecipeIngredients ingredients)
        {
            ingredients.SetInput(BossBagInput);
            ingredients.SetOutputs(BossBagOutputs);
        }
    }
}
