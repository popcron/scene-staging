using UnityEngine.VFX;

namespace Popcron.SceneStaging
{
    public class VisualEffectProcessor : ComponentProcessor<VisualEffect>
    {
        protected override void Save(Component mapObject, VisualEffect visualEffect)
        {
            mapObject.Set(nameof(visualEffect.initialEventID), visualEffect.initialEventID);
            mapObject.Set(nameof(visualEffect.initialEventName), visualEffect.initialEventName);
            mapObject.Set(nameof(visualEffect.pause), visualEffect.pause);
            mapObject.Set(nameof(visualEffect.playRate), visualEffect.playRate);
            mapObject.Set(nameof(visualEffect.resetSeedOnPlay), visualEffect.resetSeedOnPlay);
            mapObject.Set(nameof(visualEffect.startSeed), visualEffect.startSeed);
            mapObject.Set(nameof(visualEffect.visualEffectAsset), visualEffect.visualEffectAsset);
        }

        protected override void Load(Component mapObject, VisualEffect visualEffect)
        {
            visualEffect.initialEventID = mapObject.Get<int>(nameof(visualEffect.initialEventID));
            visualEffect.initialEventName = mapObject.Get<string>(nameof(visualEffect.initialEventName));
            visualEffect.pause = mapObject.Get<bool>(nameof(visualEffect.pause));
            visualEffect.playRate = mapObject.Get<float>(nameof(visualEffect.playRate));
            visualEffect.resetSeedOnPlay = mapObject.Get<bool>(nameof(visualEffect.resetSeedOnPlay));
            visualEffect.startSeed = mapObject.Get<uint>(nameof(visualEffect.startSeed));
            visualEffect.visualEffectAsset = mapObject.Get<VisualEffectAsset>(nameof(visualEffect.visualEffectAsset));
        }
    }
}