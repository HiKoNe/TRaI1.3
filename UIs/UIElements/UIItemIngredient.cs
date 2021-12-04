using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using TRaI.APIs.Ingredients;

namespace TRaI.UIs.UIElements
{
    public class UIItemIngredient : UIElement
    {
        protected int current;

        public List<Item> Item { get; set; } = new List<Item>();
        public List<string> Info { get; set; } = new List<string>();

        public int Count => Item.Count;

        public UIItemIngredient(ItemIngredient itemIngredient) :
            this(new List<ItemIngredient>() { itemIngredient }) { }

        public UIItemIngredient(List<ItemIngredient> itemIngredients)
        {
            Width.Set(52, 0);
            Height.Set(52, 0);

            foreach (var itemIngredient in itemIngredients)
                AddItem(itemIngredient);
        }

        public void AddItem(ItemIngredient itemIngredient)
        {
            bool flag = itemIngredient.StackMin == itemIngredient.StackMax;

            var item = new Item();
            item.SetDefaults(itemIngredient.ItemID);
            item.stack = flag ? itemIngredient.StackMin : 1;
            Item.Add(item);

            var list = new List<string>();

            if (!flag)
                list.Add($"{itemIngredient.StackMin}-{itemIngredient.StackMax}");
            if (itemIngredient.Chance != 1f)
                list.Add($" ({itemIngredient.Chance * 100:0.####}%)");

            foreach (var conditionName in itemIngredient.Conditions)
                if (!string.IsNullOrEmpty(conditionName))
                    list.Add(conditionName);

            Info.Add(string.Join("\n", list).Trim(' ', '\n'));
        }

        int timer;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            if (Count > 1 && !Main.keyState.PressingShift())
            {
                if (timer++ > 60)
                {
                    if (++current >= Count)
                    {
                        current = 0;
                    }
                    timer = 0;
                }
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            var oldScale = Main.inventoryScale;
            Main.inventoryScale = 1f;

            var pos = GetDimensions().Position();
            var item = Item[current];
            ItemSlot.Draw(spriteBatch, ref item, ItemSlot.Context.ChestItem, pos);
            if (Count > 0 && IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
                TRaIUtils.HoverItem = Item[current].Clone();
                if (Info[current].Length > 0)
                    TRaIGlobalItems.AddTooltip = new TooltipLine(TRaI.Instance, "AddTooltip", Info[current]) { overrideColor = Color.Gold };
            }

            Main.inventoryScale = oldScale;
        }

        public override void Click(UIMouseEvent evt)
        {
            base.Click(evt);
            if (!Item[current].IsAir)
                TRaIUI.OpenRecipes(Item[current], true);
        }

        public override void RightClick(UIMouseEvent evt)
        {
            base.RightClick(evt);
            if (!Item[current].IsAir)
                TRaIUI.OpenRecipes(Item[current], false);
        }
    }
}
