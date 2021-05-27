using UnityEngine;

namespace Popcron.SceneStaging
{
    public class BoxColliderProcessor : ComponentProcessor<BoxCollider>
    {
        protected override void Save(Component component, BoxCollider boxCollider)
        {
            component.Add(nameof(BoxCollider.size), boxCollider.size);
            component.Add(nameof(BoxCollider.center), boxCollider.center);
            component.Add(nameof(BoxCollider.isTrigger), boxCollider.isTrigger);
        }

        protected override void Load(Component component, BoxCollider boxCollider)
        {
            boxCollider.size = component.Get<Vector3>(nameof(BoxCollider.size));
            boxCollider.center = component.Get<Vector3>(nameof(BoxCollider.center));
            boxCollider.isTrigger = component.Get<bool>(nameof(BoxCollider.isTrigger));
        }
    }
}