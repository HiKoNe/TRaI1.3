using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TRaI.APIs;
using TRaI.APIs.Ingredients;

namespace TRaI.Contents.VanillaRecipes.NPCDrop
{
    public class NPCDropRecipeElement : IRecipeElement
    {
        public int NPCID { get; set; }
        public List<ItemIngredient> NPCOutputs { get; set; }

        public Mod Mod => NPCLoader.GetNPC(NPCID)?.mod;

        public NPCDropRecipeElement(int npcID)
        {
            NPCID = npcID;
            NPCOutputs = new List<ItemIngredient>();

            var npc = new NPC();
            npc.SetDefaults(npcID);
            
            NPCOutputs = LootDropEmulation.Emulate(() =>
            {
                try
                {
                    npc.NPCLoot();
                }
                catch
                {
                }
            }, 50);
            NPCOutputs.RemoveAll(i => i.ItemID == ItemID.Heart || i.ItemID == ItemID.LesserHealingPotion);
        }

        public void GetIngredients(RecipeIngredients ingredients)
        {
            ingredients.SetOutputs(NPCOutputs);
        }
    }
}
