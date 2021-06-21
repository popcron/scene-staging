using UnityEngine;
using UnityEngine.Rendering;

namespace Popcron.SceneStaging
{
    public class MeshRendererProcessor : ComponentProcessor<MeshRenderer>
    {
        protected override void Save(Component mapObject, MeshRenderer meshRenderer)
        {
            mapObject.Set(nameof(meshRenderer.sharedMaterial), meshRenderer.sharedMaterial);
            mapObject.Set(nameof(meshRenderer.sharedMaterials), meshRenderer.sharedMaterials);
            mapObject.Set(nameof(meshRenderer.receiveShadows), meshRenderer.receiveShadows);
            mapObject.Set(nameof(meshRenderer.shadowCastingMode), meshRenderer.shadowCastingMode);
            mapObject.Set(nameof(meshRenderer.sortingLayerID), meshRenderer.sortingLayerID);
            mapObject.Set(nameof(meshRenderer.sortingLayerName), meshRenderer.sortingLayerName);
            mapObject.Set(nameof(meshRenderer.sortingOrder), meshRenderer.sortingOrder);
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