using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public List<Item> items = new List<Item>();

    public Item GetItem(int id)
    {
        Item currentItem = items.Find(item => item.id == id);
        if (currentItem != null)
        {
            return currentItem;
        }
        return null;
    }

    public Item GetItem(string itemName)
    {
        return items.Find(item => item.title == itemName);
    }

    public void BuildDatabase()
    {
        items = new List<Item>() {
                new Item(0, "Iron Sword", "A common iron Sword.",
                new Dictionary<string, float>
                {
                    { "ATK", 15f },
                    { "APC", 3f },
                    { "HIT", 0f },
                    { "CRIT", 2f }
                }),

                new Item(1, "Steel Sword", "A common steel Sword.",
                new Dictionary<string, float>
                {
                    { "ATK", 25f },
                    { "APC", 4f },
                    { "HIT", 0f },
                    { "CRIT", 2f }
                }),
            };
    }
}
