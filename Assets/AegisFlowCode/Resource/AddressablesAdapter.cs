using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AegisFlow.Resource
{
    public sealed class AddressablesAdapter : IAssetLoader
    {
        public void LoadAsync<T>(
            string assetPath,
            Action<T> onSuccess,
            Action<string> onFailure = null,
            Action<float> onProgress = null)
            where T : Object
        {
            const string message = "[AegisFlow] AddressablesAdapter is not implemented.";
            Debug.LogWarning(message);
            onFailure?.Invoke(message);
        }

        public void Unload(Object asset)
        {
        }
    }
}
