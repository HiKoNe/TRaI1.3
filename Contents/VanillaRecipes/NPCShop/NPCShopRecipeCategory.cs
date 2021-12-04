using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using TRaI.APIs;
using TRaI.APIs.Ingredients;
using TRaI.UIs.UIElements;
using UIPanel = TRaI.UIs.UIElements.UIPanel;

namespace TRaI.Contents.VanillaRecipes.NPCShop
{
    public class NPCShopRecipeCategory : RecipeCategory
    {
        public override Texture2D TextureIcon => Main.itemTexture[ItemID.PiggyBank];
        public override LocalizedText Name => Language.GetText("Mods.TRaI.NPCShop");
        public override StyleDimension Width => new StyleDimension(0, 1f);
        public override int Height => 228;
        //public override bool NeedReinit => true;

        public override void InitRecipes()
        {
            TRaI.Hack = true;
            for (int i = 0; i < NPCLoader.NPCCount; i++)
            {
                var npc = new NPC();
                npc.SetDefaults(i);
                if (npc.townNPC || i == NPCID.SkeletonMerchant)
                    Recipes.Add(new NPCShopRecipeElement(i));
            }
            TRaI.Hack = false;
        }

        public override void InitElement(UIRecipeLayout layout, IRecipeElement recipeElement, RecipeIngredients ingredients)
        {
            var npcDrop = (NPCShopRecipeElement)recipeElement;

            var npcPanel = new UIPanel();
            npcPanel.Height.Set(0, 1f);
            npcPanel.Width.Set(54 * 4, 0);
            npcPanel.BackgroundColor = new Color(50, 58, 119);
            layout.Append(npcPanel);

            var npcUI = new UINPC(npcDrop.NPCID);
            npcPanel.Append(npcUI);

            layout.AddItemIngredients(ingredients.GetOutputs<ItemIngredient>(), false, 54 * 4, 0, 10, 4);
        }
    }
}
