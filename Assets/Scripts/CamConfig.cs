using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
// Adds XRCameraConfigurationExtensions extension methods to XRCameraConfiguration.
// This is for the Android platform only.
using Google.XR.ARCoreExtensions;

public class CamConfig : MonoBehaviour
{
    [SerializeField]
    private ARCoreExtensions arcoreExtensions;

    public void Awake()
    {
        // If the return value is not a valid index (ex. the value if -1),
        // then no camera configuration will be set. If no previous selection exists, 
        // the ARCore session will use the previously selected camera configuration 
        // or a default configuration.
        arcoreExtensions.OnChooseXRCameraConfiguration = SelectCameraConfiguration;
    }

    // A custom camera configuration selection function
    int SelectCameraConfiguration(List<XRCameraConfiguration> configurations)
    {
        int index = 0;

        // Use custom logic here to choose the desired configuration from supportedConfigurations.

        for (int i = 1; i < configurations.Count; ++i)
        {
            // Choose a config for a given camera that uses the maximum
            // target FPS and texture dimension. If supported, this config also enables
            // the depth sensor.
            if (configurations[i].GetFPSRange().y > configurations[index].GetFPSRange().y &&
                configurations[i].GetTextureDimensions().x > configurations[index].GetTextureDimensions().x &&
                configurations[i].GetTextureDimensions().y > configurations[index].GetTextureDimensions().y)
                // configurations[i].CameraConfigDepthSensorUsage() == CameraConfigDepthSensorUsage.RequireAndUse)
            {
                index = i;
            }
        }

        return index;
    }
}
