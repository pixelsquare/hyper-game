using System.Collections.Generic;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Santelmo.Rinsurv
{
    public static class AssetLoaderUtil
    {
        public static void GetDependenciesFromLocations(IList<IResourceLocation> locations, out List<IResourceLocation> dependencies)
        {
            var locationHashes = new HashSet<IResourceLocation>();

            foreach (var location in locations)
            {
                if (location.ResourceType == typeof(IAssetBundleResource))
                {
                    locationHashes.Add(location);
                }

                if (!location.HasDependencies)
                {
                    continue;
                }

                foreach (var dependency in location.Dependencies)
                {
                    if (dependency.ResourceType == typeof(IAssetBundleResource))
                    {
                        locationHashes.Add(dependency);
                    }
                }
            }

            dependencies = new List<IResourceLocation>(locationHashes);
        }
    }
}
