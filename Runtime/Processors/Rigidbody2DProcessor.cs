using UnityEngine;

namespace Popcron.SceneStaging
{
    public class Rigidbody2DProcessor : ComponentProcessor<Rigidbody2D>
    {
        protected override void Save(Component component, Rigidbody2D rigidbody)
        {
            component.Add(nameof(rigidbody.angularDrag), rigidbody.angularDrag);
            component.Add(nameof(rigidbody.collisionDetectionMode), rigidbody.collisionDetectionMode);
            component.Add(nameof(rigidbody.constraints), rigidbody.constraints);
            component.Add(nameof(rigidbody.drag), rigidbody.drag);
            component.Add(nameof(rigidbody.freezeRotation), rigidbody.freezeRotation);
            component.Add(nameof(rigidbody.interpolation), rigidbody.interpolation);
            component.Add(nameof(rigidbody.isKinematic), rigidbody.isKinematic);
            component.Add(nameof(rigidbody.mass), rigidbody.mass);
            component.Add(nameof(rigidbody.gravityScale), rigidbody.gravityScale);
        }

        protected override void Load(Component component, Rigidbody2D rigidbody)
        {
            rigidbody.angularDrag = component.Get<float>(nameof(rigidbody.angularDrag));
            rigidbody.collisionDetectionMode = component.Get<CollisionDetectionMode2D>(nameof(rigidbody.collisionDetectionMode));
            rigidbody.constraints = component.Get<RigidbodyConstraints2D>(nameof(rigidbody.constraints));
            rigidbody.drag = component.Get<float>(nameof(rigidbody.drag));
            rigidbody.freezeRotation = component.Get<bool>(nameof(rigidbody.freezeRotation));
            rigidbody.interpolation = component.Get<RigidbodyInterpolation2D>(nameof(rigidbody.interpolation));
            rigidbody.isKinematic = component.Get<bool>(nameof(rigidbody.isKinematic));
            rigidbody.mass = component.Get<float>(nameof(rigidbody.mass));
            rigidbody.gravityScale = component.Get<float>(nameof(rigidbody.gravityScale));
        }
    }
}