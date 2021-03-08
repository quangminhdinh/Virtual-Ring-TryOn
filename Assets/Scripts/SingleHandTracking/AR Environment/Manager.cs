using UnityEngine;
using System.Collections;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Manager : MonoBehaviour {
    #region Singleton
    public static Manager instance;

    private void Awake() {
        instance = this;
    }
    #endregion

    public GameObject LandmarkObj;

    public Vector3 GetUpperJoint(){
        return GetComponent<ARHandProcessor>().CurrentHand.GetUpperJoint();
    }

    public Vector3 GetLowerJoint(){
        return GetComponent<ARHandProcessor>().CurrentHand.GetLowerJoint();
    }

    public Vector3 GetRingPos(){
        return GetComponent<ARHandProcessor>().CurrentHand.GetRingPos();
    }
}