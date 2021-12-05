using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using TRaI.APIs;
using TRaI.Contents;

namespace TRaI
{
    public class TRaI : Mod
    {
        public static TRaI Instance { get; set; }

        public static Item[] AllItems { get; private set; }
        public static List<Item>[] NPCShops { get; private set; }
        public static List<int>[] AdjacentTiles { get; private set; }
        public static Dictionary<int, string> AllToolTips { get; private set; } = new Dictionary<int, string>();
        public static int ItemsCount { get; private set; }

        public static void Debug(object message) => Instance.Logger.Debug(message);

        public override void Load()
        {
            base.Load();
            Instance = this;
            foreach (var type in Code.GetTypes())
            {
                if (!type.IsAbstract && type.IsSubclassOf(typeof(RecipeCategory)))
                {
                    var category = (RecipeCategory)Activator.CreateInstance(type);
                    category.Load(this);
                }
            }
            TRaIAsset.Load(this);
            TRaIUI.Load();
            TRaIKeybind.Load(this);

            Main.OnPostDraw += Main_OnPostDraw;
            On.Terraria.Main.DrawInventory += Main_DrawInventory;
            OnPostAddRecipes += TRaI_OnPostAddRecipes;
            On.Terraria.Item.NewItem_int_int_int_int_int_int_bool_int_bool_bool += Item_NewItem_int_int_int_int_int_int_bool_int_bool_bool;
            IL.Terraria.NPC.NPCLoot += NPC_NPCLoot;
            On.Terraria.NPC.SpawnOnPlayer += NPC_SpawnOnPlayer;
            OnPostDrawTooltip += TRaI_OnPostDrawTooltip;
            OnPreDrawTooltipLine += TRaI_OnPreDrawTooltipLine;
            IL.Terraria.Chest.SetupShop += Chest_SetupShop;
        }

        public override void Unload()
        {
            base.Unload();
            Instance = null;
            RecipeCategoryLoader.Categories.Clear();
            TRaIAsset.Unload();
            TRaIUI.Unload();
            TRaIKeybind.Unload();

            Main.OnPostDraw -= Main_OnPostDraw;
            On.Terraria.Main.DrawInventory -= Main_DrawInventory;
            OnPostAddRecipes -= TRaI_OnPostAddRecipes;
            On.Terraria.Item.NewItem_int_int_int_int_int_int_bool_int_bool_bool -= Item_NewItem_int_int_int_int_int_int_bool_int_bool_bool;
            IL.Terraria.NPC.NPCLoot -= NPC_NPCLoot;
            On.Terraria.NPC.SpawnOnPlayer -= NPC_SpawnOnPlayer;
            OnPostDrawTooltip -= TRaI_OnPostDrawTooltip;
            OnPreDrawTooltipLine -= TRaI_OnPreDrawTooltipLine;
            IL.Terraria.Chest.SetupShop -= Chest_SetupShop;

            AllItems = null;
            AllToolTips.Clear();
        }

        static IDictionary<string, ModNPC> Mod_npcs(Mod mod) => (IDictionary<string, ModNPC>)typeof(Mod).GetField("npcs", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(mod);
        static IDictionary<string, GlobalNPC> Mod_globalNPCs(Mod mod) => (IDictionary<string, GlobalNPC>)typeof(Mod).GetField("globalNPCs", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(mod);

        public override void PostSetupContent()
        {
            foreach (var mod in ModLoader.Mods)
            {
                foreach (var modNPC in Mod_npcs(mod).Values)
                {
                    //Modify("SetupShop");
                    //Modify("SpecialNPCLoot", true);
                    //Modify("PreNPCLoot", true);
                    //Modify("NPCLoot");
                    //Modify("BossLoot");
                    void Modify(string name, bool isBool = false)
                    {
                        var method = modNPC.GetType().GetMethod(name);
                        if (method != null && method.DeclaringType != typeof(ModNPC))
                        {
                            HookEndpointManager.Modify(method, new Action<ILContext>(il =>
                            {
                                var c = new ILCursor(il);
                                ModifyIL(c, isBool);
                            }));
                        }
                    }
                }
                foreach (var globalNPC in Mod_globalNPCs(mod).Values)
                {
                    var method = globalNPC.GetType().GetMethod("SetupShop");
                    if (method != null && method.DeclaringType != typeof(GlobalNPC))
                    {
                        HookEndpointManager.Modify(method, new Action<ILContext>(il =>
                        {
                            var c = new ILCursor(il);
                            ModifyIL(c);
                        }));
                    }
                }
            }

            base.PostSetupContent();
            var list = new List<Item>();
            //Hack = true;
            Debug("Main.myPlayer: " + Main.myPlayer);
            for (int i = 1; i < ItemLoader.ItemCount; i++)
            {
                //AllItems
                var item = new Item();
                item.SetDefaults(i);
                if (item.type == ItemID.None)
                    continue;
                list.Add(item);

                AllToolTips[item.type] = item.HoverName.ToLower();
            }
            //Hack = false;
            AllItems = list.ToArray();
            ItemsCount = list.Count;

            //AdjacentTiles
            AdjacentTiles = new List<int>[TileLoader.TileCount];
            for (int i = 0; i < TileLoader.TileCount; i++)
            {
                var adjTiles = new List<int>();

                //ModTile
                ModTile modTile = TileLoader.GetTile(i);
                if (modTile != null)
                    adjTiles.AddRange(modTile.adjTiles);

                //GlobalTile
                var delegateObjArray = (object[])typeof(TileLoader)
                    .GetField("HookAdjTiles", BindingFlags.NonPublic | BindingFlags.Static)
                    .GetValue(null);
                foreach (var delegateObj in delegateObjArray)
                    adjTiles.AddRange((int[])delegateObj.GetType()
                        .GetMethod("Invoke")
                        .Invoke(delegateObj, new object[] { i }));

                //VanillaTile
                switch (i)
                {
                    case TileID.Furnaces:
                        adjTiles.Add(TileID.GlassKiln);
                        adjTiles.Add(TileID.Hellforge);
                        adjTiles.Add(TileID.AdamantiteForge);
                        break;
                    case TileID.Hellforge:
                        adjTiles.Add(TileID.AdamantiteForge);
                        break;
                    case TileID.Anvils:
                        adjTiles.Add(TileID.MythrilAnvil);
                        break;
                    case TileID.Bottles:
                        adjTiles.Add(TileID.AlchemyTable);
                        break;
                    case TileID.Tables:
                        adjTiles.Add(TileID.Tables2);
                        adjTiles.Add(TileID.AlchemyTable);
                        adjTiles.Add(TileID.BewitchingTable);
                        break;
                }

                if (adjTiles.Count > 0)
                    AdjacentTiles[i] = adjTiles;
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            base.UpdateUI(gameTime);
            TRaIUI.UpdateUI(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            base.ModifyInterfaceLayers(layers);
            TRaIUI.ModifyInterfaceLayers(layers);
        }

        void Main_DrawInventory(On.Terraria.Main.orig_DrawInventory orig, Main self)
        {
            orig(self);

            if (Main.InReforgeMenu || Main.HidePlayerCraftingMenu)
                return;

            int x = 94;
            int y = (Main.screenHeight - 600) / 2;
            if (Main.screenHeight < 700)
                y = (Main.screenHeight - 508) / 2;
            y += 450;
            if (Main.InGuideCraftMenu)
                y -= 150;

            y += Main.InGuideCraftMenu ? -35 : 35;
            bool isHover = Main.mouseX > x - 15 && Main.mouseX < x + 15 && Main.mouseY > y - 15 && Main.mouseY < y + 15 && !PlayerInput.IgnoreMouseInterface;
            Main.spriteBatch.Draw(TRaIAsset.Button[isHover.ToInt()],
                new Vector2(x, y), null,
                Color.White, 0f, TRaIAsset.Button[isHover.ToInt()].Size() / 2f, 1f, 0, 0f);
            if (isHover)
            {
                Main.instance.MouseText("TRaIItems".GetLocalizeText());
                Main.LocalPlayer.mouseInterface = true;
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    Main.PlaySound(SoundID.MenuTick);
                    TRaIUI.ActiveItems = !TRaIUI.ActiveItems;
                }
            }

            y += Main.InGuideCraftMenu ? -35 : 35;
            isHover = Main.mouseX > x - 15 && Main.mouseX < x + 15 && Main.mouseY > y - 15 && Main.mouseY < y + 15 && !PlayerInput.IgnoreMouseInterface;
            Main.spriteBatch.Draw(TRaIAsset.Button[6 + isHover.ToInt()],
                new Vector2(x, y), null,
                Color.White, 0f, TRaIAsset.Button[6 + isHover.ToInt()].Size() / 2f, 1f, 0, 0f);
            if (isHover)
            {
                Main.instance.MouseText("TRaIRecipes".GetLocalizeText());
                Main.LocalPlayer.mouseInterface = true;
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    TRaIUI.OpenRecipes();
                }
            }

            y += Main.InGuideCraftMenu ? -35 : 35;
            isHover = Main.mouseX > x - 15 && Main.mouseX < x + 15 && Main.mouseY > y - 15 && Main.mouseY < y + 15 && !PlayerInput.IgnoreMouseInterface;
            Main.spriteBatch.Draw(TRaIAsset.Button[2 + isHover.ToInt()],
                new Vector2(x, y), null,
                Color.White, 0f, TRaIAsset.Button[2 + isHover.ToInt()].Size() / 2f, 1f, 0, 0f);
            if (isHover)
            {
                Main.instance.MouseText("TRaIConfig".GetLocalizeText());
                Main.LocalPlayer.mouseInterface = true;
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    Main.PlaySound(SoundID.MenuTick);

                    var InterfaceType = typeof(Mod).Assembly.GetType("Terraria.ModLoader.UI.Interface");
                    var modConfigField = InterfaceType.GetField("modConfig", BindingFlags.NonPublic | BindingFlags.Static);
                    var modConfigObj = modConfigField.GetValue(null);

                    var UIModConfigType = typeof(Mod).Assembly.GetType("Terraria.ModLoader.Config.UI.UIModConfig");
                    var SetModMethod = UIModConfigType.GetMethod("SetMod", BindingFlags.NonPublic | BindingFlags.Instance);

                    IngameOptions.Close();
                    IngameFancyUI.CoverNextFrame();
                    Main.playerInventory = false;
                    Main.editChest = false;
                    Main.npcChatText = "";
                    Main.inFancyUI = true;
                    SetModMethod.Invoke(modConfigObj, new object[] { this, ModContent.GetInstance<TRaIConfig>() });
                    Main.InGameUI.SetState((UIState)modConfigObj);
                }
            }
        }

        void TRaI_OnPostAddRecipes(orig_PostAddRecipes orig)
        {
            orig();
            RecipeCategoryLoader.InitRecipes();
        }

        public static int Item_NewItem_int_int_int_int_int_int_bool_int_bool_bool(On.Terraria.Item.orig_NewItem_int_int_int_int_int_int_bool_int_bool_bool orig, int X, int Y, int Width, int Height, int Type, int Stack, bool noBroadcast, int pfix, bool noGrabDelay, bool reverseLookup)
        {
            int result = 0;
            if (LootDropEmulation.Emulation)
                LootDropEmulation.OnItemDrop(Type, Stack);
            else
                result = orig(X, Y, Width, Height, Type, Stack, noBroadcast, pfix, noGrabDelay, reverseLookup);
            return result;
        }

        static void Main_OnPostDraw(GameTime obj)
        {
            Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, null, null, null, Main.UIScaleMatrix); ;
            TRaIUtils.OnPostDraw(Main.spriteBatch);
            Main.spriteBatch.End();
        }

        static bool hack;
        public static bool Hack { get => hack; set => hack = value; }
        static void NPC_NPCLoot(ILContext il)
        {
            var c = new ILCursor(il);

            ModifyIL(c);

            //c.Index = 0;
            //for (int i = 0; i < 100; i++)
            //{
            //    var instr = c.Instrs[i];
            //    string str = i + ": " + instr.OpCode.ToString();
            //    if (instr.Operand != null)
            //    {
            //        str += " > ";
            //        if (instr.Operand is ILLabel l)
            //            str += c.Instrs.IndexOf(l.Target) + ": " + l.Target.OpCode.ToString();
            //        else
            //            str += instr.Operand.ToString();
            //    }
            //    Debug(str);
            //}
        }

        public static void ModifyIL(ILCursor c, bool isBool = false)
        {
            ILLabel label = default;
            while (c.TryGotoNext(i =>
            {
                if (i.Operand is ILLabel l && !i.OpCode.ToString().StartsWith("beq") && !i.OpCode.ToString().StartsWith("bge") && !i.OpCode.ToString().StartsWith("bgt") && !i.OpCode.ToString().StartsWith("ble") && !i.OpCode.ToString().StartsWith("blt") && !i.OpCode.ToString().StartsWith("bne"))
                {
                    label = l;
                    return true;
                }
                return false;
            }))
            {
                var l = c.MarkLabel();
                var l3 = c.MarkLabel();
                c.Index++;
                c.Emit(OpCodes.Nop);
                l3.Target = c.Prev;
                c.GotoLabel(l, MoveType.Before);

                var l2 = c.DefineLabel();
                c.Next.Operand = l2;
                c.GotoLabel(label, MoveType.Before);
                bool flag = false;
                if (c.Prev.OpCode != OpCodes.Nop)
                {
                    flag = true;
                    c.Emit(OpCodes.Nop);
                }
                l2.Target = c.Prev;
                if (flag)
                {
                    c.Emit(OpCodes.Ldsfld, typeof(TRaI).GetField("hack", BindingFlags.Static | BindingFlags.NonPublic));
                    c.Emit(OpCodes.Brtrue, l3);
                    var l4 = c.MarkLabel();
                    c.Index -= 3;
                    c.Emit(OpCodes.Br, l4);
                    c.Index += 3;
                }
            }

            if (isBool)
                return;

            c.Index = 0;
            int retCount = c.Instrs.Count(i => i.OpCode == OpCodes.Ret);
            for (int r = 0; r < retCount - 1; r++)
            {
                if (!c.TryGotoNext(i => i.MatchRet()))
                    break;

                var l = c.DefineLabel();
                c.Emit(OpCodes.Ldsfld, typeof(TRaI).GetField("hack", BindingFlags.Static | BindingFlags.NonPublic));
                c.Emit(OpCodes.Brtrue, l);
                c.Index++;
                c.MarkLabel(l);
            }
        }

        static void Chest_SetupShop(ILContext il)
        {
            var c = new ILCursor(il);

            //ModifyIL(c);

            while (c.TryGotoNext(i => i.MatchBrtrue(out _)))
            {
                c.EmitDelegate<Func<bool>>(() => !Hack);
                c.Emit(OpCodes.And);
                c.Index++;
            }
            c.Index = 0;
            while (c.TryGotoNext(i => i.MatchBrfalse(out _)))
            {
                c.EmitDelegate<Func<bool>>(() => Hack);
                c.Emit(OpCodes.Or);
                c.Index++;
            }
            int retCount = c.Instrs.Count(i => i.OpCode == OpCodes.Ret);
            c.Index = 0;
            for (int r = 0; r < retCount - 1; r++)
            {
                if (!c.TryGotoNext(i => i.MatchRet()))
                    break;

                var l = c.DefineLabel();
                c.EmitDelegate<Func<bool>>(() => Hack);
                c.Emit(OpCodes.Brtrue, l);
                c.Index++;
                c.MarkLabel(l);
            }
        }

        static void NPC_SpawnOnPlayer(On.Terraria.NPC.orig_SpawnOnPlayer orig, int plr, int Type)
        {
            try
            {
                orig(plr, Type);
            }
            catch
            {
            }
        }

        static void TRaI_OnPostDrawTooltip(orig_PostDrawTooltip orig, Item item, ReadOnlyCollection<DrawableTooltipLine> lines)
        {
            orig(item, lines);

            if (Hack)
                AllToolTips[item.type] = string.Join("\n", lines.Select(l => l.text.ToLower()));
        }

        static bool TRaI_OnPreDrawTooltipLine(orig_PreDrawTooltipLine orig, Item item, DrawableTooltipLine line, ref int yOffset)
        {
            bool flag = orig(item, line, ref yOffset);

            if (Hack)
                return false;

            return flag;
        }

        internal static MethodBase MethodPostAddRecipes => typeof(Mod).Assembly.GetType("Terraria.ModLoader.RecipeHooks").GetMethod("PostAddRecipes", BindingFlags.NonPublic | BindingFlags.Static);
        internal delegate void orig_PostAddRecipes();
        internal delegate void hook_PostAddRecipes(orig_PostAddRecipes orig);
        internal static event hook_PostAddRecipes OnPostAddRecipes
        {
            add => HookEndpointManager.Add<hook_PostAddRecipes>(MethodPostAddRecipes, value);
            remove => HookEndpointManager.Remove<hook_PostAddRecipes>(MethodPostAddRecipes, value);
        }

        internal static MethodBase MethodPostDrawTooltip => typeof(Mod).Assembly.GetType("Terraria.ModLoader.ItemLoader").GetMethod("PostDrawTooltip", BindingFlags.Public | BindingFlags.Static);
        internal delegate void orig_PostDrawTooltip(Item item, ReadOnlyCollection<DrawableTooltipLine> lines);
        internal delegate void hook_PostDrawTooltip(orig_PostDrawTooltip orig, Item item, ReadOnlyCollection<DrawableTooltipLine> lines);
        internal static event hook_PostDrawTooltip OnPostDrawTooltip
        {
            add => HookEndpointManager.Add<hook_PostDrawTooltip>(MethodPostDrawTooltip, value);
            remove => HookEndpointManager.Remove<hook_PostDrawTooltip>(MethodPostDrawTooltip, value);
        }

        internal static MethodBase MethodPreDrawTooltipLine => typeof(Mod).Assembly.GetType("Terraria.ModLoader.ItemLoader").GetMethod("PreDrawTooltipLine", BindingFlags.Public | BindingFlags.Static);
        internal delegate bool orig_PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset);
        internal delegate bool hook_PreDrawTooltipLine(orig_PreDrawTooltipLine orig, Item item, DrawableTooltipLine line, ref int yOffset);
        internal static event hook_PreDrawTooltipLine OnPreDrawTooltipLine
        {
            add => HookEndpointManager.Add<hook_PreDrawTooltipLine>(MethodPreDrawTooltipLine, value);
            remove => HookEndpointManager.Remove<hook_PreDrawTooltipLine>(MethodPreDrawTooltipLine, value);
        }
    }
}