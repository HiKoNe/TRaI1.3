using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics;
using Terraria.UI;

namespace TRaI.UIs.UIElements
{
    public class UIPanel : UIElement
    {
        public int CornerSize { get; set; } = 12;
        public int BarSize { get; set; } = 4;
        public Texture2D BorderTexture { get; set; }
        public Texture2D BackgroundTexture { get; set; }
        public Color BorderColor { get; set; } = Color.Black;
        public Color BackgroundColor { get; set; } = new Color(63, 82, 151) * 0.7f;
        public bool NeedsTextureLoading { get; set; } = false;

        private void LoadTextures()
        {
            if (BorderTexture == null)
                BorderTexture = TextureManager.Load("Images/UI/PanelBorder");
            if (BackgroundTexture == null)
                BackgroundTexture = TextureManager.Load("Images/UI/PanelBackground");
        }

        public UIPanel()
        {
            SetPadding(CornerSize);
            NeedsTextureLoading = true;
        }

        public UIPanel(Texture2D customBackground, Texture2D customborder, int customCornerSize = 12, int customBarSize = 4)
        {
            if (BorderTexture == null)
                BorderTexture = customborder;
            if (BackgroundTexture == null)
                BackgroundTexture = customBackground;
            CornerSize = customCornerSize;
            BarSize = customBarSize;
            SetPadding(CornerSize);
        }

        private void DrawPanel(SpriteBatch spriteBatch, Texture2D texture, Color color)
        {
            CalculatedStyle dimensions = base.GetDimensions();
            var point = new Point((int)dimensions.X, (int)dimensions.Y);
            Point point2 = new Point(point.X + (int)dimensions.Width - CornerSize, point.Y + (int)dimensions.Height - CornerSize);
            int width = point2.X - point.X - CornerSize;
            int height = point2.Y - point.Y - CornerSize;
            spriteBatch.Draw(texture, new Rectangle(point.X, point.Y, CornerSize, CornerSize), new Rectangle?(new Rectangle(0, 0, CornerSize, CornerSize)), color);
            spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y, CornerSize, CornerSize), new Rectangle?(new Rectangle(CornerSize + BarSize, 0, CornerSize, CornerSize)), color);
            spriteBatch.Draw(texture, new Rectangle(point.X, point2.Y, CornerSize, CornerSize), new Rectangle?(new Rectangle(0, CornerSize + BarSize, CornerSize, CornerSize)), color);
            spriteBatch.Draw(texture, new Rectangle(point2.X, point2.Y, CornerSize, CornerSize), new Rectangle?(new Rectangle(CornerSize + BarSize, CornerSize + BarSize, CornerSize, CornerSize)), color);
            spriteBatch.Draw(texture, new Rectangle(point.X + CornerSize, point.Y, width, CornerSize), new Rectangle?(new Rectangle(CornerSize, 0, BarSize, CornerSize)), color);
            spriteBatch.Draw(texture, new Rectangle(point.X + CornerSize, point2.Y, width, CornerSize), new Rectangle?(new Rectangle(CornerSize, CornerSize + BarSize, BarSize, CornerSize)), color);
            spriteBatch.Draw(texture, new Rectangle(point.X, point.Y + CornerSize, CornerSize, height), new Rectangle?(new Rectangle(0, CornerSize, CornerSize, BarSize)), color);
            spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y + CornerSize, CornerSize, height), new Rectangle?(new Rectangle(CornerSize + BarSize, CornerSize, CornerSize, BarSize)), color);
            spriteBatch.Draw(texture, new Rectangle(point.X + CornerSize, point.Y + CornerSize, width, height), new Rectangle?(new Rectangle(CornerSize, CornerSize, BarSize, BarSize)), color);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (NeedsTextureLoading)
            {
                NeedsTextureLoading = false;
                LoadTextures();
            }

            if (BackgroundTexture != null)
                DrawPanel(spriteBatch, BackgroundTexture, BackgroundColor);
            if (BorderTexture != null)
                DrawPanel(spriteBatch, BorderTexture, BorderColor);
        }
    }
}
