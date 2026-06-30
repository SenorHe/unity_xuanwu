using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AegisFlow.Resource
{
    /// <summary>
    /// Temporary fallback while assets are being migrated to UGF.
    /// </summary>
    public sealed class ResourcesLegacyAdapter : IAssetLoader
    {
        public void LoadAsync<T>(
            string assetPath,
            Action<T> onSuccess,
            Action<string> onFailure = null,
            Action<float> onProgress = null)
            where T : Object
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                onFailure?.Invoke("Asset path is empty.");
                return;
            }

            T asset = Resources.Load<T>(assetPath);

            if (asset == null)
            {
                onFailure?.Invoke($"Asset not found in Resources: {assetPath}");
                return;
            }

            onProgress?.Invoke(1f);
            onSuccess?.Invoke(asset);
        }

        public void Unload(Object asset)
        {
            if (asset != null && !(asset is GameObject))
            {
                Resources.UnloadAsset(asset);
            }
        }
    }
}
