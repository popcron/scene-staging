using UnityEngine.AI;

namespace Popcron.SceneStaging
{
    public class OffMeshLinkProcessor : ComponentProcessor<OffMeshLink>
    {
        protected override void Save(Component mapObject, OffMeshLink offMeshLink)
        {
            mapObject.Set(nameof(offMeshLink.activated), offMeshLink.activated);
            mapObject.Set(nameof(offMeshLink.area), offMeshLink.area);
            mapObject.Set(nameof(offMeshLink.autoUpdatePositions), offMeshLink.autoUpdatePositions);
            mapObject.Set(nameof(offMeshLink.biDirectional), offMeshLink.biDirectional);
            mapObject.Set(nameof(offMeshLink.costOverride), offMeshLink.costOverride);
            mapObject.Set(nameof(offMeshLink.endTransform), Stage.GetProp(offMeshLink.endTransform)?.ID ?? -1);
            mapObject.Set(nameof(offMeshLink.startTransform), Stage.GetProp(offMeshLink.startTransform)?.ID ?? -1);
        }

        protected override void Load(Component mapObject, OffMeshLink navMeshLink)
        {
            navMeshLink.activated = mapObject.Get<bool>(nameof(navMeshLink.activated));
            navMeshLink.area = mapObject.Get<int>(nameof(navMeshLink.area));
            navMeshLink.autoUpdatePositions = mapObject.Get<bool>(nameof(navMeshLink.autoUpdatePositions));
            navMeshLink.biDirectional = mapObject.Get<bool>(nameof(navMeshLink.biDirectional));
            navMeshLink.costOverride = mapObject.Get<float>(nameof(navMeshLink.costOverride));
            navMeshLink.endTransform = Stage.GetProp(mapObject.Get<int>(nameof(navMeshLink.endTransform)))?.Transform;
            navMeshLink.startTransform = Stage.GetProp(mapObject.Get<int>(nameof(navMeshLink.startTransform)))?.Transform;
        }
    }
}