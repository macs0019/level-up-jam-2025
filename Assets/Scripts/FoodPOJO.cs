using UnityEngine;

[System.Serializable]
public class FoodPOJO
{
    public int Id;
    public string Name;
    public Sprite Icon;

    public FoodPOJO(int id, string name, Sprite icon)
    {
        Id = id;
        Name = name;
        Icon = icon;
    }
}
