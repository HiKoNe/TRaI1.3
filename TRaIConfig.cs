using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace TRaI
{
    [Label("TRaI Config")]
    public class TRaIConfig : ModConfig
    {
        public static TRaIConfig Instance => ModContent.GetInstance<TRaIConfig>();

        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Info Display")]
        [Label("Show Item ID")]
        [DefaultValue(true)]
        public bool ShowItemID;

        [Label("Show Mod Name")]
        [DefaultValue(true)]
        public bool ShowModName;

        [Label("Show Item Price")]
        [DefaultValue(true)]
        public bool ShowItemPrice;

        [Header("Items Grid")]
        [Range(5, 40)]
        [Increment(1)]
        [Slider]
        [DrawTicks]
        [DefaultValue(20)]
        [Label("Number Items in Width")]
        public int NumberItemsWidth;

        [Range(1, 20)]
        [Increment(1)]
        [Slider]
        [DrawTicks]
        [DefaultValue(3)]
        [Label("Number Items in Height")]
        public int NumberItemsHeight;

        public override void OnChanged()
        {
            base.OnChanged();
            if (TRaIUI.ActiveItems)
                TRaIUI.ActiveItems = true;
        }
    }
}
