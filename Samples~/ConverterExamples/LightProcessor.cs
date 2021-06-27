using UnityEngine;
using UnityEngine.Rendering;

namespace Popcron.SceneStaging
{
    public class LightProcessor : ComponentProcessor<Light>
    {
        [RuntimeInitializeOnLoadMethod]
        private static void AutoRegister()
        {
            RegisterProcessor<LightProcessor>();
        }

        protected override void Save(Component component, Light light)
        {
            component.Set(nameof(light.type), light.type);
            component.Set(nameof(light.intensity), light.intensity);
            component.Set(nameof(light.color), light.color);
            component.Set(nameof(light.colorTemperature), light.colorTemperature);
            component.Set(nameof(light.innerSpotAngle), light.innerSpotAngle);
            component.Set(nameof(light.range), light.range);
            component.Set(nameof(light.useColorTemperature), light.useColorTemperature);
            component.Set(nameof(light.renderMode), light.renderMode);
            component.Set(nameof(light.shadowBias), light.shadowBias);
            component.Set(nameof(light.shadowCustomResolution), light.shadowCustomResolution);
            component.Set(nameof(light.shadowNearPlane), light.shadowNearPlane);
            component.Set(nameof(light.shadowNormalBias), light.shadowNormalBias);
            component.Set(nameof(light.shadowResolution), light.shadowResolution);
            component.Set(nameof(light.shadows), light.shadows);
            component.Set(nameof(light.shape), light.shape);
            component.Set(nameof(light.spotAngle), light.spotAngle);
        }

        protected override void Load(Component component, Light light)
        {
            light.type = component.Get<LightType>(nameof(light.type));
            light.shape = component.Get<LightShape>(nameof(light.shape));
            light.intensity = component.Get<float>(nameof(light.intensity));
            light.color = component.Get<Color>(nameof(light.color));
            light.colorTemperature = component.Get<float>(nameof(light.colorTemperature));
            light.innerSpotAngle = component.Get<float>(nameof(light.innerSpotAngle));
            light.range = component.Get<float>(nameof(light.range));
            light.useColorTemperature = component.Get<bool>(nameof(light.useColorTemperature));
            light.renderMode = component.Get<LightRenderMode>(nameof(light.renderMode));
            light.shadows = component.Get<LightShadows>(nameof(light.shadows));
            light.shadowBias = component.Get<float>(nameof(light.shadowBias));
            light.shadowCustomResolution = component.Get<int>(nameof(light.shadowCustomResolution));
            light.shadowNearPlane = component.Get<float>(nameof(light.shadowNearPlane));
            light.shadowNormalBias = component.Get<float>(nameof(light.shadowNormalBias));
            light.spotAngle = component.Get<float>(nameof(light.spotAngle));

            try
            {
                light.shadowResolution = component.Get<LightShadowResolution>(nameof(light.shadowResolution));
            }
            catch
            {

            }
        }
    }
}