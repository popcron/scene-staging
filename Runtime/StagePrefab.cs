using UnityEngine;

namespace Popcron.SceneStaging
{
    public class StagePrefab : MonoBehaviour
    {
        [SerializeField]
        private string contextMenu = null;

        public string ContextMenu => contextMenu;
    }
}
