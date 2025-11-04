using UnityEngine;

namespace _01.Scripts.Utils
{
    public static class LayerChanger
    {
        /// <summary>
        /// Sets the layer of a GameObject and all its children recursively.
        /// </summary>
        /// <param name="parentGameObject">The parent GameObject whose layer and its children's layers will be set.</param>
        /// <param name="newLayerName">The name of the layer to assign.</param>
        public static void SetLayerRecursively(GameObject parentGameObject, string newLayerName)
        {
            if (parentGameObject == null)
            {
                Debug.LogWarning("Parent GameObject is null. Cannot set layer recursively.");
                return;
            }

            int newLayer = LayerMask.NameToLayer(newLayerName);

            if (newLayer == -1)
            {
                Debug.LogError($"Layer '{newLayerName}' does not exist. Please ensure the layer is defined in Unity's Tag and Layer Manager.");
                return;
            }

            parentGameObject.layer = newLayer;

            foreach (Transform child in parentGameObject.transform)
            {
                SetLayerRecursively(child.gameObject, newLayerName);
            }
        }

        /// <summary>
        /// Sets the layer of a GameObject and all its children recursively using a layer index.
        /// </summary>
        /// <param name="parentGameObject">The parent GameObject whose layer and its children's layers will be set.</param>
        /// <param name="newLayerIndex">The integer index of the layer to assign.</param>
        public static void SetLayerRecursively(GameObject parentGameObject, int newLayerIndex)
        {
            if (parentGameObject == null)
            {
                Debug.LogWarning("Parent GameObject is null. Cannot set layer recursively.");
                return;
            }

            parentGameObject.layer = newLayerIndex;

            foreach (Transform child in parentGameObject.transform)
            {
                SetLayerRecursively(child.gameObject, newLayerIndex);
            }
        }
    }
}