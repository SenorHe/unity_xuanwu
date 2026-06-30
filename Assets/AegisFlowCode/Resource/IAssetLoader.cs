using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AegisFlow.Resource
{
    public interface IAssetLoader
    {
        void LoadAsync<T>(
            string assetPath,
            Action<T> onSuccess,
            Action<string> onFailure = null,
            Action<float> onProgress = null)
            where T : Object;

        void Unload(Object asset);
    }
}
