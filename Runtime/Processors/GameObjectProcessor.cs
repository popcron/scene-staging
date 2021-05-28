using UnityEngine;

namespace Popcron.SceneStaging
{
    public class GameObjectProcessor : ComponentProcessor<GameObject>
    {
        protected override void Save(Component mapObject, GameObject gameObject)
        {
            mapObject.Set(nameof(gameObject.name), gameObject.name);
            mapObject.Set(nameof(gameObject.layer), gameObject.layer);
            mapObject.Set(nameof(gameObject.isStatic), gameObject.isStatic);
            mapObject.Set(nameof(gameObject.tag), gameObject.tag);
        }

        protected override void Load(Component mapObject, GameObject gameObject)
        {
            gameObject.name = mapObject.Get<string>(nameof(gameObject.name));
            gameObject.layer = mapObject.Get<int>(nameof(gameObject.layer));
            gameObject.isStatic = mapObject.Get<bool>(nameof(gameObject.isStatic));
            gameObject.tag = mapObject.Get<string>(nameof(gameObject.tag));
        }
    }
}