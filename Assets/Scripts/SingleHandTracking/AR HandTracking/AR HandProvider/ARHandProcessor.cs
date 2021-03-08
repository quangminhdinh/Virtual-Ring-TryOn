using MediapipeHandTracking;
using UnityEngine;
using System;

public class ARHandProcessor : MonoBehaviour {
    private GameObject LandmarkObj = default;
    private HandRect currentHandRect = default;
    private HandRect oldHandRect = default;
    private ARHand currentHand = default;
    private bool isHandRectChange = default;

    void Start() {
        LandmarkObj = Manager.instance.LandmarkObj;
        currentHand = new ARHand();
        currentHandRect = new HandRect();
        oldHandRect = new HandRect();

        Application.onBeforeRender += UpdateRingPos;
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate() {
        if (GetComponent<ARFrameProcessor>().HandProcessor == null) return;
        float[] handRectData = GetComponent<ARFrameProcessor>().HandProcessor.getHandRectData();
        float[] handLandmarksData = GetComponent<ARFrameProcessor>().HandProcessor.getHandLandmarksData();

        if (null != handRectData) {
            currentHandRect = HandRect.ParseFrom(handRectData);
            // Console.log("Width: " + currentHandRect.Width + "\nHeight: " + currentHandRect.Height + "\nXCenter: " + currentHandRect.XCenter + "\nYCenter: " + currentHandRect.YCenter + "\nRotation: " + currentHandRect.Rotation);
            if (!isHandStay()) {
                oldHandRect = currentHandRect;
                isHandRectChange = true;
            } else {
                isHandRectChange = false;
            }
        }

        if (null != handLandmarksData && !float.IsNegativeInfinity(GetComponent<ARFrameProcessor>().ImageRatio)) {
            currentHand.ParseFrom(handLandmarksData, GetComponent<ARFrameProcessor>().ImageRatio);
        }
        long time = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
        Debug.Log("Rendered at: " + time.ToString());
    }

    void UpdateRingPos() {
        if (!LandmarkObj.activeInHierarchy) return;
        if (Vector3.Distance(LandmarkObj.transform.position, currentHand.GetRingPos()) > 0.01f) {
            LandmarkObj.transform.position = currentHand.GetRingPos();
        }
        LandmarkObj.transform.LookAt(Camera.main.transform);
        LandmarkObj.transform.Rotate( 0, 180, 0 );
    }

    private bool isHandStay() {
        return currentHandRect.XCenter == oldHandRect.XCenter &&
            currentHandRect.YCenter == oldHandRect.YCenter &&
            currentHandRect.Width == oldHandRect.Width &&
            currentHandRect.Height == oldHandRect.Height &&
            currentHandRect.Rotaion == oldHandRect.Rotaion;
    }

    public ARHand CurrentHand { get => currentHand; }
    public bool IsHandRectChange { get => isHandRectChange; }
    public HandRect CurrentHandRect { get => currentHandRect; }
}