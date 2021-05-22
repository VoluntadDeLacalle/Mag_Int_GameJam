using BasicTools.ButtonInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Item[] inventory = new Item[20];
    public UnityEngine.UI.Image[] inventoryImages = new UnityEngine.UI.Image[20];

    [ButtonAttribute("Add To Inventory", "AddToInventory")] [SerializeField]
    private bool _btnInventory;

    public void AddToInventory()
    {
        //Add weight check
        for (int i = 0; i < 20; i++)
        {
            if (inventory[i] == null)
            {
                Debug.Log($"FOUND AT POSITION: {i}");
            }
        }
    }
}
