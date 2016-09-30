using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

public class BodySourceView : MonoBehaviour 
{
    public Material BoneMaterial;
    public GameObject BodySourceManager;
	public GameObject target;
	public GameObject[] list = new GameObject[2];

	public bool manoDerCerrada = false;
	public bool manoIzqCerrada = false;
    
    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;
	private int element = 0;
	private int lenght = 2;
    
    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };
    
	void Start(){
		list [0] = GameObject.Find ("bikeWheel");
		list [1] = GameObject.Find ("bikeSeat");
	}

    void Update () 
    {
        if (BodySourceManager == null)
        {
            return;
        }
        
        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null)
        {
            return;
        }
        
        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }
        
        List<ulong> trackedIds = new List<ulong>();
        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
              }
                
            if(body.IsTracked)
            {
                trackedIds.Add (body.TrackingId);
            }
        }
        
        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);
        
        // First delete untracked bodies
        foreach(ulong trackingId in knownIds)
        {
            if(!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }

        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
            }
            
            if(body.IsTracked)
            {
                if(!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }
                
                RefreshBodyObject(body, _Bodies[body.TrackingId]);
            }
        }
    }
    
    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);
        
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.SetVertexCount(2);
            lr.material = BoneMaterial;
            lr.SetWidth(0.05f, 0.05f);
            
            jointObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
        }
        
		body.transform.parent = GameObject.Find("ARCamera").transform;

        return body;
    }
    
    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;
            
            if(_BoneMap.ContainsKey(jt)){
                targetJoint = body.Joints[_BoneMap[jt]];
            }
			if ((int)body.HandRightState == 3) {
				target = bodyObject;
				Debug.Log ("Mano derecha cerrada");
				manoDerCerrada = true;
			} else {
				manoDerCerrada = false;
			}

			if ((int)body.HandLeftState == 3 && !manoIzqCerrada) {
				manoIzqCerrada = true;
				System.Threading.Thread.Sleep(20);
				element++;
				if (element >= lenght) {
					element = 0;
				}
			}
			else {
				manoIzqCerrada = false;
			}

			if (element != 0) {
				list [element - 1].GetComponent<Renderer> ().material.color = Color.white;
			} else{
				list [lenght - 1].GetComponent<Renderer> ().material.color = Color.white;
			}
			list[element].GetComponent<Renderer> ().material.color = Color.green;

			Transform jointObj = bodyObject.transform.FindChild(jt.ToString());

			float distX = jointObj.localPosition.x - list[element].transform.position.x;
			float distY = jointObj.localPosition.y - list[element].transform.position.y;
			float distZ = jointObj.localPosition.z - list[element].transform.position.z;

			/*if (jointObj == bodyObject.transform.FindChild ("RightHand")) {
				float distX = jointObj.localPosition.x - GameObject.Find ("BikeWheel").transform.position.x;
				float distY = jointObj.localPosition.y - GameObject.Find ("BikeWheel").transform.position.y;
				float distZ = jointObj.localPosition.z - GameObject.Find ("BikeWheel").transform.position.z;
			}*/

			jointObj.localPosition = GetVector3FromJoint (sourceJoint);

			if (manoDerCerrada && jointObj == bodyObject.transform.FindChild ("HandRight") && bodyObject == target) {

				Vector3 position = new Vector3 ((jointObj.localPosition.x - distX), (jointObj.localPosition.y - distY), (jointObj.localPosition.z - distZ));

				list[element].transform.position = position;
			}
            
            /*(LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if(targetJoint.HasValue)
            {
                lr.SetPosition(0, jointObj.localPosition);
                lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
                lr.SetColors(GetColorForState (sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
            }
            else
            {
                lr.enabled = false;
            }*/
        }
    }
    
    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
        case Kinect.TrackingState.Tracked:
            return Color.green;

        case Kinect.TrackingState.Inferred:
            return Color.red;

        default:
            return Color.black;
        }
    }
    
    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 50, joint.Position.Y * 50, joint.Position.Z * 50);
    }
}
