

using System.Collections.Generic;

namespace MyBakery;


public class Recipe{//CURRENTLY UNUSED

    public string name {get; set;}
    public Dictionary<string, int> ingredients {get; set;}

    public Recipe(string n, Dictionary<string, int> ing)
    {
        name = n;
        ingredients = ing;
    }

    public void addIngredient(string item, int quantity){
        ingredients.Add(item, quantity);
    }
}