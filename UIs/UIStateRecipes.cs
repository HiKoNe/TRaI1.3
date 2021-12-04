using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;
using TRaI.APIs;
using TRaI.APIs.Ingredients;
using TRaI.UIs.UIElements;
using UIImage = TRaI.UIs.UIElements.UIImage;
using UIImageButton = TRaI.UIs.UIElements.UIImageButton;
using UIPanel = TRaI.UIs.UIElements.UIPanel;

namespace TRaI.UIs
{
    public enum Mode { All, Recipe, Use }
    public class UIStateRecipes : UIState
    {
        public const int MAX_CATEGORIES = 12;

        int categoryActive;
        int currentCategoryPage;
        int currentPage;

        public UIPanel MainPanel { get; set; }
        public UIElement RecipesPanel { get; set; }
        public UIImageButton[] LeftButtons { get; set; } = new UIImageButton[3];
        public UIImageButton[] RightButtons { get; set; } = new UIImageButton[3];
        public UIPanel[] Panels { get; set; } = new UIPanel[3];
        public UIText[] Texts { get; set; } = new UIText[3];
        public UIImageButton[] CategoryTabs { get; set; } = new UIImageButton[MAX_CATEGORIES];

        public Mode Mode { get; set; }
        public IIngredient Ingredient { get; set; }

        //Categories
        public List<RecipeCategory> ActiveCategories { get; set; } = new List<RecipeCategory>();
        public RecipeCategory Category => ActiveCategories[CurrentCategory];
        public int MaxCategoriesPage => (int)Math.Floor((ActiveCategories.Count - 1f) / MAX_CATEGORIES) + 1;
        public int CurrentCategory
        {
            get => categoryActive;
            set
            {
                if (value < 0)
                    categoryActive = ActiveCategories.Count - 1;
                else if (value > ActiveCategories.Count - 1)
                    categoryActive = 0;
                else
                    categoryActive = value;
                UpdateCategories();
            }
        }
        public int CurrentCategoryPage
        {
            get => currentCategoryPage;
            set
            {
                if (value < 0)
                    currentCategoryPage = MaxCategoriesPage - 1;
                else if (value > MaxCategoriesPage - 1)
                    currentCategoryPage = 0;
                else
                    currentCategoryPage = value;
                UpdateCategories();
            }
        }

        //Pages
        public float RecipeHeight { get; set; }
        public List<IRecipeElement> ActiveRecipes { get; set; } = new List<IRecipeElement>();
        public int RecipesPerPage => (int)Math.Floor((RecipesPanel.GetDimensions().Height + 5) / (RecipeHeight + 5));
        public int MaxPages => (int)Math.Ceiling(ActiveRecipes.Count / (float)RecipesPerPage);
        public int CurrentPage
        {
            get => currentPage;
            set
            {
                if (value < 0)
                    currentPage = MaxPages - 1;
                else if (value > MaxPages - 1)
                    currentPage = 0;
                else
                    currentPage = value;
                UpdatePages();
            }
        }

        //History
        public List<(Mode mode, IIngredient ingredient, int category, int page)> History { get; set; } = new List<(Mode mode, IIngredient ingredient, int category, int page)>();
        public int CurrentHistory { get; set; }

        public string[] Text => new string[3]
        {
            "CategoryText".GetLocalizeText(CurrentCategoryPage + 1, MaxCategoriesPage, ActiveCategories.Count, RecipeCategoryLoader.Categories.Count),
            "HistoryText".GetLocalizeText(CurrentHistory + 1, History.Count),
            "PageText".GetLocalizeText(CurrentPage + 1, MaxPages, ActiveRecipes.Count, Category.Recipes.Count),
        };

        public override void OnInitialize()
        {
            base.OnInitialize();

            MainPanel = new UIPanel();
            MainPanel.Top.Set(100, 0);
            MainPanel.Height.Set(-150, 1f);
            MainPanel.Width.Set(792, 0);
            MainPanel.HAlign = 0.5f;
            MainPanel.BackgroundColor = new Color(33, 43, 79) * 0.8f;
            Append(MainPanel);

            for (int i = 0; i < 3; i++)
            {
                ref var leftButton = ref LeftButtons[i];
                leftButton = new UIImageButton(TRaIAsset.Content[3]);
                leftButton.SetHoverImage(TRaIAsset.Content[5]);
                leftButton.SetVisibility(1f, 1f);
                leftButton.Top.Set(i * (leftButton.Height.Pixels + 2), 0);
                MainPanel.Append(leftButton);

                ref var rightButton = ref RightButtons[i];
                rightButton = new UIImageButton(TRaIAsset.Content[4]);
                rightButton.SetHoverImage(TRaIAsset.Content[5]);
                rightButton.SetVisibility(1f, 1f);
                rightButton.Left.Set(-rightButton.Width.Pixels, 1f);
                rightButton.Top = leftButton.Top;
                MainPanel.Append(rightButton);

                ref var panel = ref Panels[i];
                panel = new UIPanel();
                panel.SetPadding(0);
                panel.Height = rightButton.Height;
                panel.BorderColor = panel.BackgroundColor = new Color(73, 85, 186);
                panel.Left.Set(rightButton.Width.Pixels + 5, 0);
                panel.Width.Set(rightButton.Width.Pixels * -2 - 10, 1f);
                panel.Top = leftButton.Top;
                MainPanel.Append(panel);
            }
            LeftButtons[0].OnClick += (a, b) => { CurrentCategoryPage--; Main.PlaySound(SoundID.MenuTick); };
            RightButtons[0].OnClick += (a, b) => { CurrentCategoryPage++; Main.PlaySound(SoundID.MenuTick); };

            LeftButtons[1].OnClick += (a, b) => { HistoryBack(); Main.PlaySound(SoundID.MenuTick); };
            RightButtons[1].OnClick += (a, b) => { HistoryForward(); Main.PlaySound(SoundID.MenuTick); };

            LeftButtons[2].OnClick += (a, b) => { CurrentPage--; Main.PlaySound(SoundID.MenuTick); };
            RightButtons[2].OnClick += (a, b) => { CurrentPage++; Main.PlaySound(SoundID.MenuTick); };

            Panels[0].OnClick += (a, b) => { TRaIUI.OpenRecipes(Mode.All); Main.PlaySound(SoundID.MenuTick); };
            Panels[1].OnClick += (a, b) => { ClearHistory(); Main.PlaySound(SoundID.MenuTick); };
            Panels[2].OnClick += (a, b) => { CurrentPage = 0; Main.PlaySound(SoundID.MenuTick); };

            RecipesPanel = new UIElement();
            RecipesPanel.MarginTop = Panels[2].Top.Pixels + Panels[2].Height.Pixels + 5;
            RecipesPanel.Height.Set(-RecipesPanel.MarginTop, 1f);
            RecipesPanel.Width.Set(0, 1f);
            RecipesPanel.SetPadding(0);
            RecipesPanel.OnScrollWheel += (a, b) =>
            {
                if (a.ScrollWheelValue > 0)
                {
                    CurrentPage--;
                    Main.PlaySound(SoundID.MenuTick);
                }
                else
                {
                    CurrentPage++;
                    Main.PlaySound(SoundID.MenuTick);
                }
            };
            MainPanel.Append(RecipesPanel);

            for (int i = 0; i < 3; i++)
            {
                Texts[i] = new UIText("", 0.8f) { HAlign = 0.5f, VAlign = 0.5f };
                Panels[i].Append(Texts[i]);
            }

            for (int i = 0; i < MAX_CATEGORIES; i++)
            {
                ref var cb = ref CategoryTabs[i];
                cb = new UIImageButton(TRaIAsset.Panel[2]);
                cb.SetHoverImage(TRaIAsset.Panel[0]);
                cb.SetVisibility(1f, 1f);
                cb.HAlign = 0.5f;
                cb.Left.Set(12 + MainPanel.Width.Pixels / -2 + cb.Width.Pixels / 2 + 0 * cb.Width.Pixels, 0);
                cb.Left.Pixels += i * cb.Width.Pixels;
                cb.Top = MainPanel.Top;
                cb.Top.Pixels -= cb.Height.Pixels;
            }
            CategoryTabs[0].OnClick += (a, b) => CurrentCategory = 0;
            CategoryTabs[1].OnClick += (a, b) => CurrentCategory = 1;
            CategoryTabs[2].OnClick += (a, b) => CurrentCategory = 2;
            CategoryTabs[3].OnClick += (a, b) => CurrentCategory = 3;
            CategoryTabs[4].OnClick += (a, b) => CurrentCategory = 4;
            CategoryTabs[5].OnClick += (a, b) => CurrentCategory = 5;
            CategoryTabs[6].OnClick += (a, b) => CurrentCategory = 6;
            CategoryTabs[7].OnClick += (a, b) => CurrentCategory = 7;
            CategoryTabs[8].OnClick += (a, b) => CurrentCategory = 8;
            CategoryTabs[9].OnClick += (a, b) => CurrentCategory = 9;
            CategoryTabs[10].OnClick += (a, b) => CurrentCategory = 10;
            CategoryTabs[11].OnClick += (a, b) => CurrentCategory = 11;
        }

        public override void OnActivate()
        {
            base.OnActivate();

            foreach (var categoryButton in CategoryTabs)
            {
                categoryButton.Remove();
                categoryButton.RemoveAllChildren();
            }

            foreach (var category in RecipeCategoryLoader.Categories)
            {
                if (category.NeedReinit)
                {
                    category.Recipes.Clear();
                    category.InitRecipes();
                }
            }

            if (Mode == Mode.All)
                ActiveCategories = RecipeCategoryLoader.Categories.ToList();
            else
            {
                ActiveCategories.Clear();
                foreach (var category in RecipeCategoryLoader.Categories)
                {
                    foreach (var recipeElement in category.Recipes)
                    {
                        var ingredients = new RecipeIngredients();
                        recipeElement.GetIngredients(ingredients);
                        if ((Mode == Mode.Use) ? ingredients.ContainsInput(Ingredient) : ingredients.ContainsOutput(Ingredient))
                        {
                            ActiveCategories.Add(category);
                            break;
                        }
                    }
                }
            }

            if (ActiveCategories.Count > 0)
                CurrentCategory = 0;
            else if (CurrentHistory == 0)
                IngameFancyUI.Close();
            else
            {
                HistoryBack();
                History.RemoveAt(History.Count - 1);
                Texts[1].SetText(Text[1]);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (Panels[0].IsMouseHovering)
                "CategoryPanel".DrawMouseLangText();
            else if (Panels[1].IsMouseHovering)
                "HistoryPanel".DrawMouseLangText();
            else if (Panels[2].IsMouseHovering)
                "PagePanel".DrawMouseLangText();

            for (int i = 0; i < CategoryTabs.Length; i++)
                if (CategoryTabs[i].IsMouseHovering)
                    ActiveCategories[i + CurrentCategoryPage * MAX_CATEGORIES].Name.DrawMouseLangText();
        }

        public void UpdateCategories()
        {
            for (int i = 0; i < Math.Min(ActiveCategories.Count, MAX_CATEGORIES); i++)
            {
                int j = i + CurrentCategoryPage * MAX_CATEGORIES;
                ref var cb = ref CategoryTabs[j];

                cb.SetImage(TRaIAsset.Panel[j == CurrentCategory ? 2 : 1]);
                Append(cb);

                //if (ActiveCategories[j].TextureIcon.State == AssetState.NotLoaded)
                //    Main.Assets.Request<Texture2D>(ActiveCategories[j].TextureIcon.Name, AssetRequestMode.ImmediateLoad);
                var categoryIcon = new UIImage(ActiveCategories[j].TextureIcon);
                categoryIcon.HAlign = categoryIcon.VAlign = 0.5f;
                cb.Append(categoryIcon);
            }

            if (Mode == Mode.All)
                ActiveRecipes = Category.Recipes.ToList();
            else
                ActiveRecipes = Category.Recipes.Where(r =>
                {
                    var ingredients = new RecipeIngredients();
                    r.GetIngredients(ingredients);
                    return (Mode == Mode.Use) ? ingredients.ContainsInput(Ingredient) : ingredients.ContainsOutput(Ingredient);
                }).ToList();
            RecipeHeight = Category.Height;

            Texts[0].SetText(Text[0]);
            Texts[1].SetText(Text[1]);

            CurrentPage = 0;
        }

        public void UpdatePages()
        {
            RecipesPanel.RemoveAllChildren();

            var H = RecipesPanel.GetDimensions().Height;
            var h = (float)Category.Height;
            float recipePadding = (H - h * RecipesPerPage) / (RecipesPerPage + 1f);

            for (int i = 0; i < ActiveRecipes.Count; i++)
            {
                if (i >= RecipesPerPage)
                    break;

                int j = i + CurrentPage * RecipesPerPage;
                if (j >= ActiveRecipes.Count)
                    break;

                var recipeElement = ActiveRecipes[j];

                var layout = new UIRecipeLayout(recipeElement.Mod);
                layout.SetPadding(5);
                layout.Top.Set(recipePadding + i * (h + recipePadding), 0);
                layout.Width = Category.Width;
                layout.Height.Set(Category.Height, 0);
                var ingredients = new RecipeIngredients();
                recipeElement.GetIngredients(ingredients);
                Category.InitElement(layout, recipeElement, ingredients);
                layout.Width = Category.Width;
                layout.Height.Set(Category.Height, 0);
                RecipesPanel.Append(layout);
            }

            Texts[2].SetText(Text[2]);
        }

        public void ClearHistory()
        {
            CurrentHistory = 0;
            History.Clear();
            History.Add((Mode, Ingredient, CurrentCategory, CurrentPage));
            Texts[1].SetText(Text[1]);
        }

        public void HistoryBack()
        {
            if (!TRaIUI.IsRecipesOpened)
                return;

            if (CurrentHistory > 0)
            {
                CurrentHistory--;
                var (mode, item, category, page) = History[CurrentHistory];
                Ingredient = item;
                Mode = mode;
                OnActivate();
                CurrentCategory = category;
                CurrentPage = page;
            }
        }

        public void HistoryForward()
        {
            if (!TRaIUI.IsRecipesOpened)
                return;

            if (CurrentHistory < History.Count - 1)
            {
                CurrentHistory++;
                var (mode, item, category, page) = History[CurrentHistory];
                Ingredient = item;
                Mode = mode;
                OnActivate();
                CurrentCategory = category;
                CurrentPage = page;
            }
        }
    }
}
