using System;
using System.Collections;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using MediapipeHandTracking;

public class ARFrameProcessor : MonoBehaviour {

    public static float ALPHA = float.NegativeInfinity;
    public static float BETA = float.NegativeInfinity;
    public ARCameraManager cameraManager;
    private float imageRatio = float.NegativeInfinity;
    private HandProcessor handProcessor;
    public RawImage rawImage;

    void Awake() {
        Application.targetFrameRate = 60;
    }

    void Start() {
        BETA = (float)Screen.width / Screen.height;
        handProcessor = new HandProcessor();
        StartCoroutine(process());
    }

    unsafe void convertCPUImage() {
        XRCpuImage image;
        if (!cameraManager.TryAcquireLatestCpuImage(out image)) {
            Debug.Log("Cant get image");
            return;
        }

        if (float.IsNegativeInfinity(ALPHA)) {
            ALPHA = (float)image.height / image.width;
            imageRatio = (float)(BETA / ALPHA);
        }

        var conversionParams = new XRCpuImage.ConversionParams {
            // Get the entire image
            inputRect = new RectInt(0, 0, image.width, image.height),
            // Downsample by 2
            outputDimensions = new Vector2Int(image.width / 2, image.height / 2),
            // Choose RGBA format
            outputFormat = TextureFormat.RGBA32,
            // Flip across the vertical axis (mirror image)
            transformation = XRCpuImage.Transformation.MirrorY
        };

        int size = image.GetConvertedDataSize(conversionParams);

        var buffer = new NativeArray<byte>(size, Allocator.Temp);
        image.Convert(conversionParams, new IntPtr(buffer.GetUnsafePtr()), buffer.Length);
        image.Dispose();

        Texture2D m_Texture = new Texture2D(
            conversionParams.outputDimensions.x,
            conversionParams.outputDimensions.y,
            conversionParams.outputFormat,
            false);

        m_Texture.LoadRawTextureData(buffer);
        m_Texture.Apply();
        buffer.Dispose();
        // pass image for mediapipe
        long time = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
        Debug.Log("Texture loaded at: " + time.ToString());
        // rawImage.texture = rotateTexture(m_Texture, false);
        // rawImage.texture = m_Texture;
        handProcessor.addFrameTexture(m_Texture);
    }

    public IEnumerator process() {
        while (true) {
            yield return new WaitForEndOfFrame();
            convertCPUImage();
            yield return null;
        }
    }

     Texture2D rotateTexture(Texture2D originalTexture, bool clockwise)
     {
         Color32[] original = originalTexture.GetPixels32();
         Color32[] rotated = new Color32[original.Length];
         int w = originalTexture.width;
         int h = originalTexture.height;
 
         int iRotated, iOriginal;
 
         for (int j = 0; j < h; ++j)
         {
             for (int i = 0; i < w; ++i)
             {
                 iRotated = (i + 1) * h - j - 1;
                 iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                 rotated[iRotated] = original[iOriginal];
             }
         }
 
         Texture2D rotatedTexture = new Texture2D(h, w);
         rotatedTexture.SetPixels32(rotated);
         rotatedTexture.Apply();
         return rotatedTexture;
     }

    public HandProcessor HandProcessor { get => handProcessor;}
    public float ImageRatio { get => imageRatio;}
}