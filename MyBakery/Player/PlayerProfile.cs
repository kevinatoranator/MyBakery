

using System.Collections.Generic;

namespace MyBakery;


public class PlayerProfile
{
    public string Name { get; set; }
    public Dictionary<string, int> inventory { get; set; }
    //public List<Employee> employees{ get; set; } Emplyee name, sprite type, cost, skills

    public PlayerProfile(string Name)
    {
        this.Name = Name;
        inventory = new Dictionary<string, int>();
    }
}