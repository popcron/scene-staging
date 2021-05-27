using UnityEngine;

namespace Popcron.SceneStaging
{
    public class GameObjectProcessor : ComponentProcessor<GameObject>
    {
        protected override void Save(Component mapObject, GameObject gameObject)
        {
            mapObject.Add(nameof(gameObject.layer), gameObject.layer);
            mapObject.Add(nameof(gameObject.isStatic), gameObject.isStatic);
            mapObject.Add(nameof(gameObject.tag), gameObject.tag);
        }

        protected override void Load(Component mapObject, GameObject gameObject)
        {
            gameObject.layer = mapObject.Get<int>(nameof(gameObject.layer));
            gameObject.isStatic = mapObject.Get<bool>(nameof(gameObject.isStatic));
            gameObject.tag = mapObject.Get<string>(nameof(gameObject.tag));
        }
    }
}