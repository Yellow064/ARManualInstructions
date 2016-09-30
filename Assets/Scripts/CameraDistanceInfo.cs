using UnityEngine;
using System.Collections;

public class CameraDistanceInfo{
	public GameObject object1;
	public GameObject object2;
	float myDistance;
	public string myMsg;

	public float Distance(GameObject object1, GameObject object2){

		float myDistance = Vector3.Distance(object1.transform.position, object2.transform.position);
		Debug.Log(myDistance);

		if (myDistance < 70) {
			Debug.Log (myMsg);
		}
		
		return myDistance;
	}
}