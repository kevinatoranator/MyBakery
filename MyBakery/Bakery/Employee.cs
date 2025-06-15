
using GeneralUtil;

namespace MyBakery;


public class Employee
{   
    public string name { get; set; }
    public Sprite sprite { get; set; }
    public Employee(string name, Sprite sprite)
    {
        this.name = name;
        this.sprite = sprite;
    }
}