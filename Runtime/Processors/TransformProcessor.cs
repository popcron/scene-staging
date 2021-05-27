using UnityEngine;

namespace Popcron.SceneStaging
{
    public class TransformProcessor : ComponentProcessor<Transform>
    {
        protected override void Save(Component mapObject, Transform transform)
        {
            mapObject.Add(nameof(transform.localPosition), transform.localPosition);
            mapObject.Add(nameof(transform.localEulerAngles), transform.localEulerAngles);
            mapObject.Add(nameof(transform.localScale), transform.localScale);
        }

        protected override void Load(Component mapObject, Transform transform)
        {
            transform.localPosition = mapObject.Get<Vector3>(nameof(transform.localPosition));
            transform.localEulerAngles = mapObject.Get<Vector3>(nameof(transform.localEulerAngles));
            transform.localScale = mapObject.Get<Vector3>(nameof(transform.localScale));
        }
    }
}