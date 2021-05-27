using UnityEngine;

namespace Popcron.SceneStaging
{
    public class CameraProcessor : ComponentProcessor<Camera>
    {
        protected override void Save(Component component, Camera camera)
        {
            component.Add(nameof(camera.orthographic), camera.orthographic);
            component.Add(nameof(camera.orthographicSize), camera.orthographicSize);
            component.Add(nameof(camera.cullingMask), camera.cullingMask);
            component.Add(nameof(camera.cameraType), camera.cameraType);
            component.Add(nameof(camera.clearFlags), camera.clearFlags);
            component.Add(nameof(camera.backgroundColor), camera.backgroundColor);
            component.Add(nameof(camera.depth), camera.depth);
            component.Add(nameof(camera.lensShift), camera.lensShift);
            component.Add(nameof(camera.usePhysicalProperties), camera.usePhysicalProperties);
            component.Add(nameof(camera.targetTexture), camera.targetTexture);
            component.Add(nameof(camera.fieldOfView), camera.fieldOfView);
            component.Add(nameof(camera.focalLength), camera.focalLength);
            component.Add(nameof(camera.forceIntoRenderTexture), camera.forceIntoRenderTexture);
            component.Add(nameof(camera.nearClipPlane), camera.nearClipPlane);
            component.Add(nameof(camera.farClipPlane), camera.farClipPlane);
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