using UnityEngine;
using UnityEngine.Audio;

namespace Popcron.SceneStaging
{
    public class AudioSourceProcessor : ComponentProcessor<AudioSource>
    {
        [RuntimeInitializeOnLoadMethod]
        private static void AutoRegister()
        {
            RegisterProcessor<AudioSourceProcessor>();
        }

        protected override void Save(Component component, AudioSource audioSource)
        {
            component.Set(nameof(AudioSource.bypassEffects), audioSource.bypassEffects);
            component.Set(nameof(AudioSource.bypassListenerEffects), audioSource.bypassListenerEffects);
            component.Set(nameof(AudioSource.bypassReverbZones), audioSource.bypassReverbZones);
            component.Set(nameof(AudioSource.clip), audioSource.clip);
            component.Set(nameof(AudioSource.dopplerLevel), audioSource.dopplerLevel);
            component.Set(nameof(AudioSource.ignoreListenerPause), audioSource.ignoreListenerPause);
            component.Set(nameof(AudioSource.ignoreListenerVolume), audioSource.ignoreListenerVolume);
            component.Set(nameof(AudioSource.loop), audioSource.loop);
            component.Set(nameof(AudioSource.maxDistance), audioSource.maxDistance);
            component.Set(nameof(AudioSource.minDistance), audioSource.minDistance);
            component.Set(nameof(AudioSource.mute), audioSource.mute);
            component.Set(nameof(AudioSource.outputAudioMixerGroup), audioSource.outputAudioMixerGroup);
            component.Set(nameof(AudioSource.panStereo), audioSource.panStereo);
            component.Set(nameof(AudioSource.pitch), audioSource.pitch);
            component.Set(nameof(AudioSource.playOnAwake), audioSource.playOnAwake);
            component.Set(nameof(AudioSource.priority), audioSource.priority);
            component.Set(nameof(AudioSource.reverbZoneMix), audioSource.reverbZoneMix);
            component.Set(nameof(AudioSource.rolloffMode), audioSource.rolloffMode);
            component.Set(nameof(AudioSource.spatialBlend), audioSource.spatialBlend);
            component.Set(nameof(AudioSource.spatialize), audioSource.spatialize);
            component.Set(nameof(AudioSource.spatializePostEffects), audioSource.spatializePostEffects);
            component.Set(nameof(AudioSource.spread), audioSource.spread);
            component.Set(nameof(AudioSource.time), audioSource.time);
            component.Set(nameof(AudioSource.timeSamples), audioSource.timeSamples);
            component.Set(nameof(AudioSource.velocityUpdateMode), audioSource.velocityUpdateMode);
            component.Set(nameof(AudioSource.volume), audioSource.volume);
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