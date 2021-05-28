using UnityEngine;

namespace Popcron.SceneStaging
{
    public class CapsuleColliderProcessor : ComponentProcessor<CapsuleCollider>
    {
        protected override void Save(Component component, CapsuleCollider capsuleCollider)
        {
            component.Set(nameof(capsuleCollider.radius), capsuleCollider.radius);
            component.Set(nameof(capsuleCollider.height), capsuleCollider.height);
            component.Set(nameof(capsuleCollider.direction), capsuleCollider.direction);
            component.Set(nameof(capsuleCollider.center), capsuleCollider.center);
            component.Set(nameof(capsuleCollider.isTrigger), capsuleCollider.isTrigger);
        }

        protected override void Load(Component component, CapsuleCollider capsuleCollider)
        {
            capsuleCollider.radius = component.Get<float>(nameof(capsuleCollider.radius));
            capsuleCollider.height = component.Get<float>(nameof(capsuleCollider.height));
            capsuleCollider.direction = component.Get<int>(nameof(capsuleCollider.direction));
            capsuleCollider.center = component.Get<Vector3>(nameof(capsuleCollider.center));
            capsuleCollider.isTrigger = component.Get<bool>(nameof(capsuleCollider.isTrigger));
        }
    }
}