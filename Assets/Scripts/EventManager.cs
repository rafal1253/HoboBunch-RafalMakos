using System;
public static class EventManager
{
    public static event Action<ExtractionBuilding> OnNewExtractionBuilding;
    public static void InvokeOnNewExtractionBuilding(ExtractionBuilding extractionBuilding) => OnNewExtractionBuilding?.Invoke(extractionBuilding);

    public static event Action<ProductionBuilding> OnNewProductionBuilding;
    public static void InvokeOnNewProductionBuilding(ProductionBuilding productionBuilding) => OnNewProductionBuilding?.Invoke(productionBuilding);

    public static event Action<WarehouseBuilding> OnNewWarehouseBuilding;
    public static void InvokeOnNewWarehouseBuilding(WarehouseBuilding warehouseBuilding) => OnNewWarehouseBuilding?.Invoke(warehouseBuilding);


}


