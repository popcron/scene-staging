using UnityEngine;

namespace Popcron.SceneStaging
{
    public class SphereColliderProcessor : ComponentProcessor<SphereCollider>
    {
        protected override void Save(Component mapObject, SphereCollider sphereCollider)
        {
            mapObject.Add(nameof(sphereCollider.radius), sphereCollider.radius);
            mapObject.Add(nameof(sphereCollider.center), sphereCollider.center);
            mapObject.Add(nameof(sphereCollider.isTrigger), sphereCollider.isTrigger);
        }

        protected override void Load(Component mapObject, SphereCollider sphereCollider)
        {
            sphereCollider.radius = mapObject.Get<float>(nameof(sphereCollider.radius));
            sphereCollider.center = mapObject.Get<Vector3>(nameof(sphereCollider.center));
            sphereCollider.isTrigger = mapObject.Get<bool>(nameof(sphereCollider.isTrigger));
        }
    }
}