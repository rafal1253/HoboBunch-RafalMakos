using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarehouseBuilding : Building
{
    // Start is called before the first frame update
    void Start()
    {
        EventManager.InvokeOnNewWarehouseBuilding(gameObject);
    }
}
