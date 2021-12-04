namespace TRaI.APIs.Ingredients
{
    public class TileIngredient : IIngredient
    {
        public int TileID { get; set; }

        public TileIngredient()
        {
            TileID = -1;
        }

        public TileIngredient(int tileID)
        {
            TileID = tileID;
        }

        public bool Equals(IIngredient ingredient) =>
            ingredient is TileIngredient tileIngredient && TileID == tileIngredient.TileID;
    }
}
