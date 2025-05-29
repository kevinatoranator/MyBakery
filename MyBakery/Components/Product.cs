

using System;
using GeneralUtil;

namespace MyBakery;

public class Product{

    public int Value{get; set;}
    public GameManager.Items Type{get; set;}
    public int Quantity{get; set;}
    public Sprite Sprite{get; set;}
    public Boolean Sellable{get; set;}

    public void Sell(int amount){
        if(Quantity >= amount){
            Quantity -= amount;
            GameManager.inventory[0].Quantity += amount*Value;
        }
            
    }
    public void Buy(int amount){
        if(GameManager.inventory[0].Quantity >= amount*Value){
            Quantity += amount;
            GameManager.inventory[0].Quantity -= amount*Value;
        }else
            Console.WriteLine("Out of money");
    }

    public Product(GameManager.Items type, int value, int quantity, Boolean sellable, Sprite sprite){
        Type = type;
        Value = value;
        Quantity = quantity;
        Sellable = sellable;
        Sprite = sprite;
    }
}