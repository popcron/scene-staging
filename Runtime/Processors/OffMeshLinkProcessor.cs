using UnityEngine.AI;

namespace Popcron.SceneStaging
{
    public class OffMeshLinkProcessor : ComponentProcessor<OffMeshLink>
    {
        protected override void Save(Component mapObject, OffMeshLink offMeshLink)
        {
            mapObject.Add(nameof(offMeshLink.activated), offMeshLink.activated);
            mapObject.Add(nameof(offMeshLink.area), offMeshLink.area);
            mapObject.Add(nameof(offMeshLink.autoUpdatePositions), offMeshLink.autoUpdatePositions);
            mapObject.Add(nameof(offMeshLink.biDirectional), offMeshLink.biDirectional);
            mapObject.Add(nameof(offMeshLink.costOverride), offMeshLink.costOverride);
            mapObject.Add(nameof(offMeshLink.endTransform), Stage.GetProp(offMeshLink.endTransform)?.ID ?? -1);
            mapObject.Add(nameof(offMeshLink.startTransform), Stage.GetProp(offMeshLink.startTransform)?.ID ?? -1);
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