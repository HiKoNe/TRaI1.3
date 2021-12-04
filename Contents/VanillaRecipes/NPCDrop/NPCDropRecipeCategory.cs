using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using TRaI.APIs;
using TRaI.APIs.Ingredients;
using TRaI.UIs.UIElements;
using UIPanel = TRaI.UIs.UIElements.UIPanel;

namespace TRaI.Contents.VanillaRecipes.NPCDrop
{
    public class NPCDropRecipeCategory : RecipeCategory
    {
        public override Texture2D TextureIcon => TRaIAsset.Content[1];
        public override LocalizedText Name => Language.GetText("Mods.TRaI.NPCDrops");
        public override StyleDimension Width => new StyleDimension(0, 1f);
        public override int Height => 228;

        public override void InitRecipes()
        {
            LootDropEmulation.SetLoadingName(Name.Value);

            for (int i = -65; i < NPCLoader.NPCCount; i++)
            {
                LootDropEmulation.SetLoadingProgress((i + 65) / (float)(NPCLoader.NPCCount + 65));
                Recipes.Add(new NPCDropRecipeElement(i));
            }
        }

        public override void InitElement(UIRecipeLayout layout, IRecipeElement recipeElement, RecipeIngredients ingredients)
        {
            var npcDrop = (NPCDropRecipeElement)recipeElement;

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
