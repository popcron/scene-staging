using UnityEngine;

namespace Popcron.SceneStaging
{
    public class BoxColliderProcessor : ComponentProcessor<BoxCollider>
    {
        [RuntimeInitializeOnLoadMethod]
        private static void AutoRegister()
        {
            RegisterProcessor<BoxColliderProcessor>();
        }

        protected override void Save(Component component, BoxCollider boxCollider)
        {
            component.Set(nameof(BoxCollider.size), boxCollider.size);
            component.Set(nameof(BoxCollider.center), boxCollider.center);
            component.Set(nameof(BoxCollider.isTrigger), boxCollider.isTrigger);
        }

        protected override void Load(Component component, BoxCollider boxCollider)
        {
            boxCollider.size = component.Get<Vector3>(nameof(BoxCollider.size));
            boxCollider.center = component.Get<Vector3>(nameof(BoxCollider.center));
            boxCollider.isTrigger = component.Get<bool>(nameof(BoxCollider.isTrigger));
        }
    }
}