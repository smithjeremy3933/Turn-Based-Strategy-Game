﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public int id;
    public string title;
    public string description;
    public Dictionary<string, float> stats = new Dictionary<string, float>();

    public Item(int id, string title, string description, Dictionary<string, float> stats)
    {
        this.id = id;
        this.title = title;
        this.description = description;
        this.stats = stats;
    }

    public Item(Item item)
    {
        this.id = item.id;
        this.title = item.title;
        this.description = item.description;
        this.stats = item.stats;
    }
}
