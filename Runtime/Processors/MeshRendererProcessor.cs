using UnityEngine;
using UnityEngine.Rendering;

namespace Popcron.SceneStaging
{
    public class MeshRendererProcessor : ComponentProcessor<MeshRenderer>
    {
        protected override void Save(Component mapObject, MeshRenderer meshRenderer)
        {
            mapObject.Add(nameof(meshRenderer.sharedMaterial), meshRenderer.sharedMaterial);
            mapObject.Add(nameof(meshRenderer.sharedMaterials), meshRenderer.sharedMaterials);
            mapObject.Add(nameof(meshRenderer.receiveShadows), meshRenderer.receiveShadows);
            mapObject.Add(nameof(meshRenderer.shadowCastingMode), meshRenderer.shadowCastingMode);
            mapObject.Add(nameof(meshRenderer.sortingLayerID), meshRenderer.sortingLayerID);
            mapObject.Add(nameof(meshRenderer.sortingLayerName), meshRenderer.sortingLayerName);
            mapObject.Add(nameof(meshRenderer.sortingOrder), meshRenderer.sortingOrder);
        }

        protected override void Load(Component mapObject, MeshRenderer meshRenderer)
        {
            meshRenderer.sharedMaterial = mapObject.Get<Material>(nameof(meshRenderer.sharedMaterial));
            meshRenderer.sharedMaterials = mapObject.Get<Material[]>(nameof(meshRenderer.sharedMaterials));
            meshRenderer.receiveShadows = mapObject.Get<bool>(nameof(meshRenderer.receiveShadows));
            meshRenderer.shadowCastingMode = mapObject.Get<ShadowCastingMode>(nameof(meshRenderer.shadowCastingMode));
            meshRenderer.sortingLayerID = mapObject.Get<int>(nameof(meshRenderer.sortingLayerID));
            meshRenderer.sortingLayerName = mapObject.Get<string>(nameof(meshRenderer.sortingLayerName));
            meshRenderer.sortingOrder = mapObject.Get<int>(nameof(meshRenderer.sortingOrder));
        }
    }
}