using System;
using UnityEngine;
public static class EventManager
{
    public static event Action<GameObject> OnNewExtractionBuilding;
    public static void InvokeOnNewExtractionBuilding(GameObject extractionBuilding) => OnNewExtractionBuilding?.Invoke(extractionBuilding);

    public static event Action<GameObject> OnNewProductionBuilding;
    public static void InvokeOnNewProductionBuilding(GameObject productionBuilding) => OnNewProductionBuilding?.Invoke(productionBuilding);

    public static event Action<GameObject> OnNewWarehouseBuilding;
    public static void InvokeOnNewWarehouseBuilding(GameObject warehouseBuilding) => OnNewWarehouseBuilding?.Invoke(warehouseBuilding);


}


