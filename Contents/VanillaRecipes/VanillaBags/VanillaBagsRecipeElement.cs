using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TRaI.APIs;
using TRaI.APIs.Ingredients;

namespace TRaI.Contents.VanillaRecipes.VanillaBags
{
    public class VanillaBagsRecipeElement : IRecipeElement
    {
        public Item BagInput { get; set; }
        public List<ItemIngredient> BagOutputs { get; set; }

        public Mod Mod => BagInput.modItem?.mod;

        public VanillaBagsRecipeElement(int bag)
        {
            BagInput = new Item();
            BagInput.SetDefaults(bag);

            switch (bag)
            {
                case ItemID.WoodenCrate:
                case ItemID.IronCrate:
                case ItemID.GoldenCrate:
                case ItemID.JungleFishingCrate:
                case ItemID.FloatingIslandFishingCrate:
                case ItemID.CorruptFishingCrate:
                case ItemID.CrimsonFishingCrate:
                case ItemID.HallowedFishingCrate:
                case ItemID.DungeonFishingCrate:
                    BagOutputs = LootDropEmulation.Emulate(() => Main.LocalPlayer.openCrate(bag));
                    break;

                case ItemID.HerbBag:
                    BagOutputs = LootDropEmulation.Emulate(() => Main.LocalPlayer.openHerbBag());
                    break;
                case ItemID.GoodieBag:
                    BagOutputs = LootDropEmulation.Emulate(() => Main.LocalPlayer.openGoodieBag());
                    break;
                case ItemID.LockBox:
                    BagOutputs = LootDropEmulation.Emulate(() => Main.LocalPlayer.openLockBox());
                    break;
                case ItemID.Present:
                    BagOutputs = LootDropEmulation.Emulate(() => Main.LocalPlayer.openPresent());
                    break;
                case ItemID.BluePresent:
                case ItemID.GreenPresent:
                case ItemID.YellowPresent:
                    BagOutputs = new List<ItemIngredient>()
                    {
                        new ItemIngredient(602, 1, 1, 0.0666f),
                        new ItemIngredient(586, 20, 49, 0.168f),
                        new ItemIngredient(591, 20, 49, 0.168f)
                    };
                    break;
                default:
                    BagOutputs = new List<ItemIngredient>();
                    break;
            }
        }

        public void GetIngredients(RecipeIngredients ingredients)
        {
            ingredients.SetInput(BagInput);
            ingredients.SetOutputs(BagOutputs);
        }
    }
}
