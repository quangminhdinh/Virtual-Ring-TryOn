using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;

public class LightEstimation : MonoBehaviour
{
    [SerializeField]
    private ARCameraManager arCameraManager;

    private Light currentLight;

    private void Awake() 
    {
        currentLight = GetComponent<Light>();
    }

    private void OnEnable() 
    {
        arCameraManager.frameReceived += FrameUpdated;
    }

    private void OnDisable() 
    {
        arCameraManager.frameReceived -= FrameUpdated;
    }

    private void FrameUpdated(ARCameraFrameEventArgs args) 
    {
        if (args.lightEstimation.averageBrightness.HasValue)
        {
            currentLight.intensity = args.lightEstimation.averageBrightness.Value;
        }

        if (args.lightEstimation.averageColorTemperature.HasValue)
        {
            currentLight.colorTemperature = args.lightEstimation.averageColorTemperature.Value;
        }

        if (args.lightEstimation.colorCorrection.HasValue)
        {
            currentLight.color = args.lightEstimation.colorCorrection.Value;
        }

        if (args.lightEstimation.mainLightDirection.HasValue)
        {
            currentLight.transform.rotation = Quaternion.LookRotation(args.lightEstimation.mainLightDirection.Value);
        }

        if (args.lightEstimation.mainLightColor.HasValue)
        {
            currentLight.color = args.lightEstimation.mainLightColor.Value;
        }

        if (args.lightEstimation.mainLightIntensityLumens.HasValue)
        {
            currentLight.intensity = args.lightEstimation.averageMainLightBrightness.Value;
        }

        if (args.lightEstimation.ambientSphericalHarmonics.HasValue)
        {
            RenderSettings.ambientMode = AmbientMode.Skybox;
            RenderSettings.ambientProbe = args.lightEstimation.ambientSphericalHarmonics.Value;
        }
    }
}
