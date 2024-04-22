﻿using System;
using System.Collections.Generic;
using AzuCraftyBoxes.APIs;
using AzuCraftyBoxes.IContainers;
using AzuCraftyBoxes.Util.Functions;
using JetBrains.Annotations;
using UnityEngine;
#if ! API
using System.Linq;
#endif

namespace AzuCraftyBoxes;

[PublicAPI]
public class API
{
    public static bool IsLoaded()
    {
#if API
		return false;
#else
        return true;
#endif
    }


    public static Type GetIContainerType()
    {
        return typeof(IContainers.IContainer);
    }

    public static Type GetVanillaContainerType()
    {
        return typeof(IContainers.VanillaContainer);
    }

    public static Type GetKgDrawerType()
    {
        return typeof(IContainers.kgDrawer);
    }

    public static Type GetItemDrawersAPIType()
    {
        return typeof(APIs.ItemDrawers_API);
    }

    public static Type GetBoxesUtilFunctionsType()
    {
        return typeof(Util.Functions.Boxes);
    }

    public static IContainer CreateContainer(string type, params object[] args)
    {
        // Factory method to create container instances
        // 'type' could be "Vanilla", "kgDrawer", etc.
        switch (type)
        {
            case "Vanilla":
                return VanillaContainer.Create(args[0] as Container);
            case "kgDrawer":
                return kgDrawer.Create(args[0] as ItemDrawers_API.Drawer);
            default:
                throw new ArgumentException($"Unknown container type: {type}");
        }
    }

    public static void AddContainer(Container container)
    {
        Boxes.AddContainer(container);
    }

    public static void RemoveContainer(Container container)
    {
        Boxes.RemoveContainer(container);
    }

    public static List<IContainer> GetNearbyContainers<T>(T gameObject, float rangeToUse) where T : Component
    {
        return Boxes.GetNearbyContainers(gameObject, rangeToUse);
    }

    public static Dictionary<string, List<string>> GetExcludedPrefabsForAllContainers()
    {
        return Boxes.GetExcludedPrefabsForAllContainers();
    }

    public static bool CanItemBePulled(string container, string prefab)
    {
        return Boxes.CanItemBePulled(container, prefab);
    }
    
    public static int CountItemInContainer(IContainer container, string itemName)
    {
        if (container.ContainsItem(itemName, 1, out int count))
        {
            return count;
        }
        return 0;
    }
    
    public static bool ContainsItem(IContainer container, string itemName, int amount)
    {
        return container.ContainsItem(itemName, amount, out _);
    }
}