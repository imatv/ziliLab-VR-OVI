using UnityEngine;
using System.Collections;

public class yrotation : MonoBehaviour {
	public GameObject center;
	public Vector3 rot;
	public Vector3 objrot;
	void Start () {
		center = GameObject.Find ("CenterEyeAnchor");
	}
	
	// Update is called once per frame
	void Update () {
		rot = center.transform.rotation.eulerAngles;
		objrot = transform.rotation.eulerAngles;
		gameObject.transform.Rotate (new Vector3 (0,-(objrot.y-rot.y),0));
	}
}
