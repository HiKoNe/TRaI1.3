using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace TRaI.UIs.UIElements
{
    public class UIImage : UIElement
    {
		public Texture2D Texture { get; set; }
		public float ImageScale { get; set; } = 1f;
		public float Rotation { get; set; }
		public bool ScaleToFit { get; set; }
		public bool AllowResizingDimensions { get; set; } = true;
		public Color Color { get; set; } = Color.White;
		public Vector2 NormalizedOrigin { get; set; }
		public bool RemoveFloatingPointsFromDrawPosition { get; set; }

		public UIImage(Texture2D texture)
		{
			Texture = texture;
			Width.Set(texture.Width, 0f);
			Height.Set(texture.Height, 0f);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = base.GetDimensions();
			bool scaleToFit = ScaleToFit;
			if (scaleToFit)
			{
				spriteBatch.Draw(Texture, dimensions.ToRectangle(), Color);
			}
			else
			{
				Vector2 vector = Texture.Size();
				Vector2 vector2 = dimensions.Position() + vector * (1f - ImageScale) / 2f + vector * NormalizedOrigin;
				bool removeFloatingPointsFromDrawPosition = RemoveFloatingPointsFromDrawPosition;
				if (removeFloatingPointsFromDrawPosition)
				{
					vector2 = vector2.Floor();
				}
				spriteBatch.Draw(Texture, vector2, null, Color, Rotation, vector * NormalizedOrigin, ImageScale, 0, 0f);
			}
		}
	}
}
