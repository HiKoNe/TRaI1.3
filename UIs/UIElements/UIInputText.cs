using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.UI;

namespace TRaI.UIs.UIElements
{
    public class UIInputText : UIElement
    {
        protected string text;
        protected int cursorTimer;
        protected int cursorPosition;

        public string Text
        {
            get => text;
            set
            {
                CursorPosition = Math.Min(CursorPosition, value.Length);
                text = value;
                OnTextChange?.Invoke(Text);
            }
        }
        public bool Big { get; set; }
        public float Scale { get; set; }
        public bool Focus { get; set; }
        public DynamicSpriteFont Font { get; set; }
        public Color TextColor { get; set; }
        public Vector2 MeasureText { get; set; }
        public int CursorPosition { get => cursorPosition; set => cursorPosition = (int)MathHelper.Clamp(value, 0, Text.Length); }

        public delegate void onTextChange(string text);
        public event onTextChange OnTextChange;

        public UIInputText()
        {
            text = "";
            Big = false;
            Focus = false;
            Scale = 1f;
            Font = Main.fontMouseText;
            TextColor = Color.White;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            cursorTimer++;
            cursorTimer %= 60;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            var pos = this.GetDimensions().ToRectangle().TopLeft();

            if (Focus)
            {
                Main.LocalPlayer.mouseInterface = true;
                PlayerInput.WritingText = true;
                Main.instance.HandleIME();

                var newText = Main.GetInputText("");
                Text = Text.Insert(CursorPosition, newText);
                CursorPosition += newText.Length;

                if (IsKey(Keys.Left))
                    CursorPosition--;

                if (IsKey(Keys.Right))
                    CursorPosition++;

                if (IsKey(Keys.Back) && CursorPosition > 0)
                    Text = Text.Remove(CursorPosition - 1, 1);

                if (IsKey(Keys.Delete) && CursorPosition < Text.Length)
                    Text = Text.Remove(CursorPosition, 1);
            }

            DrawString(spriteBatch, Text, pos);

            if (Focus && cursorTimer < 30)
            {
                float cursorX = Font.MeasureString(Text.Substring(0, CursorPosition)).X;
                DrawString(spriteBatch, "|", pos + new Vector2(cursorX - 3, 0));
            }
        }

        public void DrawString(SpriteBatch spriteBatch, string text, Vector2 pos)
        {
            if (Big)
                MeasureText = Utils.DrawBorderStringBig(spriteBatch, text, pos, TextColor, Scale);
            else
                MeasureText = Utils.DrawBorderString(spriteBatch, text, pos, TextColor, Scale);
        }

        public static bool IsKey(Keys key)
        {
            return Main.keyState.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key);
        }
    }
}
