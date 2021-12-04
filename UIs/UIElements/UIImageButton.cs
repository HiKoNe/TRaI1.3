using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace TRaI.UIs.UIElements
{
    public class UIImageButton : UIElement
    {
        private Texture2D Texture { get; set; }
        private float VisibilityActive { get; set; } = 1f;
        private float VisibilityInactive { get; set; } = 0.4f;
        private Texture2D BorderTexture { get; set; }

        public event Action<UIElement> OnUpdate;

        public UIImageButton(Texture2D texture)
        {
            Texture = texture;
            Width.Set(Texture.Width, 0f);
            Height.Set(Texture.Height, 0f);
        }

        public void SetHoverImage(Texture2D texture)
        {
            BorderTexture = texture;
        }

        public void SetImage(Texture2D texture)
        {
            Texture = texture;
            Width.Set(Texture.Width, 0f);
            Height.Set(Texture.Height, 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = base.GetDimensions();
            spriteBatch.Draw(Texture, dimensions.Position(), Color.White * (IsMouseHovering ? VisibilityActive : VisibilityInactive));
            bool flag = BorderTexture != null && IsMouseHovering;
            if (flag)
                spriteBatch.Draw(BorderTexture, dimensions.Position(), Color.White);

            OnUpdate?.Invoke(this);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            Main.PlaySound(SoundID.MenuTick);
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);
        }

        public void SetVisibility(float whenActive, float whenInactive)
        {
            VisibilityActive = MathHelper.Clamp(whenActive, 0f, 1f);
            VisibilityInactive = MathHelper.Clamp(whenInactive, 0f, 1f);
        }
    }
}
