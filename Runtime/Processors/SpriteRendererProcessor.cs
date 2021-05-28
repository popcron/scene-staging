using UnityEngine;
using UnityEngine.Rendering;

namespace Popcron.SceneStaging
{
    public class SpriteRendererProcessor : ComponentProcessor<SpriteRenderer>
    {
        protected override void Save(Component mapObject, SpriteRenderer spriteRenderer)
        {
            mapObject.Set(nameof(spriteRenderer.sprite), spriteRenderer.sprite);
            mapObject.Set(nameof(spriteRenderer.flipX), spriteRenderer.flipX);
            mapObject.Set(nameof(spriteRenderer.flipY), spriteRenderer.flipY);
            mapObject.Set(nameof(spriteRenderer.color), spriteRenderer.color);
            mapObject.Set(nameof(spriteRenderer.drawMode), spriteRenderer.drawMode);
            mapObject.Set(nameof(spriteRenderer.adaptiveModeThreshold), spriteRenderer.adaptiveModeThreshold);
            mapObject.Set(nameof(spriteRenderer.spriteSortPoint), spriteRenderer.spriteSortPoint);
            mapObject.Set(nameof(spriteRenderer.tileMode), spriteRenderer.tileMode);
            mapObject.Set(nameof(spriteRenderer.size), spriteRenderer.size);
            mapObject.Set(nameof(spriteRenderer.maskInteraction), spriteRenderer.maskInteraction);

            mapObject.Set(nameof(spriteRenderer.sharedMaterial), spriteRenderer.sharedMaterial);
            mapObject.Set(nameof(spriteRenderer.sharedMaterials), spriteRenderer.sharedMaterials);
            mapObject.Set(nameof(spriteRenderer.receiveShadows), spriteRenderer.receiveShadows);
            mapObject.Set(nameof(spriteRenderer.shadowCastingMode), spriteRenderer.shadowCastingMode);
            mapObject.Set(nameof(spriteRenderer.sortingLayerID), spriteRenderer.sortingLayerID);
            mapObject.Set(nameof(spriteRenderer.sortingLayerName), spriteRenderer.sortingLayerName);
            mapObject.Set(nameof(spriteRenderer.sortingOrder), spriteRenderer.sortingOrder);
        }

        protected override void Load(Component mapObject, SpriteRenderer spriteRenderer)
        {
            spriteRenderer.sprite = mapObject.Get<Sprite>(nameof(spriteRenderer.sprite));
            spriteRenderer.flipX = mapObject.Get<bool>(nameof(spriteRenderer.flipX));
            spriteRenderer.flipY = mapObject.Get<bool>(nameof(spriteRenderer.flipY));
            spriteRenderer.color = mapObject.Get<Color>(nameof(spriteRenderer.color));
            spriteRenderer.drawMode = mapObject.Get<SpriteDrawMode>(nameof(spriteRenderer.drawMode));
            spriteRenderer.adaptiveModeThreshold = mapObject.Get<float>(nameof(spriteRenderer.adaptiveModeThreshold));
            spriteRenderer.spriteSortPoint = mapObject.Get<SpriteSortPoint>(nameof(spriteRenderer.spriteSortPoint));
            spriteRenderer.tileMode = mapObject.Get<SpriteTileMode>(nameof(spriteRenderer.tileMode));
            spriteRenderer.size = mapObject.Get<Vector2>(nameof(spriteRenderer.size));
            spriteRenderer.maskInteraction = mapObject.Get<SpriteMaskInteraction>(nameof(spriteRenderer.maskInteraction));

            spriteRenderer.sharedMaterial = mapObject.Get<Material>(nameof(spriteRenderer.sharedMaterial));
            spriteRenderer.sharedMaterials = mapObject.Get<Material[]>(nameof(spriteRenderer.sharedMaterials));
            spriteRenderer.receiveShadows = mapObject.Get<bool>(nameof(spriteRenderer.receiveShadows));
            spriteRenderer.shadowCastingMode = mapObject.Get<ShadowCastingMode>(nameof(spriteRenderer.shadowCastingMode));
            spriteRenderer.sortingLayerID = mapObject.Get<int>(nameof(spriteRenderer.sortingLayerID));
            spriteRenderer.sortingLayerName = mapObject.Get<string>(nameof(spriteRenderer.sortingLayerName));
            spriteRenderer.sortingOrder = mapObject.Get<int>(nameof(spriteRenderer.sortingOrder));
        }
    }
}