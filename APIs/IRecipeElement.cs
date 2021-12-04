using Terraria.ModLoader;

namespace TRaI.APIs
{
    public interface IRecipeElement
    {
        Mod Mod { get; }
        void GetIngredients(RecipeIngredients ingredients);
    }
}
