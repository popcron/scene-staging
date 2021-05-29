using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Popcron.SceneStaging
{
    [AddComponentMenu("")]
    [ExecuteAlways]
    public class SceneSettings : MonoBehaviour, IOnLoaded
    {
        [HideInInspector]
        public Material skybox;

        [HideInInspector]
        public AmbientMode ambientMode;

        [HideInInspector]
        public Color ambientSkyColor;

        [HideInInspector]
        public Color ambientEquatorColor;

        [HideInInspector]
        public Color ambientGroundColor;

        [HideInInspector]
        public float ambientIntensity;

        [HideInInspector]
        public Color ambientLight;
        
        [HideInInspector]
        public Object customReflection;

        [HideInInspector]
        public DefaultReflectionMode defaultReflectionMode;

        [HideInInspector]
        public int defaultReflectionResolution;

        [HideInInspector]
        public float flareFadeSpeed;

        [HideInInspector]
        public float flareStrength;

        [HideInInspector]
        public bool fog;

        [HideInInspector]
        public Color fogColor;

        [HideInInspector]
        public Light sun;

        [HideInInspector]
        public Color subtractiveShadowColor;

        [HideInInspector]
        public float reflectionIntensity;

        [HideInInspector]
        public int reflectionBounces;

        [HideInInspector]
        public float haloStrength;

        [HideInInspector]
        public float fogStartDistance;

        [HideInInspector]
        public FogMode fogMode;

        [HideInInspector]
        public float fogEndDistance;

        [HideInInspector]
        public float fogDensity;

        private void FixedUpdate()
        {
            if (!Application.isPlaying)
            {
                SaveSettings();
            }
        }

        /// <summary>
        /// Saves the settings of the scene.
        /// </summary>
        public void SaveSettings()
        {
            skybox = RenderSettings.skybox;
            ambientMode = RenderSettings.ambientMode;
            ambientSkyColor = RenderSettings.ambientSkyColor;
            ambientEquatorColor = RenderSettings.ambientEquatorColor;
            ambientGroundColor = RenderSettings.ambientGroundColor;
            ambientIntensity = RenderSettings.ambientIntensity;
            ambientLight = RenderSettings.ambientLight;
            customReflection = RenderSettings.customReflection;
            defaultReflectionMode = RenderSettings.defaultReflectionMode;
            defaultReflectionResolution = RenderSettings.defaultReflectionResolution;
            flareFadeSpeed = RenderSettings.flareFadeSpeed;
            flareStrength = RenderSettings.flareStrength;
            fog = RenderSettings.fog ;
            fogColor = RenderSettings.fogColor;
            fogDensity = RenderSettings.fogDensity;
            fogEndDistance = RenderSettings.fogEndDistance;
            fogMode = RenderSettings.fogMode;
            fogStartDistance = RenderSettings.fogStartDistance;
            haloStrength = RenderSettings.haloStrength;
            reflectionBounces = RenderSettings.reflectionBounces;
            reflectionIntensity = RenderSettings.reflectionIntensity;
            subtractiveShadowColor = RenderSettings.subtractiveShadowColor;
            sun = RenderSettings.sun;
        }

        /// <summary>
        /// Loads the settings into the scene.
        /// </summary>
        public void LoadSettings()
        {
            RenderSettings.skybox = skybox;
            RenderSettings.ambientMode = ambientMode;
            RenderSettings.ambientSkyColor = ambientSkyColor;
            RenderSettings.ambientEquatorColor = ambientEquatorColor;
            RenderSettings.ambientGroundColor = ambientGroundColor;
            RenderSettings.ambientIntensity = ambientIntensity;
            RenderSettings.ambientLight = ambientLight;

            //custom reflection changes from Cubemap to Texture
            PropertyInfo customReflection = typeof(RenderSettings).GetProperty(nameof(RenderSettings.customReflection));
            customReflection.SetValue(null, this.customReflection);

            RenderSettings.defaultReflectionMode = defaultReflectionMode;
            RenderSettings.defaultReflectionResolution = defaultReflectionResolution;
            RenderSettings.flareFadeSpeed = flareFadeSpeed;
            RenderSettings.flareStrength = flareStrength;
            RenderSettings.fog = fog;
            RenderSettings.fogColor = fogColor;
            RenderSettings.fogDensity = fogDensity;
            RenderSettings.fogEndDistance = fogEndDistance;
            RenderSettings.fogMode = fogMode;
            RenderSettings.fogStartDistance = fogStartDistance;
            RenderSettings.haloStrength = haloStrength;
            RenderSettings.reflectionBounces = reflectionBounces;
            RenderSettings.reflectionIntensity = reflectionIntensity;
            RenderSettings.subtractiveShadowColor = subtractiveShadowColor;
            RenderSettings.sun = sun;
        }

        void IOnLoaded.Loaded(List<Variable> variables)
        {
            LoadSettings();
        }
    }
}