using System.Collections.Generic;
using Terraria;

namespace TRaI.APIs.Ingredients
{
    public class ItemIngredient : IIngredient
    {
        public int ItemID { get; set; }
        public int StackMin { get; set; }
        public int StackMax { get; set; }
        public float Chance { get; set; }
        public List<string> Conditions { get; set; }

        public ItemIngredient()
        {
            ItemID = 0;
            StackMin = 0;
            StackMax = 0;
            Chance = 0f;
            Conditions = new List<string>();
        }

        public ItemIngredient(Item item, int stackMin, int stackMax, float chance = 1f, List<string> conditions = null) :
            this(item.type, stackMin, stackMax, chance, conditions) { }
        public ItemIngredient(int itemID, int stackMin, int stackMax, float chance = 1f, List<string> conditions = null)
        {
            ItemID = itemID;
            StackMin = stackMin;
            StackMax = stackMax;
            Chance = chance;
            Conditions = conditions ?? new List<string>();
        }

        public ItemIngredient(Item item, int stack = 1, float chance = 1f, List<string> conditions = null) :
            this(item.type, stack, chance, conditions) { }
        public ItemIngredient(int itemID, int stack = 1, float chance = 1f, List<string> conditions = null) :
            this(itemID, stack, stack, chance, conditions) { }

        public ItemIngredient Clone()
        {
            return new ItemIngredient(ItemID, StackMin, StackMax, Chance, new List<string>(Conditions));
        }

        public bool Equals(IIngredient ingredient) =>
            ingredient is ItemIngredient itemIngredient && ItemID == itemIngredient.ItemID;
    }
}
