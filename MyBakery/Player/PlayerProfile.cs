

using System.Collections.Generic;

namespace MyBakery;


public class PlayerProfile
{
    public string Name { get; set; }
    public Dictionary<GameManager.Items, int> inventory { get; set; }

    public PlayerProfile(string Name)
    {
        this.Name = Name;
        inventory = new Dictionary<GameManager.Items, int>(); 
    }
}