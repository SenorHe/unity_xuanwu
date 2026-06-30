using System;
using GameFramework.Resource;
using UnityEngine;
using UnityGameFramework.Runtime;
using Object = UnityEngine.Object;

namespace AegisFlow.Resource
{
    /// <summary>
    /// Bridges AegisFlow asset requests to the UGF resource component.
    /// </summary>
    public sealed class UGFResourceAdapter : IAssetLoader
    {
        private readonly ResourceComponent m_ResourceComponent;

        public UGFResourceAdapter(ResourceComponent resourceComponent)
        {
            m_ResourceComponent = resourceComponent;
        }

        public void LoadAsync<T>(
            string assetPath,
            Action<T> onSuccess,
            Action<string> onFailure = null,
            Action<float> onProgress = null)
            where T : Object
        {
            if (m_ResourceComponent == null)
            {
                onFailure?.Invoke("UGF ResourceComponent is unavailable.");
                return;
            }

            if (string.IsNullOrWhiteSpace(assetPath))
            {
                onFailure?.Invoke("Asset path is empty.");
                return;
            }

            LoadAssetCallbacks callbacks = new LoadAssetCallbacks(
                (assetName, asset, duration, userData) =>
                {
                    if (asset is T typedAsset)
                    {
                        onSuccess?.Invoke(typedAsset);
                        return;
                    }

                    onFailure?.Invoke($"Loaded asset has an unexpected type: {assetName}");
                },
                (assetName, status, errorMessage, userData) =>
                    onFailure?.Invoke($"{assetName} ({status}): {errorMessage}"),
                (assetName, progress, userData) => onProgress?.Invoke(progress));

            m_ResourceComponent.LoadAsset(assetPath, typeof(T), callbacks);
        }

        public void Unload(Object asset)
        {
            if (asset != null)
            {
                m_ResourceComponent?.UnloadAsset(asset);
            }
        }
    }
}
