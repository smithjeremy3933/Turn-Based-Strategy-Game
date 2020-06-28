using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public List<Item> items = new List<Item>();

    void Awake()
    {
        BuildDatabase();
    }

    public Item GetItem(int id)
    {
        return items.Find(item => item.id == id);
    }

    public Item GetItem(string itemName)
    {
        return items.Find(item => item.title == itemName);
    }

    private void BuildDatabase()
    {
        items = new List<Item>() {
                new Item(0, "Iron Sword", "A common iron Sword.",
                new Dictionary<string, float>
                {
                    { "Power: ", 15f },
                    { "AP Cost: ", 3f }
                }),

                new Item(1, "Steel Sword", "A common steel Sword.",
                new Dictionary<string, float>
                {
                    { "Power: ", 25f },
                    { "AP Cost: ", 4f }
                }),
            };
    }
}
