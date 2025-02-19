﻿using AzuCraftyBoxes.Util.Functions;
using Backpacks;

namespace AzuCraftyBoxes.IContainers;

public class BackpackContainer(ItemContainer _container) : IContainer
{
    public int ProcessContainerInventory(string reqName, int totalAmount, int totalRequirement)
    {
        Inventory cInventory = _container.Inventory;
        int thisAmount = Mathf.Min(cInventory.CountItems(reqName), totalRequirement - totalAmount);


        if (thisAmount == 0) return totalAmount;

        for (int i = 0; i < cInventory.GetAllItems().Count; ++i)
        {
            ItemDrop.ItemData item = cInventory.GetItem(i);
            if (item?.m_shared?.m_name != reqName) continue;
            AzuCraftyBoxesPlugin.AzuCraftyBoxesLogger.LogIfReleaseAndDebugEnable($"Container Total Items Count is {cInventory.GetAllItems().Count}");
            AzuCraftyBoxesPlugin.AzuCraftyBoxesLogger.LogIfReleaseAndDebugEnable($"(ConsumeResourcesPatch) Got stack of {item.m_stack} {reqName}");

            int stackAmount = Mathf.Min(item.m_stack, totalRequirement - totalAmount);
            if (stackAmount == item.m_stack)
            {
                AzuCraftyBoxesPlugin.AzuCraftyBoxesLogger.LogIfReleaseAndDebugEnable($"(ConsumeResourcesPatch) Removing item {reqName} from container");
                AzuCraftyBoxesPlugin.AzuCraftyBoxesLogger.LogIfReleaseAndDebugEnable($"Container inventory before removal: {cInventory.GetAllItems().Count}, Item at index {i}: {cInventory.GetItem(i)?.m_shared?.m_name}");

                bool removed = cInventory.RemoveItem(i);
                AzuCraftyBoxesPlugin.AzuCraftyBoxesLogger.LogIfReleaseAndDebugEnable("Removed was " + removed);
                AzuCraftyBoxesPlugin.AzuCraftyBoxesLogger.LogIfReleaseAndDebugEnable($"Container inventory after attempted removal: {cInventory.GetAllItems().Count}");

                --i;
            }
            else
            {
                AzuCraftyBoxesPlugin.AzuCraftyBoxesLogger.LogIfReleaseAndDebugEnable($"(ConsumeResourcesPatch) Removing {stackAmount} {reqName} from container");
                item.m_stack -= stackAmount;
            }

            totalAmount += stackAmount;

            AzuCraftyBoxesPlugin.AzuCraftyBoxesLogger.LogIfReleaseAndDebugEnable($"(ConsumeResourcesPatch) Total amount is now {totalAmount}/{totalRequirement} {reqName}");

            if (totalAmount >= totalRequirement)
            {
                break;
            }
        }

        try
        {
            _container.Save();
            cInventory.Changed();
        }
        catch
        {
            // Do nothing because this occasionally fails on backpacks. Fix better later.
        }

        AzuCraftyBoxesPlugin.AzuCraftyBoxesLogger.LogIfReleaseAndDebugEnable("Saved container");

        if (totalAmount >= totalRequirement)
        {
            AzuCraftyBoxesPlugin.AzuCraftyBoxesLogger.LogIfReleaseAndDebugEnable($"(ConsumeResourcesPatch) Consumed enough {reqName}");
        }

        return totalAmount;
    }

    public int ItemCount(string name)
    {
        Inventory cInventory = _container.Inventory;
        if (cInventory == null) return 0;
        int result = cInventory.GetAllItems().Where(item => item.m_shared.m_name == name).Sum(item => item.m_stack);
        return result;
    }

    public void RemoveItem(string name, int amount)
    {
        Backpacks.API.DeleteItemsFromBackpacks(Player.m_localPlayer.GetInventory(), name, amount);
    }

    public void RemoveItem(string prefab, string sharedName, int amount)
    {
        Backpacks.API.DeleteItemsFromBackpacks(Player.m_localPlayer.GetInventory(), sharedName, amount);
    }

    public void Save()
    {
        try
        {
            _container.Save();
            _container.Inventory?.Changed();
        }
        catch (Exception e)
        {
            // Ignored for Backpacks.
        }
    }

    public Vector3 GetPosition() => Player.m_localPlayer.transform.position;
    public string GetPrefabName() => _container.Item.m_dropPrefab.name;
    public Inventory GetInventory() => _container.Inventory;


    public static BackpackContainer Create(ItemContainer container) => new(container);
}