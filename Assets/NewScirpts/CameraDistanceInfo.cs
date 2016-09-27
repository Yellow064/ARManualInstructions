using UnityEngine;
using System.Collections;

public class CameraDistanceInfo : MonoBehaviour {
    Transform ARCameraTransform;
    Transform MyTransform;
    float myDistance;
	// Use this for initialization
	void Start () {
        ARCameraTransform = GameObject.Find("ARCamera").transform;
        MyTransform = GameObject.Find("Betty").transform;
    }
	
	// Update is called once per frame
	void Update () {
        float myDistance = Vector3.Distance(MyTransform.position, ARCameraTransform.position);
        Debug.Log(myDistance);
        Debug.Log("Say somehting hater");
    }
}
