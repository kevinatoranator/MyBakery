
using System.Collections.Generic;
using GeneralUtil;
using CoreLibrary.Graphics;

namespace MyBakery;


public class Employee
{
    public string Name { get; set; }
    public int Cost { get; set; }
    public Dictionary<string, int> skills { get; set; }//change to enum instead of string
    public Employee(string name, int cost)
    {
        Name = name;
        Cost = cost;
    }

    //FUTURE METHODS
    //gain xp
    //level up
}