using CreatorKitCode;
using CreatorKitCodeInternal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharScreen : MenuScreen
{
    public static event Action InventoryShown;
    public static event Action<InventorySystem> InventoryUpdated;
    public InventoryUI Inventory;
    public override void ShowScreen()
    {
        base.ShowScreen();
        Inventory.Load();
    }
    private void OnEnable()
    {
        Inventory.Init();

    }
   
}
