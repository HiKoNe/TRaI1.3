using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TRaI
{
    public class TRaIKeybind : ModPlayer
    {
        public const string TAG_ITEM_PANEL_SHOW = "itemPanelShow";

        public static ModHotKey ItemUsageKeybind { get; set; }
        public static ModHotKey ItemRecipeKeybind { get; set; }
        public static ModHotKey RecipeBackKeybind { get; set; }

        internal static void Load(Mod mod)
        {
            ItemUsageKeybind = mod.RegisterHotKey("Item Usage", "U");
            ItemRecipeKeybind = mod.RegisterHotKey("Item Recipe", "R");
            RecipeBackKeybind = mod.RegisterHotKey("Recipe Back", "Back");
        }

        internal static void Unload()
        {
            ItemUsageKeybind = null;
            ItemRecipeKeybind = null;
            RecipeBackKeybind = null;
        }

        public override void OnEnterWorld(Player player)
        {
            base.OnEnterWorld(player);
            TRaI.Hack = true;
            foreach (var item in TRaI.AllItems)
            {
                TRaI.AllToolTips[item.type] = item.HoverName.ToLower();
                Main.HoverItem = item;
                try
                {
                    Main.instance.MouseText("");
                }
                catch
                {
                }
                
            }
            TRaI.Hack = false;
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            base.ProcessTriggers(triggersSet);
            bool shift = Main.keyState.PressingShift();

            if (Main.HoverItem != null && !Main.HoverItem.IsAir)
            {
                if (ItemUsageKeybind.JustPressed)
                    TRaIUI.OpenRecipes(Main.HoverItem, shift);
                else if (ItemRecipeKeybind.JustPressed)
                    TRaIUI.OpenRecipes(Main.HoverItem, !shift);
            }

            if (RecipeBackKeybind.JustPressed)
            {
                if (shift)
                    TRaIUI.UIRecipes.HistoryForward();
                else
                    TRaIUI.UIRecipes.HistoryBack();
            }
        }

        public override TagCompound Save()
        {
            base.Save();
            return new TagCompound { [TAG_ITEM_PANEL_SHOW] = TRaIUI.ActiveItems };
        }

        public override void Load(TagCompound tag)
        {
            base.Load(tag);
            if (tag.ContainsKey(TAG_ITEM_PANEL_SHOW))
                TRaIUI.ActiveItems = tag.GetBool(TAG_ITEM_PANEL_SHOW);
        }
    }
}
