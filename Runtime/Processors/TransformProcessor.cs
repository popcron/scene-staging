using UnityEngine;

namespace Popcron.SceneStaging
{
    public class TransformProcessor : ComponentProcessor<Transform>
    {
        protected override void Save(Component mapObject, Transform transform)
        {
            mapObject.Set(nameof(transform.localPosition), transform.localPosition);
            mapObject.Set(nameof(transform.localEulerAngles), transform.localEulerAngles);
            mapObject.Set(nameof(transform.localScale), transform.localScale);
        }

        protected override void Load(Component mapObject, Transform transform)
        {
            transform.localPosition = mapObject.Get<Vector3>(nameof(transform.localPosition));
            transform.localEulerAngles = mapObject.Get<Vector3>(nameof(transform.localEulerAngles));
            transform.localScale = mapObject.Get<Vector3>(nameof(transform.localScale));
        }
    }
}