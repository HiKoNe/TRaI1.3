using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TRaI.APIs;
using TRaI.APIs.Ingredients;

namespace TRaI.Contents.VanillaRecipes.ModBags
{
    public class ModBagsRecipeElement : IRecipeElement
    {
        public Item ModBagInput { get; set; }
        public List<ItemIngredient> ModBagOutputs { get; set; }

        public Mod Mod => ModBagInput.modItem?.mod;

        public ModBagsRecipeElement(int modBag)
        {
            ModBagInput = new Item();
            ModBagInput.SetDefaults(modBag);

            var modItem = ItemLoader.GetItem(modBag);
            ModBagOutputs = LootDropEmulation.Emulate(() => modItem.RightClick(Main.LocalPlayer), 5000);
        }

        public void GetIngredients(RecipeIngredients ingredients)
        {
            ingredients.SetInput(ModBagInput);
            ingredients.SetOutputs(ModBagOutputs);
        }
    }
}
