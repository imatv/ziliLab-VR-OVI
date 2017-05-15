using UnityEngine;
using System.Collections;

public class rodscript : MonoBehaviour {
	GameObject rod;
	Vector3 big;
	Vector3 small;
	// Use this for initialization
	void Start () {
		rod = GameObject.Find ("Rod");
		big = new Vector3 (0, 0, 4);
		small = new Vector3 (0, 0, 2);
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Q))
		{	
			rod.transform.Rotate(big);
		}
		if (Input.GetKeyDown(KeyCode.W))
		{
			rod.transform.Rotate(-big);
		}		
		if (Input.GetKeyDown(KeyCode.A))
		{
			rod.transform.Rotate(small);
		}		
		if (Input.GetKeyDown(KeyCode.S))
		{
			rod.transform.Rotate(-small);
		}		
	}
}
