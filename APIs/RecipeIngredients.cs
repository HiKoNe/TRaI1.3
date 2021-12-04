using System.Collections.Generic;
using System.Linq;
using TRaI.APIs.Ingredients;

namespace TRaI.APIs
{
    public class RecipeIngredients
    {
        public List<IIngredient> Inputs { get; set; }
        public List<IIngredient> Outputs { get; set; }

        public RecipeIngredients()
        {
            Inputs = new List<IIngredient>();
            Outputs = new List<IIngredient>();
        }

        public List<T> GetInputs<T>() where T : IIngredient =>
            Inputs.OfType<T>().ToList();
        public List<T> GetOutputs<T>() where T : IIngredient =>
            Outputs.OfType<T>().ToList();

        public void SetInput<T>(T input) where T : IIngredient =>
            Inputs.Add(input);
        public void SetInputs<T>(IEnumerable<T> inputs) where T : IIngredient
        {
            foreach (var input in inputs)
                Inputs.Add(input);
        }

        public void SetOutput<T>(T output) where T : IIngredient =>
            Outputs.Add(output);
        public void SetOutputs<T>(IEnumerable<T> outputs) where T : IIngredient
        {
            foreach (var output in outputs)
                Outputs.Add(output);
        }

        public bool ContainsInput(IIngredient ingredient) => Inputs.Any(i => i.GetType() == ingredient.GetType() && i.Equals(ingredient));
        public bool ContainsOutput(IIngredient ingredient) => Outputs.Any(i => i.GetType() == ingredient.GetType() && i.Equals(ingredient));
    }
}
