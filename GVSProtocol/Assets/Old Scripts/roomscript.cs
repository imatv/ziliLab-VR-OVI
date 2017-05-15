using UnityEngine;
using System.Collections;

public class roomscript : MonoBehaviour {
	GameObject room;
	// Use this for initialization
	void Start () {
		room = GameObject.Find ("Room");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Alpha1)) {	
			room.transform.rotation = Quaternion.Euler (new Vector3 (0, 180, -20));
		}
		if (Input.GetKeyDown (KeyCode.Alpha2)) {	
			room.transform.rotation = Quaternion.Euler (new Vector3 (0, 180, -15));
		}
		if (Input.GetKeyDown (KeyCode.Alpha3)) {	
			room.transform.rotation = Quaternion.Euler (new Vector3 (0, 180, -10));
		}
		if (Input.GetKeyDown (KeyCode.Alpha4)) {	
			room.transform.rotation = Quaternion.Euler (new Vector3 (0, 180, -5));
		}
		if (Input.GetKeyDown (KeyCode.Alpha5)) {	
			room.transform.rotation = Quaternion.Euler (new Vector3 (0, 180, 5));
		}
		if (Input.GetKeyDown (KeyCode.Alpha6)) {	
			room.transform.rotation = Quaternion.Euler (new Vector3 (0, 180, 10));
		}
		if (Input.GetKeyDown (KeyCode.Alpha7)) {	
			room.transform.rotation = Quaternion.Euler (new Vector3 (0, 180, 15));
		}
		if (Input.GetKeyDown (KeyCode.Alpha8)) {	
			room.transform.rotation = Quaternion.Euler (new Vector3 (0, 180, 20));
		}
		if (Input.GetKeyDown (KeyCode.BackQuote)) {	
			room.transform.rotation = Quaternion.Euler (new Vector3 (0, 180, 0));
		}
	}
}
