using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AegisFlow.Resource
{
    /// <summary>
    /// Stable application-facing entry point for asynchronous asset loading.
    /// </summary>
    public static class AssetService
    {
        private static IAssetLoader s_AssetLoader;

        public static void Initialize(IAssetLoader assetLoader)
        {
            s_AssetLoader = assetLoader;
        }

        public static void LoadAsync<T>(
            string assetPath,
            Action<T> onSuccess,
            Action<string> onFailure = null,
            Action<float> onProgress = null)
            where T : Object
        {
            if (s_AssetLoader == null)
            {
                const string message = "[AegisFlow] AssetService is not initialized.";
                Debug.LogError(message);
                onFailure?.Invoke(message);
                return;
            }

            s_AssetLoader.LoadAsync(assetPath, onSuccess, onFailure, onProgress);
        }

        public static void Unload(Object asset)
        {
            if (asset != null)
            {
                s_AssetLoader?.Unload(asset);
            }
        }

        public static void Shutdown()
        {
            s_AssetLoader = null;
        }
    }
}
