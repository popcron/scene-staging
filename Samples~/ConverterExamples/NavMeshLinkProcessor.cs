using System.Reflection;
using UnityEngine;
using UnityEngine.AI;

namespace Popcron.SceneStaging
{
#if UNITY_NAVMESHCOMPONENTS
    public class NavMeshLinkProcessor : ComponentProcessor<NavMeshLink>
    {
        protected override void Save(Comp mapObject, NavMeshLink navMeshLink)
        {
            mapObject.Add(nameof(navMeshLink.agentTypeID), navMeshLink.agentTypeID);
            mapObject.Add(nameof(navMeshLink.area), navMeshLink.area);
            mapObject.Add(nameof(navMeshLink.autoUpdate), navMeshLink.autoUpdate);
            mapObject.Add(nameof(navMeshLink.bidirectional), navMeshLink.bidirectional);
            mapObject.Add(nameof(navMeshLink.costModifier), navMeshLink.costModifier);
            mapObject.Add(nameof(navMeshLink.endPoint), navMeshLink.endPoint);
            mapObject.Add(nameof(navMeshLink.startPoint), navMeshLink.startPoint);
            mapObject.Add(nameof(navMeshLink.width), navMeshLink.width);
        }

        protected override void Load(Comp mapObject, NavMeshLink navMeshLink)
        {
            navMeshLink.agentTypeID = mapObject.Get<int>(nameof(navMeshLink.agentTypeID));
            navMeshLink.area = mapObject.Get<int>(nameof(navMeshLink.area));
            navMeshLink.autoUpdate = mapObject.Get<bool>(nameof(navMeshLink.autoUpdate));
            navMeshLink.bidirectional = mapObject.Get<bool>(nameof(navMeshLink.bidirectional));
            navMeshLink.costModifier = mapObject.Get<int>(nameof(navMeshLink.costModifier));
            navMeshLink.endPoint = mapObject.Get<Vector3>(nameof(navMeshLink.endPoint));
            navMeshLink.startPoint = mapObject.Get<Vector3>(nameof(navMeshLink.startPoint));
            navMeshLink.width = mapObject.Get<float>(nameof(navMeshLink.width));

            FieldInfo m_LinkInstance = navMeshLink.GetType().GetField(nameof(m_LinkInstance), BindingFlags.NonPublic | BindingFlags.Instance);
            NavMeshLinkInstance linkInstance = (NavMeshLinkInstance)m_LinkInstance.GetValue(navMeshLink);
            linkInstance.Remove();
        }
    }
#endif
}