using UnityEngine;
using System.Collections;

public class blindscript : MonoBehaviour {
	GameObject position;
	// Use this for initialization
	void Start () {
		position = GameObject.Find ("Position");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.X)) {
			position.SetActive (!position.activeSelf);
		}
	}
}