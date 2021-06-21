using UnityEngine;

namespace Popcron.SceneStaging
{
    public class AudioReverbZoneProcessor : ComponentProcessor<AudioReverbZone>
    {
        protected override void Save(Component component, AudioReverbZone audioReverbZone)
        {
            component.Set(nameof(AudioReverbZone.decayHFRatio), audioReverbZone.decayHFRatio);
            component.Set(nameof(AudioReverbZone.decayTime), audioReverbZone.decayTime);
            component.Set(nameof(AudioReverbZone.density), audioReverbZone.density);
            component.Set(nameof(AudioReverbZone.diffusion), audioReverbZone.diffusion);
            component.Set(nameof(AudioReverbZone.HFReference), audioReverbZone.HFReference);
            component.Set(nameof(AudioReverbZone.LFReference), audioReverbZone.LFReference);
            component.Set(nameof(AudioReverbZone.maxDistance), audioReverbZone.maxDistance);
            component.Set(nameof(AudioReverbZone.minDistance), audioReverbZone.minDistance);
            component.Set(nameof(AudioReverbZone.reflections), audioReverbZone.reflections);
            component.Set(nameof(AudioReverbZone.reflectionsDelay), audioReverbZone.reflectionsDelay);
            component.Set(nameof(AudioReverbZone.reverb), audioReverbZone.reverb);
            component.Set(nameof(AudioReverbZone.reverbDelay), audioReverbZone.reverbDelay);
            component.Set(nameof(AudioReverbZone.reverbPreset), audioReverbZone.reverbPreset);
            component.Set(nameof(AudioReverbZone.room), audioReverbZone.room);
            component.Set(nameof(AudioReverbZone.roomHF), audioReverbZone.roomHF);
            component.Set(nameof(AudioReverbZone.roomLF), audioReverbZone.roomLF);
        }

        protected override void Load(Component component, AudioReverbZone audioReverbZone)
        {
            audioReverbZone.decayHFRatio = component.Get<float>(nameof(AudioReverbZone.decayHFRatio));
            audioReverbZone.decayTime = component.Get<float>(nameof(AudioReverbZone.decayTime));
            audioReverbZone.density = component.Get<float>(nameof(AudioReverbZone.density));
            audioReverbZone.diffusion = component.Get<float>(nameof(AudioReverbZone.diffusion));
            audioReverbZone.HFReference = component.Get<float>(nameof(AudioReverbZone.HFReference));
            audioReverbZone.LFReference = component.Get<float>(nameof(AudioReverbZone.LFReference));
            audioReverbZone.maxDistance = component.Get<float>(nameof(AudioReverbZone.maxDistance));
            audioReverbZone.minDistance = component.Get<float>(nameof(AudioReverbZone.minDistance));
            audioReverbZone.reflections = component.Get<int>(nameof(AudioReverbZone.reflections));
            audioReverbZone.reflectionsDelay = component.Get<float>(nameof(AudioReverbZone.reflectionsDelay));
            audioReverbZone.reverb = component.Get<int>(nameof(AudioReverbZone.reverb));
            audioReverbZone.reverbDelay = component.Get<float>(nameof(AudioReverbZone.reverbDelay));
            audioReverbZone.reverbPreset = component.Get<AudioReverbPreset>(nameof(AudioReverbZone.reverbPreset));
            audioReverbZone.room = component.Get<int>(nameof(AudioReverbZone.room));
            audioReverbZone.roomHF = component.Get<int>(nameof(AudioReverbZone.roomHF));
            audioReverbZone.roomLF = component.Get<int>(nameof(AudioReverbZone.roomLF));
        }
    }
}