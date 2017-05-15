using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class controlAlignment : MonoBehaviour {

    // References to world objects
	GameObject rod;
	GameObject room;
	GameObject text;
	GameObject position;

    // Internal Control Variables
    Vector3 adjustment;
    List<decimal> Room0;
    List<decimal> RoomP18;
    List<decimal> RoomN18;
    decimal rodAngle;
    
    bool start;
	bool startSupine;
    bool blocked;
	float blockTimer;
    bool startTrial;
    int roomChosen;
    int rodChosen;
    bool randomChosen;
    bool responseGiven;
    bool done;
    int trialCount;
    string output;
    bool mousefixed;
	bool supineRotated;
	bool completeCheck;
    float holdTimer;
    float holdThreshold;
    bool holdingLeft;
    bool holdingRight;
    
    List<int> incomplete;
    List<int> rodChoices;

    // Settings
	[Header("Run Settings")]
	public string SubjectString;
    public int N;
    public bool upright;
	public bool mouse;
    
    [Header("Data Settings")]
	public string OutputPath;
    
	// Use this for initialization
	void Start () {
        rod = GameObject.Find ("Rod");
		room = GameObject.Find ("Room");
		text = GameObject.Find ("Text");
		position = GameObject.Find ("Position");
        
        adjustment = new Vector3 (0.0f, 0.0f, 0.2f);
        rodAngle = 0.0m;
        
        start = true;
		startSupine = true;
		blocked = false;
        blockTimer = 0.0f;
        startTrial = false;
        roomChosen = 90;
        rodChosen = 90;
        randomChosen = false;
        responseGiven = false;
        done = false;
        trialCount = 0;
        output = "";
        mousefixed = false;
        supineRotated = false;
        completeCheck = false;
        holdTimer = 0.0f;
        holdThreshold = 0.5f;
        holdingLeft = false;
        holdingRight = false;
        
        incomplete = new List<int> ();
        rodChoices = new List<int> ();
        
        Room0 = new List<decimal> ();
        RoomP18 = new List<decimal> ();
        RoomN18 = new List<decimal> ();
        
        for (int i = -15; i <= -2; i++) {
            rodChoices.Add(i);
        }
        for (int i = 2; i <= 15; i++) {
            rodChoices.Add(i);
        }
	}
	
    // Used to handle a response for a trial
	void recResponse(int room, decimal resp) {

		//Format: "Room Angle,Rod Angle\n"
        
		// Update tally
        if (room == 0) {
            Room0.Add(resp);
        }
        if (room == 18) {
            RoomP18.Add(resp);
        }
        if (room == -18) {
            RoomN18.Add(resp);
        }
		
        // Append to output string
		output += room + "," + resp + "\n";
        Debug.Log(output);
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
                if (Room0.Count != N) {
                    incomplete.Add (0);
                }
                if (RoomP18.Count != N) {
                    incomplete.Add (18);
                }
                if (RoomN18.Count != N) {
                    incomplete.Add (-18);
                }
				completeCheck = true;
			}
    
            // If not done...
			if (completeCheck && incomplete.Count != 0) {
            
                // Choose an arbitrary incomplete condition to run
                if (!randomChosen) {
                    //Debug.Log ("Choosing random trial");
                    // Choose condition to run
                    int r1 = UnityEngine.Random.Range (0, incomplete.Count);
                    int r2 = UnityEngine.Random.Range (0, rodChoices.Count);
                    roomChosen = incomplete[r1];
                    rodChosen = rodChoices[r2];
                    rodAngle = (decimal)rodChosen;
                    //Debug.Log ("Trial chosen");
                    randomChosen = true;
                }
                // Block for 1s and alter room/rod orientations
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
                        room.transform.rotation = Quaternion.Euler (new Vector3 (0, 180, roomChosen));
                        rod.transform.rotation = Quaternion.Euler (new Vector3 (0, 180, rodChosen));
                    }
                    else {
                        room.transform.rotation = Quaternion.Euler (new Vector3 (90, 180, roomChosen));
                        rod.transform.rotation = Quaternion.Euler (new Vector3 (90, 180, rodChosen));
                    }
                    //Debug.Log ("Room/Rod rotated");
                    Debug.Log ("Room: " + roomChosen + ", Rod: " + rodChosen);
                    Debug.Log ("Trial Number: " + (trialCount+1));
                }
                if (blocked) {
                    blockTimer += Time.deltaTime;
                }
                if (blockTimer > 1.0f && !startTrial)
                {
                    blocked = false;
                    position.SetActive (true);
                    startTrial = true;
                    //Debug.Log ("View unblocked");
                    //Debug.Log ("Start trial");
                }
                
                
                // Allow for a response
                if (startTrial && !responseGiven) {
                    // Allow for adjustment
                    if (!mouse) {
                        // Left or right arrow keys
                        
                        // On press
                        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                            //Debug.Log ("left keypress");
                            rod.transform.Rotate(-adjustment);
                            rodAngle -= 0.2m;
                            holdingLeft = true;
                        }
                        else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                            //Debug.Log ("right keypress");
                            rod.transform.Rotate(adjustment);
                            rodAngle += 0.2m;
                            holdingRight = true;
                        }
                        
                        // On release
                        if (Input.GetKeyUp(KeyCode.LeftArrow)) {
                            holdingLeft = false;
                            holdTimer = 0.0f;
                        }
                        if (Input.GetKeyUp(KeyCode.RightArrow)) {
                            holdingRight = false;
                            holdTimer = 0.0f;
                        }
                        
                        // If holding, check if held for long enough before applying quick adjust
                        if (holdingLeft || holdingRight) {
                            holdTimer += Time.deltaTime;
                        }
                        if (holdTimer >= holdThreshold) {
                            if (holdingLeft) {
                                rod.transform.Rotate(-adjustment);
                                rodAngle -= 0.2m;
                            }
                            else if (holdingRight) {
                                rod.transform.Rotate(adjustment);
                                rodAngle += 0.2m;
                            }
                        }
                    }
                    else {
                        // Left or right mouse buttons
                        
                        // On press
                        if (Input.GetButtonDown("Fire1")) {
                            //Debug.Log ("left mouse button");
                            rod.transform.Rotate(-adjustment);
                            rodAngle -= 0.2m;
                            holdingLeft = true;
                        }
                        else if (Input.GetButtonDown("Fire2")) {
                            //Debug.Log ("right mouse button");
                            rod.transform.Rotate(adjustment);
                            rodAngle += 0.2m;
                            holdingRight = true;
                        }
                        
                        // On release
                        if (Input.GetButtonUp("Fire1")) {
                            holdingLeft = false;
                            holdTimer = 0.0f;
                        }
                        if (Input.GetButtonUp("Fire2")) {
                            holdingRight = false;
                            holdTimer = 0.0f;
                        }
                        
                        // If holding, check if held for long enough before applying quick adjust
                        if (holdingLeft || holdingRight) {
                            holdTimer += Time.deltaTime;
                        }
                        if (holdTimer >= holdThreshold) {
                            if (holdingLeft) {
                                rod.transform.Rotate(-adjustment);
                                rodAngle -= 0.2m;
                            }
                            else if (holdingRight) {
                                rod.transform.Rotate(adjustment);
                                rodAngle += 0.2m;
                            }
                        }
                    }
                    
                    // Subjective Vertical submit
                    if (Input.GetKeyDown (KeyCode.Return)) {	
                        // Save current room and rod angle
                        recResponse(roomChosen, rodAngle);
                        responseGiven = true;
                        trialCount++;
                        
                        // Ensure key press handling is reset
                        holdingLeft = false;
                        holdingRight = false;
                        holdTimer = 0.0f;
                        
                        // Save data to file output after every response
                        string filePath = OutputPath + SubjectString + ".txt";
                        System.IO.File.WriteAllText(filePath, output);
                        Debug.Log ("Data saved");
                    }
                }
            
                // Reset for next trial
                if (responseGiven) {
                    blockTimer = 0.0f;
                    startTrial = false;
                    completeCheck = false;
                    incomplete = new List<int> ();
                    randomChosen = false;
                    roomChosen = 90;
                    rodChosen = 90;
                    rodAngle = 0.0m;
                    responseGiven = false;
                    //Debug.Log ("Reset variables");
                }
            }
            
            // All trials completed
			if (completeCheck && incomplete.Count == 0 && !done) {
				done = true;
                
                decimal sum = 0.0m;
                for (int i = 0; i < Room0.Count; i++) {
                    sum += Room0[i];
                }
                decimal avg = sum/(decimal)N;
                Debug.Log ("Room 0 Subjective Vertical Average: " + avg);
                
                sum = 0.0m;
                for (int i = 0; i < RoomP18.Count; i++) {
                    sum += RoomP18[i];
                }
                avg = sum/(decimal)N;
                Debug.Log ("Room P18 Subjective Vertical Average: " + avg);
                
                sum = 0.0m;
                for (int i = 0; i < RoomN18.Count; i++) {
                    sum += RoomN18[i];
                }
                avg = sum/(decimal)N;
                Debug.Log ("Room N18 Subjective Vertical Average: " + avg);
                
				//Debug.Log ("All trials completed");
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
				text.GetComponent<TextMesh>().text = "All trials complete.";
			}
        }
	}
}
