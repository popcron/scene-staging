using UnityEngine;

namespace Popcron.SceneStaging
{
    [CreateAssetMenu(menuName = "Stage")]
    public class StageAsset : ScriptableObject
    {
        [SerializeField]
        private Stage stage;

        /// <summary>
        /// A copy of the stage in this asset.
        /// </summary>
        public Stage Stage
        {
            get => stage.Clone();
            set => stage = value.Clone();
        }

        public string DisplayName => stage.DisplayName;
        public string ID => stage.ID;

        /// <summary>
        /// Creates a new stage asset with this stage object.
        /// </summary>
        public static StageAsset Create(Stage stage)
        {
            StageAsset asset = CreateInstance<StageAsset>();
            asset.stage = stage;
            asset.name = stage.DisplayName;
            return asset;
        }

        /// <summary>
        /// Creates a new stage asset with no stage assigned.
        /// </summary>
        public static StageAsset Create(string name)
        {
            StageAsset asset = CreateInstance<StageAsset>();
            asset.stage = null;
            asset.name = name;
            return asset;
        }
    }
}