using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.UI;
using TRaI.APIs.Ingredients;
using TRaI.UIs;

namespace TRaI
{
    public class TRaIUI
    {
        public static UserInterface UIItems { get; private set; }
        public static UIStateRecipes UIRecipes { get; private set; }

        public static bool ActiveItems
        {
            get => UIItems?.CurrentState != null;
            set
            {
                if (value)
                    UIItems?.SetState(new UIStateItems(TRaIConfig.Instance.NumberItemsWidth, TRaIConfig.Instance.NumberItemsHeight));
                else
                    UIItems?.SetState(null);
            }
        }

        public static bool IsRecipesOpened => Main.InGameUI.CurrentState is UIStateRecipes;

        public static void OpenRecipes(Item item, bool isRecipe)
        {
            if (item is null)
                return;

            OpenRecipes(isRecipe ? Mode.Recipe : Mode.Use, new ItemIngredient(item));
        }

        public static void OpenRecipes(Mode mode = Mode.All, IIngredient ingredient = null)
        {
            Main.PlaySound(SoundID.MenuOpen);
            Main.LocalPlayer.talkNPC = -1;
            if (mode != Mode.All && ingredient is null)
                return;

            UIRecipes.Ingredient = ingredient;
            UIRecipes.Mode = mode;
            UIRecipes.Recalculate();
            if (Main.InGameUI.CurrentState is UIStateRecipes)
            {
                var h = UIRecipes.History[UIRecipes.CurrentHistory];
                h.category = UIRecipes.CurrentCategory;
                h.page = UIRecipes.CurrentPage;
                UIRecipes.History[UIRecipes.CurrentHistory] = h;

                UIRecipes.CurrentHistory++;
                UIRecipes.History.RemoveRange(UIRecipes.CurrentHistory, UIRecipes.History.Count - UIRecipes.CurrentHistory);
                UIRecipes.History.Add((UIRecipes.Mode, UIRecipes.Ingredient, UIRecipes.CurrentCategory, UIRecipes.CurrentPage));
                UIRecipes.Activate();
            }
            else
            {
                UIRecipes.History.Clear();
                UIRecipes.CurrentHistory = 0;
                UIRecipes.History.Add((UIRecipes.Mode, UIRecipes.Ingredient, UIRecipes.CurrentCategory, UIRecipes.CurrentPage));
                
                OpenUIState(UIRecipes);
            }
        }

        public static void OpenUIState(UIState state)
        {
            IngameFancyUI.CoverNextFrame();
            Main.playerInventory = false;
            Main.editChest = false;
            Main.npcChatText = "";
            Main.inFancyUI = true;
            Main.InGameUI.SetState(state);
        }

        internal static void Load()
        {
            UIItems = new UserInterface();
            UIRecipes = new UIStateRecipes();
        }

        internal static void Unload()
        {
            UIItems = null;
            UIRecipes = null;
        }

        internal static void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            var index = layers.FindIndex(layer => layer.Name == "Vanilla: Inventory");
            if (index != -1)
                layers.Insert(index + 1, new LegacyGameInterfaceLayer("TRaI: Items", () =>
                {
                    if (Main.playerInventory && !Main.inFancyUI)
                        UIItems.Draw(Main.spriteBatch, null);
                    return true;
                }, InterfaceScaleType.UI));
        }

        internal static void UpdateUI(GameTime gameTime)
        {
            if (Main.playerInventory && !Main.inFancyUI)
                UIItems.Update(gameTime);
        }
    }
}
