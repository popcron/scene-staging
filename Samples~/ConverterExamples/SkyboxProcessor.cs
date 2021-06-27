using UnityEngine;

namespace Popcron.SceneStaging
{
    public class SkyboxProcessor : ComponentProcessor<Skybox>
    {
        [RuntimeInitializeOnLoadMethod]
        private static void AutoRegister()
        {
            RegisterProcessor<SkyboxProcessor>();
        }

        protected override void Save(Component mapObject, Skybox skybox)
        {
            mapObject.Set(nameof(skybox.material), skybox.material);
        }

        protected override void Load(Component mapObject, Skybox skybox)
        {
            skybox.material = mapObject.Get<Material>(nameof(skybox.material));
        }
    }
}