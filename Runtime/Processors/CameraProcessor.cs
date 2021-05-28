using UnityEngine;

namespace Popcron.SceneStaging
{
    public class CameraProcessor : ComponentProcessor<Camera>
    {
        protected override void Save(Component component, Camera camera)
        {
            component.Set(nameof(camera.orthographic), camera.orthographic);
            component.Set(nameof(camera.orthographicSize), camera.orthographicSize);
            component.Set(nameof(camera.cullingMask), camera.cullingMask);
            component.Set(nameof(camera.cameraType), camera.cameraType);
            component.Set(nameof(camera.clearFlags), camera.clearFlags);
            component.Set(nameof(camera.backgroundColor), camera.backgroundColor);
            component.Set(nameof(camera.depth), camera.depth);
            component.Set(nameof(camera.lensShift), camera.lensShift);
            component.Set(nameof(camera.usePhysicalProperties), camera.usePhysicalProperties);
            component.Set(nameof(camera.targetTexture), camera.targetTexture);
            component.Set(nameof(camera.fieldOfView), camera.fieldOfView);
            component.Set(nameof(camera.focalLength), camera.focalLength);
            component.Set(nameof(camera.forceIntoRenderTexture), camera.forceIntoRenderTexture);
            component.Set(nameof(camera.nearClipPlane), camera.nearClipPlane);
            component.Set(nameof(camera.farClipPlane), camera.farClipPlane);
        }

        protected override void Load(Component component, Camera camera)
        {
            camera.orthographic = component.Get<bool>(nameof(camera.orthographic));
            camera.orthographicSize = component.Get<float>(nameof(camera.orthographicSize));
            camera.cullingMask = component.Get<int>(nameof(camera.cullingMask));
            camera.cameraType = component.Get<CameraType>(nameof(camera.cameraType));
            camera.clearFlags = component.Get<CameraClearFlags>(nameof(camera.clearFlags));
            camera.backgroundColor = component.Get<Color>(nameof(camera.backgroundColor));
            camera.depth = component.Get<float>(nameof(camera.depth));
            camera.lensShift = component.Get<Vector2>(nameof(camera.lensShift));
            camera.usePhysicalProperties = component.Get<bool>(nameof(camera.usePhysicalProperties));
            camera.targetTexture = component.Get<RenderTexture>(nameof(camera.targetTexture));
            camera.fieldOfView = component.Get<float>(nameof(camera.fieldOfView));
            camera.focalLength = component.Get<float>(nameof(camera.focalLength));
            camera.forceIntoRenderTexture = component.Get<bool>(nameof(camera.forceIntoRenderTexture));
            camera.nearClipPlane = component.Get<float>(nameof(camera.nearClipPlane));
            camera.farClipPlane = component.Get<float>(nameof(camera.farClipPlane));
        }
    }
}