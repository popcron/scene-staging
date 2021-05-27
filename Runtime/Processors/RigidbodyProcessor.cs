using UnityEngine;

namespace Popcron.SceneStaging
{
    public class RigidbodyProcessor : ComponentProcessor<Rigidbody>
    {
        protected override void Save(Component component, Rigidbody rigidbody)
        {
            component.Add(nameof(rigidbody.angularDrag), rigidbody.angularDrag);
            component.Add(nameof(rigidbody.collisionDetectionMode), rigidbody.collisionDetectionMode);
            component.Add(nameof(rigidbody.constraints), rigidbody.constraints);
            component.Add(nameof(rigidbody.detectCollisions), rigidbody.detectCollisions);
            component.Add(nameof(rigidbody.drag), rigidbody.drag);
            component.Add(nameof(rigidbody.freezeRotation), rigidbody.freezeRotation);
            component.Add(nameof(rigidbody.interpolation), rigidbody.interpolation);
            component.Add(nameof(rigidbody.isKinematic), rigidbody.isKinematic);
            component.Add(nameof(rigidbody.mass), rigidbody.mass);
            component.Add(nameof(rigidbody.useGravity), rigidbody.useGravity);
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