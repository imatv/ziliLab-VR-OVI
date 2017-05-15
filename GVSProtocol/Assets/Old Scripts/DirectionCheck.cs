using UnityEngine;
using System.Collections;

public class DirectionCheck : MonoBehaviour {
	public bool trigger;
	public GameObject parent;
	void Start(){
		trigger = false;
		parent = GameObject.Find(gameObject.name[0]+"check");

	}
	void OnTriggerEnter (Collider col){
		if (col.name == parent.name) {
			trigger = true;
			Debug.Log (gameObject.name[0]+"entered");
		}
	}
	void OnTriggerExit (Collider col){
		if (col.name == parent.name) {
			trigger = false;
			Debug.Log (gameObject.name[0]+"exited");
		}
	}

}
