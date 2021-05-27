using UnityEngine.VFX;

namespace Popcron.SceneStaging
{
    public class VisualEffectProcessor : ComponentProcessor<VisualEffect>
    {
        protected override void Save(Component mapObject, VisualEffect visualEffect)
        {
            mapObject.Add(nameof(visualEffect.initialEventID), visualEffect.initialEventID);
            mapObject.Add(nameof(visualEffect.initialEventName), visualEffect.initialEventName);
            mapObject.Add(nameof(visualEffect.pause), visualEffect.pause);
            mapObject.Add(nameof(visualEffect.playRate), visualEffect.playRate);
            mapObject.Add(nameof(visualEffect.resetSeedOnPlay), visualEffect.resetSeedOnPlay);
            mapObject.Add(nameof(visualEffect.startSeed), visualEffect.startSeed);
            mapObject.Add(nameof(visualEffect.visualEffectAsset), visualEffect.visualEffectAsset);
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