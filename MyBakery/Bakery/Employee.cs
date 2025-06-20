
using System.Collections.Generic;
using GeneralUtil;

namespace MyBakery;


public class Employee
{
    public string Name { get; set; }
    public Sprite Sprite { get; set; }
    public int Cost { get; set; }
    public Dictionary<string, int> skills { get; set; }//change to enum instead of string
    public Employee(string name, Sprite sprite, int cost)
    {
        Name = name;
        Sprite = sprite;
        Cost = cost;
    }

    //FUTURE METHODS
    //gain xp
    //level up
}