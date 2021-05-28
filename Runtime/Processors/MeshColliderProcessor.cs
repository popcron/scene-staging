using UnityEngine;

namespace Popcron.SceneStaging
{
    public class MeshColliderProcessor : ComponentProcessor<MeshCollider>
    {
        protected override void Save(Component mapObject, MeshCollider meshCollider)
        {
            mapObject.Set(nameof(meshCollider.sharedMesh), meshCollider.sharedMesh);
            mapObject.Set(nameof(meshCollider.convex), meshCollider.convex);
            mapObject.Set(nameof(meshCollider.isTrigger), meshCollider.isTrigger);
        }

        protected override void Load(Component mapObject, MeshCollider meshCollider)
        {
            meshCollider.sharedMesh = mapObject.Get<Mesh>(nameof(meshCollider.sharedMesh));
            meshCollider.convex = mapObject.Get<bool>(nameof(meshCollider.convex));
            meshCollider.isTrigger = mapObject.Get<bool>(nameof(meshCollider.isTrigger));
        }
    }
}