using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using TRaI.APIs;
using Terraria.ID;

namespace TRaI.Contents.VanillaRecipes.NPCShop
{
    public class NPCShopRecipeElement : IRecipeElement
    {
        public int NPCID { get; set; }
        public List<Item> NPCShop { get; set; }

        public Mod Mod => NPCLoader.GetNPC(NPCID)?.mod;

        public NPCShopRecipeElement(int npcID)
        {
            NPCID = npcID;

            if (NPC2Shop.Contains(npcID))
                npcID = NPC2Shop.IndexOf(npcID) + 1;

            var chest = new Chest();
            chest.item = new Item[1000];
            for (int i = 0; i < chest.item.Length; i++)
                chest.item[i] = new Item();

            try
            {
                chest.SetupShop(npcID);
            }
            catch
            {
            }
            NPCShop = chest.item.ToList();

            if (NPCID == 18 || NPCID == 22)
                NPCShop.Clear();

            NPCShop.RemoveAll(i => i.stack <= 0 || i.type <= ItemID.None);
            NPCShop = NPCShop.Distinct(new ItemComparer()).ToList();
        }

        class ItemComparer : IEqualityComparer<Item>
        {
            public bool Equals(Item x, Item y)
            {
                return GetHashCode(x) == GetHashCode(y);
            }

            public int GetHashCode(Item obj)
            {
                return obj.type;
            }
        }

        public void GetIngredients(RecipeIngredients ingredients)
        {
            ingredients.SetOutputs(NPCShop);
        }

        public readonly List<int> NPC2Shop = new List<int>()
        {
            17, 19, 20, 38, 54, 107, 108, 124, 142, 160, 178, 207, 208, 209, 227, 228, 229, 353, 368, 453, 550, 588, 633, 663,
        };
    }
}
