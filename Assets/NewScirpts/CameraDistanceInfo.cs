using UnityEngine;
using System.Collections;

public class CameraDistanceInfo : MonoBehaviour {
    public GameObject object1;
    public GameObject object2;
    float myDistance;
    public string myMsg;
	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        float myDistance = Vector3.Distance(object1.transform.position, object2.transform.position);
        Debug.Log(myDistance);
        if(myDistance < 70)
            Debug.Log(myMsg);
    }
}
