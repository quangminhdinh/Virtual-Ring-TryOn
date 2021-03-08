using UnityEngine;
using MediapipeHandTracking;

public class ARHand {
    private Vector3 upperJoint, lowerJoint, direction, ringPos = default;
    public float currentDepth = 0f;
    private Camera cam;

    public static int ROOT = 0;
    public static int[] INDEX_FINGER = {5, 6, 7, 8};
    public static int[] MID_FINGER = {9, 10, 11, 12};
    public static int[] RING_FINGER = {13, 14, 15, 16};
    public static int[] THUMB = {2, 3, 4};
    public static int[] PINKY = {17, 18, 19, 20};


    public static int UPPER_JOINT_POS = RING_FINGER[1];
    public static int LOWER_JOINT_POS = RING_FINGER[0];

    public ARHand() {
        cam = Camera.main;
    }

    private ARHand(Vector3 upperJoint, Vector3 lowerJoint) {
        this.upperJoint = upperJoint;
        this.lowerJoint = lowerJoint;
    }

    public void ParseFrom(float[] arr, float c) {
        if (null == arr || arr.Length < 63) return;

        this.upperJoint = ConvertPosition(arr, c, UPPER_JOINT_POS);
        this.lowerJoint = ConvertPosition(arr, c, LOWER_JOINT_POS);

        direction = this.upperJoint - this.lowerJoint;
        ringPos = this.lowerJoint + 0.5f * direction;
    }

    public Vector3 ConvertPosition(float[] arr, float c, int index) {
        float x = Screen.width * ((arr[index * 3 + 1] - 0.5f * (1 - c)) / c) + 0.1f;
        float y = Screen.height * (arr[index * 3]);
        return cam.ScreenToWorldPoint(new Vector3(x, y, 0.5f));
    }

    public Vector3 GetUpperJoint() => this.upperJoint;
    public Vector3 GetLowerJoint() => this.lowerJoint;
    public Vector3 GetRingPos() => this.ringPos;

}