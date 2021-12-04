using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.ModLoader;
using TRaI.APIs.Ingredients;

namespace TRaI.Contents
{
    public static class LootDropEmulation
    {
        internal static bool Emulation { get; set; }
        internal static Dictionary<int, List<int>> EmulatedItems { get; set; } = new Dictionary<int, List<int>>();

        internal static void OnItemDrop(int type, int stack)
        {
            if (type > 0 && stack > 0)
            {
                if (!EmulatedItems.ContainsKey(type))
                    EmulatedItems.Add(type, new List<int>());
                EmulatedItems[type].Add(stack);
            }
        }

        static readonly object ProgressObj = typeof(Mod).Assembly
            .GetType("Terraria.ModLoader.UI.Interface")
            .GetField("loadMods", BindingFlags.NonPublic | BindingFlags.Static)
            .GetValue(null);
        static readonly MethodInfo SetProgressTextMethod = typeof(Mod).Assembly
            .GetType("Terraria.ModLoader.UI.UILoadMods")
            .GetMethod("SetProgressText", BindingFlags.NonPublic | BindingFlags.Instance);
        static readonly PropertyInfo ProgressProperty = typeof(Mod).Assembly
            .GetType("Terraria.ModLoader.UI.UILoadMods")
            .GetProperty("Progress", BindingFlags.Public | BindingFlags.Instance);

        public static void SetLoadingName(string name) =>
            SetProgressTextMethod.Invoke(ProgressObj, new object[] { "LootEmulations".GetLocalizeText(name) });

        public static void SetLoadingProgress(float progress) =>
            ProgressProperty.SetValue(ProgressObj, progress);

        public static List<ItemIngredient> Emulate(Action action, int iterations = 20000)
        {
            EmulatedItems.Clear();

            Emulation = true;
            for (int i = 0; i < iterations; i++)
                action.Invoke();
            Emulation = false;

            var list = new List<ItemIngredient>();
            foreach (var item in EmulatedItems)
            {
                var type = item.Key;
                var stacks = item.Value;
                list.Add(new ItemIngredient(type, stacks.Min(), stacks.Max(), stacks.Count / (float)iterations));
            }

            list.Sort((d, d2) => d2.Chance.CompareTo(d.Chance));
            return list;
        }
    }
}
