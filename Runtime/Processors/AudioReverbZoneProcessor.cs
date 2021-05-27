using UnityEngine;

namespace Popcron.SceneStaging
{
    public class AudioReverbZoneProcessor : ComponentProcessor<AudioReverbZone>
    {
        protected override void Save(Component component, AudioReverbZone audioReverbZone)
        {
            component.Add(nameof(AudioReverbZone.decayHFRatio), audioReverbZone.decayHFRatio);
            component.Add(nameof(AudioReverbZone.decayTime), audioReverbZone.decayTime);
            component.Add(nameof(AudioReverbZone.density), audioReverbZone.density);
            component.Add(nameof(AudioReverbZone.diffusion), audioReverbZone.diffusion);
            component.Add(nameof(AudioReverbZone.HFReference), audioReverbZone.HFReference);
            component.Add(nameof(AudioReverbZone.LFReference), audioReverbZone.LFReference);
            component.Add(nameof(AudioReverbZone.maxDistance), audioReverbZone.maxDistance);
            component.Add(nameof(AudioReverbZone.minDistance), audioReverbZone.minDistance);
            component.Add(nameof(AudioReverbZone.reflections), audioReverbZone.reflections);
            component.Add(nameof(AudioReverbZone.reflectionsDelay), audioReverbZone.reflectionsDelay);
            component.Add(nameof(AudioReverbZone.reverb), audioReverbZone.reverb);
            component.Add(nameof(AudioReverbZone.reverbDelay), audioReverbZone.reverbDelay);
            component.Add(nameof(AudioReverbZone.reverbPreset), audioReverbZone.reverbPreset);
            component.Add(nameof(AudioReverbZone.room), audioReverbZone.room);
            component.Add(nameof(AudioReverbZone.roomHF), audioReverbZone.roomHF);
            component.Add(nameof(AudioReverbZone.roomLF), audioReverbZone.roomLF);
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