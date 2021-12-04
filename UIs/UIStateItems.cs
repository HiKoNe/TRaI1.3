using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;
using TRaI.UIs.UIElements;
using UIImageButton = TRaI.UIs.UIElements.UIImageButton;
using UIPanel = TRaI.UIs.UIElements.UIPanel;

namespace TRaI.UIs
{
    public class UIStateItems : UIState
    {
        protected int scroll;

        public UIPanel SearchBar { get; set; }
        public UIInputText SearchBarText { get; set; }
        public UIItemsGrid ItemsGrid { get; set; }
        public UIText TextScroll { get; set; }
        public int CountX { get; private set; }
        public int CountY { get; private set; }
        public List<int> Items { get; set; }
        public int Scroll
        {
            get => scroll;
            set
            {
                if (value < 0)
                    scroll = MaxScroll;
                else if (value > MaxScroll)
                    scroll = 0;
                else
                    scroll = value;
                TextScroll?.SetText(ScrollText);

                ItemsGrid.Items = Items.GetRange(Scroll * ImtesGridCount, Math.Min((Scroll + 1) * ImtesGridCount, Items.Count - Scroll * ImtesGridCount));
            }
        }

        public int ImtesGridCount => CountX * CountY;
        public int MaxScroll => Items.Count / ImtesGridCount;
        public string ScrollText => $"{Scroll + 1} / {MaxScroll + 1} ({Items.Count}) [{TRaI.ItemsCount}]";

        public UIStateItems(int x, int y)
        {
            this.CountX = x;
            this.CountY = y;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

            SearchBar = new UIPanel(TRaIAsset.SearchBar[0], TRaIAsset.SearchBar[1]);
            SearchBar.Left.Set(0, 0.25f);
            SearchBar.Width.Set(0, 0.50f);
            SearchBar.Height.Set(40, 0);
            SearchBar.Top.Set(-SearchBar.Height.Pixels - 10, 1f);
            SearchBar.BackgroundColor = Color.Black;
            SearchBar.BorderColor = Color.White;
            SearchBar.OnClick += (a, b) => SearchBarText.Focus = !SearchBarText.Focus;
            SearchBar.OnRightClick += (a, b) => SearchBarText.Text = "";
            Append(SearchBar);

            SearchBarText = new UIInputText() { Scale = 0.95f };
            SearchBarText.OnTextChange += this.SearchBarText_OnTextChange;
            SearchBar.Append(SearchBarText);

            ItemsGrid = new UIItemsGrid(CountX, CountY, 55, 0.75f);
            Items = TRaISearch.Search("");
            Scroll = 0;
            Append(ItemsGrid);

            TextScroll = new UIText(ScrollText);
            TextScroll.Left.Set(Main.fontMouseText.MeasureString(TextScroll.Text).X / -2, 0.5f);
            TextScroll.Top.Set(-ItemsGrid.GridSizeY - 25 - 55, 1f);
            Append(TextScroll);

            var buttonLeft = new UIImageButton(TRaIAsset.Content[3]);
            buttonLeft.SetHoverImage(TRaIAsset.Content[5]);
            buttonLeft.Top = TextScroll.Top;
            buttonLeft.Top.Pixels -= 5;
            buttonLeft.Left.Set(ItemsGrid.GridSizeX / -2, 0.5f);
            buttonLeft.OnUpdate += e =>
            {
                if (e.IsMouseHovering)
                {
                    Main.LocalPlayer.mouseInterface = true;
                }
            };
            buttonLeft.OnClick += (a, b) => { Scroll--; Main.PlaySound(SoundID.MenuTick); };
            Append(buttonLeft);

            var buttonRight = new UIImageButton(TRaIAsset.Content[4]);
            buttonRight.SetHoverImage(TRaIAsset.Content[5]);
            buttonRight.Top = TextScroll.Top;
            buttonRight.Top.Pixels -= 5;
            buttonRight.Left.Set(ItemsGrid.GridSizeX / 2 - buttonRight.Width.Pixels, 0.5f);
            buttonRight.OnUpdate += e =>
            {
                if (e.IsMouseHovering)
                {
                    Main.LocalPlayer.mouseInterface = true;
                }
            };
            buttonRight.OnClick += (a, b) => { Scroll++; Main.PlaySound(SoundID.MenuTick); };
            Append(buttonRight);
        }

        void SearchBarText_OnTextChange(string text)
        {
            Items = TRaISearch.Search(text);
            Scroll = 0;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (SearchBar.IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            else if (Main.mouseLeft || Main.mouseRight)
                SearchBarText.Focus = false;

            if (Main.keyState.IsKeyDown(Keys.Escape) && !Main.oldKeyState.IsKeyDown(Keys.Escape))
                SearchBarText.Focus = false;

            if (SearchBarText.Focus)
                SearchBar.BorderColor = Color.Yellow;
            else
                SearchBar.BorderColor = Color.White;
        }
    }
}
