namespace Mod.Helpers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;

/// <summary> Gets all the addressable keys from a game. </summary>
public static class AddressableKeysGrabber
{
    /// <summary> IResourceLocator for the game. </summary>
    public static IResourceLocator MainAddressablesLocator =>
        Addressables.ResourceLocators.FirstOrDefault(loc => loc.LocatorId == "AddressablesMainContentCatalog");

    extension(IResourceLocator locator)
    {
        /// <summary> Gets all the IResourceLocation locations from a IResourceLocator. </summary>
        public IEnumerable<IResourceLocation> AllLocations =>
            locator.GetAllLocationsOfType(typeof(object));

        /// <summary> Gets all the IResourceLocation locations from a IResourceLocator of a specific type. </summary>
        public IEnumerable<IResourceLocation> GetAllLocationsOfType(Type Search) =>
            locator.Keys.SelectMany(key => locator.Locate(key, Search, out IList<IResourceLocation> locations) ? locations : []).Distinct().OrderBy(l => l.PrimaryKey);
    }

    /// <summary> Writes all of the addressable locations to a file. </summary>
    public static void WriteAddressableLocationsToFile(string filePath) =>
        File.WriteAllLines(filePath, MainAddressablesLocator.AllLocations.Select(location => $"{location.PrimaryKey} ({location.ResourceType.Name})"));
}