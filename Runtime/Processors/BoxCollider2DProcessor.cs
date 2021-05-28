using UnityEngine;

namespace Popcron.SceneStaging
{
    public class BoxCollider2DProcessor : ComponentProcessor<BoxCollider2D>
    {
        protected override void Save(Component component, BoxCollider2D boxCollider)
        {
            component.Set(nameof(boxCollider.autoTiling), boxCollider.autoTiling);
            component.Set(nameof(boxCollider.edgeRadius), boxCollider.edgeRadius);
            component.Set(nameof(boxCollider.size), boxCollider.size);
            component.Set(nameof(boxCollider.density), boxCollider.density);
            component.Set(nameof(boxCollider.isTrigger), boxCollider.isTrigger);
            component.Set(nameof(boxCollider.offset), boxCollider.offset);
            component.Set(nameof(boxCollider.sharedMaterial), boxCollider.sharedMaterial);
            component.Set(nameof(boxCollider.usedByComposite), boxCollider.usedByComposite);
            component.Set(nameof(boxCollider.usedByEffector), boxCollider.usedByEffector);
        }

        protected override void Load(Component component, BoxCollider2D boxCollider)
        {
            boxCollider.autoTiling = component.Get<bool>(nameof(boxCollider.autoTiling));
            boxCollider.edgeRadius = component.Get<float>(nameof(boxCollider.edgeRadius));
            boxCollider.size = component.Get<Vector2>(nameof(boxCollider.size));
            boxCollider.density = component.Get<float>(nameof(boxCollider.density));
            boxCollider.isTrigger = component.Get<bool>(nameof(boxCollider.isTrigger));
            boxCollider.offset = component.Get<Vector2>(nameof(boxCollider.offset));
            boxCollider.sharedMaterial = component.Get<PhysicsMaterial2D>(nameof(boxCollider.sharedMaterial));
            boxCollider.usedByComposite = component.Get<bool>(nameof(boxCollider.usedByComposite));
            boxCollider.usedByEffector = component.Get<bool>(nameof(boxCollider.usedByEffector));
        }
    }
}