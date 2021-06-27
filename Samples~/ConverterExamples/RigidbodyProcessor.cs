using UnityEngine;

namespace Popcron.SceneStaging
{
    public class RigidbodyProcessor : ComponentProcessor<Rigidbody>
    {
        [RuntimeInitializeOnLoadMethod]
        private static void AutoRegister()
        {
            RegisterProcessor<RigidbodyProcessor>();
        }

        protected override void Save(Component component, Rigidbody rigidbody)
        {
            component.Set(nameof(rigidbody.angularDrag), rigidbody.angularDrag);
            component.Set(nameof(rigidbody.collisionDetectionMode), rigidbody.collisionDetectionMode);
            component.Set(nameof(rigidbody.constraints), rigidbody.constraints);
            component.Set(nameof(rigidbody.detectCollisions), rigidbody.detectCollisions);
            component.Set(nameof(rigidbody.drag), rigidbody.drag);
            component.Set(nameof(rigidbody.freezeRotation), rigidbody.freezeRotation);
            component.Set(nameof(rigidbody.interpolation), rigidbody.interpolation);
            component.Set(nameof(rigidbody.isKinematic), rigidbody.isKinematic);
            component.Set(nameof(rigidbody.mass), rigidbody.mass);
            component.Set(nameof(rigidbody.useGravity), rigidbody.useGravity);
        }

        protected override void Load(Component component, Rigidbody rigidbody)
        {
            rigidbody.angularDrag = component.Get<float>(nameof(rigidbody.angularDrag));
            rigidbody.collisionDetectionMode = component.Get<CollisionDetectionMode>(nameof(rigidbody.collisionDetectionMode));
            rigidbody.constraints = component.Get<RigidbodyConstraints>(nameof(rigidbody.constraints));
            rigidbody.detectCollisions = component.Get<bool>(nameof(rigidbody.detectCollisions));
            rigidbody.drag = component.Get<float>(nameof(rigidbody.drag));
            rigidbody.freezeRotation = component.Get<bool>(nameof(rigidbody.freezeRotation));
            rigidbody.interpolation = component.Get<RigidbodyInterpolation>(nameof(rigidbody.interpolation));
            rigidbody.isKinematic = component.Get<bool>(nameof(rigidbody.isKinematic));
            rigidbody.mass = component.Get<float>(nameof(rigidbody.mass));
            rigidbody.useGravity = component.Get<bool>(nameof(rigidbody.useGravity));
        }
    }
}