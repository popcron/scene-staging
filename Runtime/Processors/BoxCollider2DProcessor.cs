using UnityEngine;

namespace Popcron.SceneStaging
{
    public class BoxCollider2DProcessor : ComponentProcessor<BoxCollider2D>
    {
        protected override void Save(Component component, BoxCollider2D boxCollider)
        {
            component.Add(nameof(boxCollider.autoTiling), boxCollider.autoTiling);
            component.Add(nameof(boxCollider.edgeRadius), boxCollider.edgeRadius);
            component.Add(nameof(boxCollider.size), boxCollider.size);
            component.Add(nameof(boxCollider.density), boxCollider.density);
            component.Add(nameof(boxCollider.isTrigger), boxCollider.isTrigger);
            component.Add(nameof(boxCollider.offset), boxCollider.offset);
            component.Add(nameof(boxCollider.sharedMaterial), boxCollider.sharedMaterial);
            component.Add(nameof(boxCollider.usedByComposite), boxCollider.usedByComposite);
            component.Add(nameof(boxCollider.usedByEffector), boxCollider.usedByEffector);
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