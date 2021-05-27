using UnityEngine;
using UnityEngine.Rendering;

namespace Popcron.SceneStaging
{
    public class LightProcessor : ComponentProcessor<Light>
    {
        protected override void Save(Component component, Light light)
        {
            component.Add(nameof(light.type), light.type);
            component.Add(nameof(light.intensity), light.intensity);
            component.Add(nameof(light.color), light.color);
            component.Add(nameof(light.colorTemperature), light.colorTemperature);
            component.Add(nameof(light.innerSpotAngle), light.innerSpotAngle);
            component.Add(nameof(light.range), light.range);
            component.Add(nameof(light.useColorTemperature), light.useColorTemperature);
            component.Add(nameof(light.renderMode), light.renderMode);
            component.Add(nameof(light.shadowBias), light.shadowBias);
            component.Add(nameof(light.shadowCustomResolution), light.shadowCustomResolution);
            component.Add(nameof(light.shadowNearPlane), light.shadowNearPlane);
            component.Add(nameof(light.shadowNormalBias), light.shadowNormalBias);
            component.Add(nameof(light.shadowResolution), light.shadowResolution);
            component.Add(nameof(light.shadows), light.shadows);
            component.Add(nameof(light.shape), light.shape);
            component.Add(nameof(light.spotAngle), light.spotAngle);
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