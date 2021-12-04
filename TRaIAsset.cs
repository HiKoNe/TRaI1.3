using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace TRaI
{
    public static class TRaIAsset
    {
        public static Texture2D[] Button { get; set; }
        public static Texture2D[] SearchBar { get; set; }
        public static Texture2D[] Panel { get; set; }
        public static Texture2D[] Content { get; set; }

        public static void Load(Mod mod)
        {
            Button = new Texture2D[8];
            for (int i = 0; i < Button.Length; i++)
            {
                string name = $"Assets/Button_{i}";
                Button[i] = mod.GetTexture(name);
            }

            SearchBar = new Texture2D[2];
            for (int i = 0; i < SearchBar.Length; i++)
            {
                string name = $"Assets/SearchBar_{i}";
                SearchBar[i] = mod.GetTexture(name);
            }

            Panel = new Texture2D[3];
            for (int i = 0; i < Panel.Length; i++)
            {
                string name = $"Assets/Panel_{i}";
                Panel[i] = mod.GetTexture(name);
            }

            Content = new Texture2D[6];
            for (int i = 0; i < Content.Length; i++)
            {
                string name = $"Assets/Content_{i}";
                Content[i] = mod.GetTexture(name);
            }
        }

        public static void Unload()
        {
            Button = null;
            SearchBar = null;
            Panel = null;
            Content = null;
        }
    }
}
