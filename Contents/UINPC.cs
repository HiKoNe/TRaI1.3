using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.UI;

namespace TRaI.Contents
{
    public class UINPC : UIElement
    {
        public NPC NPC { get; set; }
        public int FrameTimer { get; set; }
        public int FrameCounter { get; set; }

        public UINPC(int npcID)
        {
            NPC = new NPC();
            NPC.SetDefaults(npcID);
            Width.Set(0, 1f);
            Height.Set(0, 1f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            var dim = GetDimensions();
            Main.instance.LoadNPC(NPC.type);
            var npcTexture = Main.npcTexture[NPC.type];
            var framesCount = Main.npcFrameCount[NPC.type];
            var textureWidth = npcTexture.Width;
            var textureHeight = npcTexture.Height / framesCount;

            if (++FrameTimer > 3)
            {
                if (++FrameCounter > framesCount - 1)
                    FrameCounter = 0;

                FrameTimer = 0;
            }
            var npcRect = new Rectangle(0, npcTexture.Height / framesCount * FrameCounter, textureWidth, textureHeight);
            float scale = (dim.Width - 10f) / Math.Max(textureWidth, textureHeight);

            Main.spriteBatch.Draw(npcTexture, dim.Center(), npcRect, Color.White, 0, new Vector2(textureWidth, textureHeight) * 0.5f, Vector2.One * scale, 0, 0);

            if (IsMouseHovering)
            {
                string npcName = Lang.GetNPCNameValue(NPC.type) + (NPC.modNPC != null ? $" ({NPC.modNPC.mod.Name})" : "");
                npcName.DrawMouseText();
            }
        }
    }
}
