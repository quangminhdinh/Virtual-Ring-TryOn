using System.Collections;
using System.Collections.Generic;
using MediapipeHandTracking;
using UnityEngine;

public class FPS : MonoBehaviour {
    public static FPS instance;
    private float deltaTime = 0.0f;
    private float fps;
    public float Count { get => fps; }
    public FPSRank Rank { get; set; }

    private ARHandProcessor handProcessor;
    // private RaycastOnPlane hitest;

    private void Awake() {
        instance = this;
        fps = 60;
        Rank = FPSRank.High;
        Application.targetFrameRate = 60;

    }

    void Update() {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI() {
        int w = Screen.width, h = Screen.height;
        GUIStyle style = new GUIStyle();
        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        // Rect rect1 = new Rect(0, rect.height + 2, w, h * 2 / 100);
        // Rect rect2 = new Rect(0, rect1.yMax + 2, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        float msec = deltaTime * 1000.0f;
        fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
        // GUI.Label(rect1, "Hand: " + handProcessor.CurrentHand.GetLandmark(0), style);
        // GUI.Label(rect2, "Hit: " + hitest.LazeOnSpace.Tail, style);
    }

    public enum FPSRank {
        Low,
        Medium,
        High
    }
}
