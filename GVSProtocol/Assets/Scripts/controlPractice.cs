using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class controlPractice : MonoBehaviour {

	// References to world objects
	GameObject rod;
	GameObject room;
	GameObject text;
	GameObject position;

	// Internal control variables
	bool start;
	bool startSupine;
	bool blocked;
	float blockTimer;
    float trialTimer;
	bool startTrial;
	bool completeCheck;
	List<int> incomplete;
	bool randomChosen;
	int idxChosen;
	bool response;
	string output;
	bool done;
	bool supineRotated;
	int trialcount;
	float inc;
	bool mousefixed;
    
	int NTrials;

	// Settings
	[Header("Run Settings")]
	public float trialTime;
	public bool upright;
	public bool mouse;

	[Header("Room Activation")]
	public bool Room0;
	public bool RoomP18;
	public bool RoomN18;

	// Struct to hold room/rod combinations, and how many times run
	public class Condition {
		public float roomAngle;
		public float rodAngle;
		int trials;
		int maxTrials;
		bool maxed;

		// Constructor to initialize a condition
		public Condition(float rmAng, float rdAng, int t, int maxt, bool m) {
			roomAngle = rmAng;
			rodAngle = rdAng;
			trials = t;
			maxTrials = maxt;
			maxed = m;
		}

		// Increment number of trials run for this condition
		public void incTrials() {
			trials++;
			// Check if this condition is done
			if (trials == maxTrials)
				maxed = true;
		}

		// Return the number of trials run for this condition
		public bool isMaxed() {
			return maxed;
		}
	}
	// List to hold all conditions
	List<Condition> Conds = new List<Condition>();

	// Use this for initialization
	void Start () {

		// Grab references & Initialize controls
		rod = GameObject.Find ("Rod");
		room = GameObject.Find ("Room");
		text = GameObject.Find ("Text");
		position = GameObject.Find ("Position");
		start = true;
		startSupine = true;
		blocked = false;
		blockTimer = 0.0f;
        trialTimer = 0.0f;
		startTrial = false;
		completeCheck = false;
		incomplete = new List<int> ();
		randomChosen = false;
		idxChosen = -1;
		response = false;
		output = "";
		done = false;
		trialcount = 0;
		mousefixed = false;
        NTrials = 2;

		if (!upright)
			supineRotated = false;
		else
			supineRotated = true;

		// Initialize all conditions (exclude rdAng 0 always)
		// rmAng: 0
		// rdAng: +/- 5 from subjVert
		inc = 2.5f;
		float incAngle = 0f;

		// Room #1: rmAng = 0, rdAng = [-5, 5] (subjVert = 0)
		if (Room0) {
			for (incAngle = -5.0f; incAngle <= 5.0f; incAngle += inc) {
				if (incAngle == 0.0f)
					continue;
				Conds.Add (new Condition (0.0f, incAngle, 0, NTrials, false));
			}
		}
		// Room #4: rmAng = +18, rdAng = [-6, 10] (subjVert = 5)
		if (RoomP18) {
			for (incAngle = -3.5f; incAngle <= 4.0f; incAngle += inc) {
				if (incAngle == 0.0f)
					continue;
				Conds.Add (new Condition (18.0f, incAngle, 0, NTrials, false));
			}
		}
		// Room #5: rmAng = -18, rdAng = [-10, -6] (subjVert = -5)
		if (RoomN18) {
			for (incAngle = -4f; incAngle <= 3.5f; incAngle += inc) {
				if (incAngle == 0.0f)
					continue;
				Conds.Add (new Condition (-18.0f, incAngle, 0, NTrials, false));
			}
		}
	}

	// Used to handle a response for a trial
	void recResponse(ref List<Condition> c, int idx, int resp) {

		//Format: "Room Angle,Rod Angle,Participant Response\n"

		// Update tally
		c[idx].incTrials();

		// Append to output string
		output += c[idx].roomAngle + "," + c[idx].rodAngle;
		if (resp == 0)
			output += ",L\n";
		else if (resp == 1)
			output += ",R\n";
        else
            output += ",NaN\n";

		//Debug.Log (output);
	}

	// Update is called once per frame
	void Update () {
		
        // Allow for a fixed mouse cursor
        if (Input.GetKeyDown (KeyCode.RightShift)) {
			mousefixed=!mousefixed;
			Screen.lockCursor=mousefixed;
		}
        
		// Allow for adjustment in supine position
		// Press 'ENTER' when ready
		if (!upright && !start && startSupine && Input.GetKeyDown (KeyCode.Return)) {
			startSupine = false;
			// Show rod
			rod.GetComponent<MeshRenderer>().enabled = true;
			// Remove text
			text.SetActive(false);
			Debug.Log ("(Supine) Enter pressed, starting trials");
		}

		// Allow for adjustment in upright position
		// Press 'ENTER' when ready
		if (start && Input.GetKeyDown (KeyCode.Return)) {
			start = false;
			if (upright) {
				// Show rod
				rod.GetComponent<MeshRenderer>().enabled = true;
				// Remove text
				text.SetActive(false);
				Debug.Log ("(Upright) Enter pressed, starting trials");
			}
		}
        // Press 'r' to recenter while upright
		if (start && !supineRotated && Input.GetKeyDown (KeyCode.R)) {
			Debug.Log ("(Upright) R pressed, recentering");
			// Reset camera center to zero now
			UnityEngine.VR.InputTracking.Recenter();
		}

		// Rotate to supine position
		if (!start && !upright && !supineRotated) {
			position.transform.rotation = Quaternion.Euler (new Vector3 (-90, 0, 0));
			supineRotated = true;
		}

		// Begin trials
		if ((upright && !start) || (!upright && !startSupine)) {

			// Check for conditions that are not done
			if (!completeCheck) {
				//Debug.Log ("Checking for incomplete trials");
				for (int i = 0; i < Conds.Count; i++) {
					if (!Conds[i].isMaxed()) {
						incomplete.Add (i);
					}
				}
				completeCheck = true;
			}
			// If not done...
			if (completeCheck && incomplete.Count != 0) {
                // Choose an arbitrary incomplete condition to run
                if (!randomChosen) {
                    Debug.Log ("Choosing random trial");
                    // Choose condition to run
                    int r = UnityEngine.Random.Range (0, incomplete.Count);
                    idxChosen = incomplete[r];
                    Debug.Log ("Trial chosen");
                    randomChosen = true;
                }
                // Block for 0.5s (ISI) and alter room/rod orientations
                if (!blocked && !startTrial) {
                    blocked = true;
                    position.SetActive (false);
                    //Debug.Log ("View blocked");

                    // Reset camera center to zero before each trial if upright
                    if (upright) {
                        //Debug.Log ("Reset headset zero");
                        UnityEngine.VR.InputTracking.Recenter();
                    }

                    // Alter orientations
                    if (upright) {
                        room.transform.rotation = Quaternion.Euler (new Vector3 (0, 180, Conds[idxChosen].roomAngle));
                        rod.transform.rotation = Quaternion.Euler (new Vector3 (0, 180, Conds[idxChosen].rodAngle));
                    }
                    else {
                        room.transform.rotation = Quaternion.Euler (new Vector3 (90, 180, Conds[idxChosen].roomAngle));
                        rod.transform.rotation = Quaternion.Euler (new Vector3 (90, 180, Conds[idxChosen].rodAngle));
                    }
                    //Debug.Log ("Room/Rod rotated");
                    Debug.Log ("Room: " + Conds[idxChosen].roomAngle + ", Rod: " + Conds[idxChosen].rodAngle);
                    Debug.Log ("Trial Number: " + (trialcount+1));
                }
                if (blocked) {
                    blockTimer += Time.deltaTime;
                }
                if (blockTimer >= 0.5f && !startTrial)
                {
                    blocked = false;
                    position.SetActive (true);
                    startTrial = true;
                    //Debug.Log ("View unblocked");
                    //Debug.Log ("Start trial");
                }
                
                // Record response
                if (startTrial && !response) {
                    
                    trialTimer += Time.deltaTime;
                    
                    if (trialTimer >= trialTime) {
                        // Took too long
                        Debug.Log ("Didn't respond in time...");
                        recResponse(ref Conds, idxChosen, -1);
                        response = true;
                        trialcount++;
                    }
                    
                    if (!mouse) {
                        // Left or right arrow keys
                        if (Input.GetKeyDown (KeyCode.LeftArrow)) {
                            Debug.Log ("left keypress");
                            recResponse(ref Conds, idxChosen, 0);
                            response = true;
                            trialcount++;
							position.SetActive (false);
                        }
                        else if (Input.GetKeyDown (KeyCode.RightArrow)) {
                            Debug.Log ("right keypress");
                            recResponse(ref Conds, idxChosen, 1);
                            response = true;
                            trialcount++;
							position.SetActive (false);
                        }
                    }
                    else {
                        // Left or right mouse buttons
                        if (Input.GetButtonDown("Fire1")) {
                            //Debug.Log ("left mouse button");
                            recResponse(ref Conds, idxChosen, 0);
                            response = true;
                            trialcount++;
							position.SetActive (false);
                        }
                        else if (Input.GetButtonDown("Fire2")) {
                            //Debug.Log ("right mouse button");
                            recResponse(ref Conds, idxChosen, 1);
                            response = true;
                            trialcount++;
							position.SetActive (false);
                        }
                    }
                }
                
                if (response && trialTimer < trialTime) {
                    trialTimer += Time.deltaTime;
                }

                // Reset variables for next trial
                if (response && trialTimer >= trialTime) {
                    blockTimer = 0.0f;
                    trialTimer = 0.0f;
                    startTrial = false;
                    completeCheck = false;
                    incomplete = new List<int> ();
                    randomChosen = false;
                    idxChosen = -1;
                    response = false;
                    //Debug.Log ("Reset variables");
                }
			}
			// All trials completed
			if (completeCheck && incomplete.Count == 0 && !done) {
				done = true;
				Debug.Log ("All trials completed");
			}
			if (done) {
				position.SetActive (true);
				// Remove rod & reset room
				rod.GetComponent<MeshRenderer>().enabled = false;
				if (upright) {
					room.transform.rotation = Quaternion.Euler (new Vector3 (0, 180, 0));
				}
				else {
					room.transform.rotation = Quaternion.Euler (new Vector3 (90, 180, 0));
				}
				// Show text saying complete
				text.SetActive(true);
				text.GetComponent<TextMesh>().text = "All trials complete.\nWait for experimenter.";
			}
		}
	}
}

