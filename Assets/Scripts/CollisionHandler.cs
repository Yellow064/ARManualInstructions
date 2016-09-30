using UnityEngine;
using System.Collections;

public class CollisionHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter (CollisionHandler Col){
		Debug.Log ("Collision");
	}
}
