

using System;
using System.Collections.Generic;
using GeneralUtil;

namespace MyBakery;

public class Product{

    public enum ProductQualities
    {
        Refrigerated,
        Stackable
    }
    public int Value { get; set; }
    public Boolean Sellable{get; set;}
    public Dictionary<String, int> Recipe {get; set;}
    public HashSet<ProductQualities> ProductQualitiesSet{get; set;}
    

    public void Sell(String item, int amount)
    {
        if (GameManager.PlayerInfo.inventory[item] >= amount)
        {
            GameManager.PlayerInfo.inventory[item] -= amount;
            GameManager.PlayerInfo.inventory["Coin"] += amount * Value;
        }

    }
    public void Buy(String item, int amount){
        if(GameManager.PlayerInfo.inventory["Coin"] >= amount*Value){
             GameManager.PlayerInfo.inventory[item] += amount;
            GameManager.PlayerInfo.inventory["Coin"] -= amount*Value;
        }else
            Console.WriteLine("Out of money");
    }

    public Product(int value, Boolean sellable, Dictionary<String, int> recipe)
    {
        Value = value;
        Sellable = sellable;
        Recipe = recipe;
        ProductQualitiesSet = new HashSet<ProductQualities>();
    }
}