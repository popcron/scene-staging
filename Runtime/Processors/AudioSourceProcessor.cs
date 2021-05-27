using UnityEngine;
using UnityEngine.Audio;

namespace Popcron.SceneStaging
{
    public class AudioSourceProcessor : ComponentProcessor<AudioSource>
    {
        protected override void Save(Component component, AudioSource audioSource)
        {
            component.Add(nameof(AudioSource.bypassEffects), audioSource.bypassEffects);
            component.Add(nameof(AudioSource.bypassListenerEffects), audioSource.bypassListenerEffects);
            component.Add(nameof(AudioSource.bypassReverbZones), audioSource.bypassReverbZones);
            component.Add(nameof(AudioSource.clip), audioSource.clip);
            component.Add(nameof(AudioSource.dopplerLevel), audioSource.dopplerLevel);
            component.Add(nameof(AudioSource.ignoreListenerPause), audioSource.ignoreListenerPause);
            component.Add(nameof(AudioSource.ignoreListenerVolume), audioSource.ignoreListenerVolume);
            component.Add(nameof(AudioSource.loop), audioSource.loop);
            component.Add(nameof(AudioSource.maxDistance), audioSource.maxDistance);
            component.Add(nameof(AudioSource.minDistance), audioSource.minDistance);
            component.Add(nameof(AudioSource.mute), audioSource.mute);
            component.Add(nameof(AudioSource.outputAudioMixerGroup), audioSource.outputAudioMixerGroup);
            component.Add(nameof(AudioSource.panStereo), audioSource.panStereo);
            component.Add(nameof(AudioSource.pitch), audioSource.pitch);
            component.Add(nameof(AudioSource.playOnAwake), audioSource.playOnAwake);
            component.Add(nameof(AudioSource.priority), audioSource.priority);
            component.Add(nameof(AudioSource.reverbZoneMix), audioSource.reverbZoneMix);
            component.Add(nameof(AudioSource.rolloffMode), audioSource.rolloffMode);
            component.Add(nameof(AudioSource.spatialBlend), audioSource.spatialBlend);
            component.Add(nameof(AudioSource.spatialize), audioSource.spatialize);
            component.Add(nameof(AudioSource.spatializePostEffects), audioSource.spatializePostEffects);
            component.Add(nameof(AudioSource.spread), audioSource.spread);
            component.Add(nameof(AudioSource.time), audioSource.time);
            component.Add(nameof(AudioSource.timeSamples), audioSource.timeSamples);
            component.Add(nameof(AudioSource.velocityUpdateMode), audioSource.velocityUpdateMode);
            component.Add(nameof(AudioSource.volume), audioSource.volume);
        }

        protected override void Load(Component component, AudioSource audioSource)
        {
            audioSource.bypassEffects = component.Get<bool>(nameof(AudioSource.bypassEffects));
            audioSource.bypassListenerEffects = component.Get<bool>(nameof(AudioSource.bypassListenerEffects));
            audioSource.bypassReverbZones = component.Get<bool>(nameof(AudioSource.bypassReverbZones));
            audioSource.clip = component.Get<AudioClip>(nameof(AudioSource.clip));
            audioSource.dopplerLevel = component.Get<float>(nameof(AudioSource.dopplerLevel));
            audioSource.ignoreListenerPause = component.Get<bool>(nameof(AudioSource.ignoreListenerPause));
            audioSource.ignoreListenerVolume = component.Get<bool>(nameof(AudioSource.ignoreListenerVolume));
            audioSource.loop = component.Get<bool>(nameof(AudioSource.loop));
            audioSource.maxDistance = component.Get<float>(nameof(AudioSource.maxDistance));
            audioSource.minDistance = component.Get<float>(nameof(AudioSource.minDistance));
            audioSource.mute = component.Get<bool>(nameof(AudioSource.mute));
            audioSource.outputAudioMixerGroup = component.Get<AudioMixerGroup>(nameof(AudioSource.outputAudioMixerGroup));
            audioSource.panStereo = component.Get<float>(nameof(AudioSource.panStereo));
            audioSource.pitch = component.Get<float>(nameof(AudioSource.pitch));
            audioSource.playOnAwake = component.Get<bool>(nameof(AudioSource.playOnAwake));
            audioSource.priority = component.Get<int>(nameof(AudioSource.priority));
            audioSource.reverbZoneMix = component.Get<float>(nameof(AudioSource.reverbZoneMix));
            audioSource.rolloffMode = component.Get<AudioRolloffMode>(nameof(AudioSource.rolloffMode));
            audioSource.spatialBlend = component.Get<float>(nameof(AudioSource.spatialBlend));
            audioSource.spatialize = component.Get<bool>(nameof(AudioSource.spatialize));
            audioSource.spatializePostEffects = component.Get<bool>(nameof(AudioSource.spatializePostEffects));
            audioSource.spread = component.Get<float>(nameof(AudioSource.spread));
            audioSource.time = component.Get<float>(nameof(AudioSource.time));
            audioSource.timeSamples = component.Get<int>(nameof(AudioSource.timeSamples));
            audioSource.velocityUpdateMode = component.Get<AudioVelocityUpdateMode>(nameof(AudioSource.velocityUpdateMode));
            audioSource.volume = component.Get<float>(nameof(AudioSource.volume));
        }
    }
}