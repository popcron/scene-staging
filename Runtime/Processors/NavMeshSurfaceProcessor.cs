using UnityEngine;
using UnityEngine.AI;

namespace Popcron.SceneStaging
{
#if UNITY_NAVMESHCOMPONENTS
    public class NavMeshSurfaceProcessor : ComponentProcessor<NavMeshSurface>
    {
        protected override void Save(Comp mapObject, NavMeshSurface navMeshSurface)
        {
            mapObject.Add(nameof(navMeshSurface.agentTypeID), navMeshSurface.agentTypeID);
            mapObject.Add(nameof(navMeshSurface.center), navMeshSurface.center);
            mapObject.Add(nameof(navMeshSurface.collectObjects), navMeshSurface.collectObjects);
            mapObject.Add(nameof(navMeshSurface.defaultArea), navMeshSurface.defaultArea);
            mapObject.Add(nameof(navMeshSurface.ignoreNavMeshAgent), navMeshSurface.ignoreNavMeshAgent);
            mapObject.Add(nameof(navMeshSurface.ignoreNavMeshObstacle), navMeshSurface.ignoreNavMeshObstacle);

            NavMeshData navMeshData = navMeshSurface.navMeshData;
            string navMeshDataPath = ReferencesDatabase.GetPath(navMeshData);
            mapObject.Add(nameof(navMeshSurface.navMeshData), navMeshDataPath);

            mapObject.Add(nameof(navMeshSurface.overrideTileSize), navMeshSurface.overrideTileSize);
            mapObject.Add(nameof(navMeshSurface.overrideVoxelSize), navMeshSurface.overrideVoxelSize);
            mapObject.Add(nameof(navMeshSurface.size), navMeshSurface.size);
            mapObject.Add(nameof(navMeshSurface.tileSize), navMeshSurface.tileSize);
            mapObject.Add(nameof(navMeshSurface.useGeometry), navMeshSurface.useGeometry);
            mapObject.Add(nameof(navMeshSurface.voxelSize), navMeshSurface.voxelSize);
        }

        protected override void Load(Comp mapObject, NavMeshSurface navMeshSurface)
        { 
            navMeshSurface.agentTypeID = mapObject.Get<int>(nameof(navMeshSurface.agentTypeID));
            navMeshSurface.center = mapObject.Get<Vector3>(nameof(navMeshSurface.center));
            navMeshSurface.collectObjects = mapObject.Get<CollectObjects>(nameof(navMeshSurface.collectObjects));
            navMeshSurface.defaultArea = mapObject.Get<int>(nameof(navMeshSurface.defaultArea));
            navMeshSurface.ignoreNavMeshAgent = mapObject.Get<bool>(nameof(navMeshSurface.ignoreNavMeshAgent));
            navMeshSurface.ignoreNavMeshObstacle = mapObject.Get<bool>(nameof(navMeshSurface.ignoreNavMeshObstacle));

            string navMeshDataPath = mapObject.Get<string>(nameof(navMeshSurface.navMeshData));
            navMeshSurface.navMeshData = ReferencesDatabase.Get<NavMeshData>(navMeshDataPath);

            navMeshSurface.overrideTileSize = mapObject.Get<bool>(nameof(navMeshSurface.overrideTileSize));
            navMeshSurface.overrideVoxelSize = mapObject.Get<bool>(nameof(navMeshSurface.overrideVoxelSize));
            navMeshSurface.size = mapObject.Get<Vector3>(nameof(navMeshSurface.size));
            navMeshSurface.tileSize = mapObject.Get<int>(nameof(navMeshSurface.tileSize));
            navMeshSurface.useGeometry = mapObject.Get<NavMeshCollectGeometry>(nameof(navMeshSurface.useGeometry));
            navMeshSurface.voxelSize = mapObject.Get<float>(nameof(navMeshSurface.voxelSize));
        }
    }
#endif
}