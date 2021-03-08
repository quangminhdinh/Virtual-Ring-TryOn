# Virtual Ring Try-On

An application of AR technology in trying clothes and jewellery in Vietnam.
This project uses Mediapipe v0.6.9 in hand recognition and Unity's AR Foundation with Google ARCore Extension in model rendering.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

This project requires Unity v2020.2.1.f1 or later with the following packages installed:
- AR Foundation 4.1.0-preview.10 or later.
- ARCore XR Plugin 4.1.0-preview.10 or later.
- Android Logcat for debugging.

### Installing

1. Install packages and enable [ARCore](https://developers.google.com/ar/develop/unity-arf/enable-arcore).
2. Enable unsafe code, uncheck Mutil Thread Rendering, remove Vulkan Graphic setting, set Scripting Backend to IL2CPP and enable Target Architectures ARM64 (Edit > Project Settings > Player).
3. Download folder [Plugins](https://drive.google.com/file/d/1qz5Zmh8pnLTYHvREGdN6JEz720L-jWfW/view?usp=sharing), unzip and put it in `Assets/` folder.
4. Open Scene TryOn and switch to Android platform.

## Mediapipe's integration

Currently, Mediapipe does not support Unity and therefore, it is necessary to call mediapipe through Android plugin `.aar` files to make it works (Assets > Plugins > Android).

## Usage

Enable HandTracking object to start tracking.

### HandProcessor

This class is used to integrate mediapipe in the Android plugin.

After received image frome camera, use `addFrameTexture` function to convert texture and send it to mediapipe.

```csharp
public unsafe void addFrameTexture(Texture2D m_Texture) {
    byte[] frameImage = ImageConversion.EncodeToJPG(m_Texture);

    sbyte[] frameImageSigned = Array.ConvertAll(frameImage, b => unchecked((sbyte)b));

    ...
    singleHandMain.Call("setFrame", frameImageSigned);
    ...
}
```

Get data from `getHandLandmarksData` or `getHandRectData`.

```csharp
public float[] getHandLandmarksData() {
    return singleHandMain.Call<float[]>("getLandmarks");
}

public float[] getHandRectData() {
    return singleHandMain.Call<float[]>("getPalmRect");
}
```

### ARFrameProcessor

Access the device camera image on the CPU by using [`cameraManager.TryAcquireLatestCpuImage`](https://github.com/Unity-Technologies/arfoundation-samples/blob/main/Assets/Scripts/CpuImageSample.cs).

Get converted texture at the end of function `convertCPUImage` and send it to `HandProcessor`.

```csharp
handProcessor.addFrameTexture(m_Texture);
```

### ARHand

This class is used to get landmarks' normalized positions from mediapipe raw data and convert them to Unity's coordinates.

Z-index will be fixed at 0.5f and image depth will be recalculate based on the 2D distance between landmark 2 and landmark 17 to be more precise and less noise. The 3D model will be scaled later based on the new image depth.

```csharp
public Vector3 ConvertPosition(float[] arr, float c, int index) {
    float x = Screen.width * ((arr[index * 3 + 1] - 0.5f * (1 - c)) / c) + 0.1f;
    float y = Screen.height * (arr[index * 3]);
    return cam.ScreenToWorldPoint(new Vector3(x, y, 0.5f));
}
```

Function `ParseFrom` is used to convert positions and calculate ring's position in the current frame.

```csharp
direction = this.upperJoint - this.lowerJoint;
ringPos = this.lowerJoint + 0.5f * direction;
```

### ARHandProcessor

Update model's state based on calculated position and rotation.

```csharp
void UpdateRingPos() {
    if (!LandmarkObj.activeInHierarchy) return;
    if (Vector3.Distance(LandmarkObj.transform.position, currentHand.GetRingPos()) > 0.01f) {
        LandmarkObj.transform.position = currentHand.GetRingPos();
    }
    LandmarkObj.transform.LookAt(Camera.main.transform);
    LandmarkObj.transform.Rotate( 0, 180, 0 );
}
```

## Features

- Hand recognition.
- Model rendering.
- Light Estimation.

## Current issues

- Latency between actual ring position and model position.
- Environmental and material processing.

## References

- https://gitlab.com/thangnh.sas/mediapipe-unity-hand-tracking
- https://google.github.io/mediapipe/getting_started/android_archive_library.html
- https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.1/manual/index.html
- https://developers.google.com/ar/develop/unity-arf
