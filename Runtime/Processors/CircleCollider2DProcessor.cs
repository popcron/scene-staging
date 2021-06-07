using UnityEngine;

namespace Popcron.SceneStaging
{
    public class CircleCollider2DProcessor : ComponentProcessor<CircleCollider2D>
    {
        protected override void Save(Component component, CircleCollider2D circleCollider)
        {
            component.Set(nameof(circleCollider.radius), circleCollider.radius);
            
            Rigidbody2D rb = circleCollider.attachedRigidbody;
            if (rb && rb.useAutoMass)
            {
                component.Set(nameof(circleCollider.density), circleCollider.density);
            }

            component.Set(nameof(circleCollider.isTrigger), circleCollider.isTrigger);
            component.Set(nameof(circleCollider.offset), circleCollider.offset);
            component.Set(nameof(circleCollider.sharedMaterial), circleCollider.sharedMaterial);
            component.Set(nameof(circleCollider.usedByEffector), circleCollider.usedByEffector);
        }

        protected override void Load(Component component, CircleCollider2D circleCollider)
        {
            circleCollider.radius = component.Get<float>(nameof(circleCollider.radius));

            Rigidbody2D rb = circleCollider.attachedRigidbody;
            if (rb && rb.useAutoMass)
            {
                circleCollider.density = component.Get<float>(nameof(circleCollider.density));
            }

            circleCollider.isTrigger = component.Get<bool>(nameof(circleCollider.isTrigger));
            circleCollider.offset = component.Get<Vector2>(nameof(circleCollider.offset));
            circleCollider.sharedMaterial = component.Get<PhysicsMaterial2D>(nameof(circleCollider.sharedMaterial));
            circleCollider.usedByEffector = component.Get<bool>(nameof(circleCollider.usedByEffector));
        }
    }
}