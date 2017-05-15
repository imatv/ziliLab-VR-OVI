using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class controlSeeded : MonoBehaviour {

	// References to world objects
	GameObject rod;
	GameObject room;
	GameObject text;
	GameObject position;
    GameObject fixation;
    GameObject cylinder;

	// Internal control variables
	bool start;
	bool startSupine;

	bool firstSupineRotate;
	bool blocked;
	float blockTimer;
	bool startTrial;
	bool completeCheck;
	List<int> incomplete;
	bool randomChosen;
	int idxChosen;
	bool response;
	string output;
	bool done;
	bool run0;
	bool supineRotated;
	int trialcount;
	float inc;
	bool loadFile;
	bool pause;
	bool pauseJustFinished;
	bool justLoadedFile;
	bool mousefixed;
	int NTrials;
	int PauseCount;
	bool mouse;
	public float trialTimer;
	float trialTime;

    bool startafterpause = true;

	// Select/Deselect rooms & Set max tally count
	[Header("Run Settings")]
	public string SubjectString;
	public bool upright;
	public float SubjVertRoom0;
	public float SubjVertRoomP18;
	public float SubjVertRoomN18;

	[Header("Room Activation")]
	public bool Room0;
	//public bool RoomN8;
	public bool RoomP18;
	public bool RoomN18;
    public bool NoRoom;
	//public bool RoomP28;
	//public bool RoomN28;
	//public bool RoomP38;
	//public bool RoomN38;

	[Header("Data Settings")]
	public string OutputPath;

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
		UnityEngine.Random.seed = 42;
		// Grab references & Initialize controls
		rod = GameObject.Find ("Rod");
		room = GameObject.Find ("Room");
		text = GameObject.Find ("Text");
		position = GameObject.Find ("Position");
       // fixation = GameObject.Find("FixationPoint");
        cylinder = GameObject.Find("Cylinder");
        //fixation.SetActive(false);
        cylinder.GetComponent<MeshRenderer>().enabled = false;
        start = true;
		startSupine = true;
		firstSupineRotate = true;
		blocked = false;
		blockTimer = 0.0f;
		startTrial = false;
		completeCheck = false;
		incomplete = new List<int> ();
		randomChosen = false;
		idxChosen = -1;
		response = false;
		output = "";
		done = false;
		run0 = true;
		trialcount = 0;
		pause = false;
		pauseJustFinished = false;
		mousefixed = false;
		NTrials = 8;
		mouse = true;
		trialTimer = 0.0f;
		trialTime = 1.5f;
        
        if (upright) {
        	supineRotated = false;
            PauseCount = 128;
        }
        else {
            supineRotated = true;
            PauseCount = 25;
        }

        // Initialize all conditions (exclude rdAng 0 always)
        // rmAng: 0, +/-8, +/-18, +/-28, +/-38
        // rdAng: +/- 5 from subjVert

        //inc = 1.0f;
        //float incAngle = 0f;

        if (NoRoom) {
            Conds.Add(new Condition(1.0f, -0.25f, 0, NTrials, false));
            Conds.Add(new Condition(1.0f, -1.0f, 0, NTrials, false));
            Conds.Add(new Condition(1.0f, 1.0f, 0, NTrials, false));
            Conds.Add(new Condition(1.0f, -0.5f, 0, NTrials, false));
            Conds.Add(new Condition(1.0f, 0.5f, 0, NTrials, false));
            Conds.Add(new Condition(1.0f, 0.25f, 0, NTrials, false));
            Conds.Add(new Condition(1.0f, -1.5f, 0, NTrials, false));
            Conds.Add(new Condition(1.0f, 1.5f, 0, NTrials, false));
        }

		// Room #1: rmAng = 0, rdAng = [-5, 5] (subjVert = 0)
		if (Room0) {
			if (SubjVertRoom0 == 0.0f) {
				Conds.Add (new Condition (0.0f, -0.25f, 0, NTrials, false));
				Conds.Add (new Condition (0.0f, -1.0f, 0, NTrials, false));
                Conds.Add(new Condition(0.0f, 1.0f, 0, NTrials, false));
                Conds.Add (new Condition (0.0f, -0.5f, 0, NTrials, false));
				Conds.Add (new Condition (0.0f, 0.5f, 0, NTrials, false));
				Conds.Add (new Condition (0.0f, 0.25f, 0, NTrials, false));
                Conds.Add (new Condition (0.0f, -1.5f, 0, NTrials, false));
                Conds.Add (new Condition (0.0f, 1.5f, 0, NTrials, false));
            }
			else {
                Conds.Add (new Condition(0.0f, SubjVertRoom0 + 1.5f, 0, NTrials, false));
                Conds.Add (new Condition(0.0f, SubjVertRoom0 - 1.5f, 0, NTrials, false));
                Conds.Add (new Condition (0.0f, SubjVertRoom0 + 0.5f, 0, NTrials, false));
				Conds.Add (new Condition (0.0f, SubjVertRoom0 + 1.0f, 0, NTrials, false));
				Conds.Add (new Condition (0.0f, SubjVertRoom0 - 0.5f, 0, NTrials, false));
				Conds.Add (new Condition (0.0f, SubjVertRoom0 - 1.0f, 0, NTrials, false));
				Conds.Add (new Condition (0.0f, SubjVertRoom0 - 2.0f, 0, NTrials, false));
                Conds.Add (new Condition(0.0f, SubjVertRoom0 + 2.0f, 0, NTrials, false));
            }
		}

        // Room #4: rmAng = +18, rdAng = [0, 10] (subjVert = 5)
        if (RoomP18) {
			Conds.Add (new Condition (18.0f, SubjVertRoomP18 + 1.0f, 0, NTrials, false));
			Conds.Add (new Condition (18.0f, SubjVertRoomP18 + 2.0f, 0, NTrials, false));
			Conds.Add (new Condition (18.0f, SubjVertRoomP18 - 1.0f, 0, NTrials, false));
			Conds.Add (new Condition (18.0f, SubjVertRoomP18 - 2.0f, 0, NTrials, false));
			Conds.Add (new Condition (18.0f, SubjVertRoomP18 - 4.0f, 0, NTrials, false));
            Conds.Add(new Condition(18.0f, SubjVertRoomP18 + 4.0f, 0, NTrials, false));
            Conds.Add (new Condition (18.0f, SubjVertRoomP18 + 3.0f, 0, NTrials, false));
            Conds.Add (new Condition (18.0f, SubjVertRoomP18 - 3.0f, 0, NTrials, false));
        }
		// Room #5: rmAng = -18, rdAng = [-10, -0] (subjVert = -5)
		if (RoomN18) {
			Conds.Add (new Condition (-18.0f, SubjVertRoomN18 + 1.0f, 0, NTrials, false));
			Conds.Add (new Condition (-18.0f, SubjVertRoomN18 + 2.0f, 0, NTrials, false));
			Conds.Add (new Condition (-18.0f, SubjVertRoomN18 - 1.0f, 0, NTrials, false));
			Conds.Add (new Condition (-18.0f, SubjVertRoomN18 - 2.0f, 0, NTrials, false));
            Conds.Add(new Condition(-18.0f, SubjVertRoomP18 - 4.0f, 0, NTrials, false));
            Conds.Add(new Condition(-18.0f, SubjVertRoomP18 + 4.0f, 0, NTrials, false));
            Conds.Add (new Condition (-18.0f, SubjVertRoomN18 + 3.0f, 0, NTrials, false));
            Conds.Add (new Condition (-18.0f, SubjVertRoomN18 - 3.0f, 0, NTrials, false));
        }

		// Load data file
		// Build filename
		Debug.Log ("Loading file...");
		string fileName = OutputPath + SubjectString + ".txt";
		Debug.Log (fileName);
		
		// See if data file even exists
		try {
			// Create stream reader for data file
			StreamReader theReader = new StreamReader (fileName);
			Debug.Log ("File exists, reading data");
			
			// Immediately clean up the reader when done (memory)
			using (theReader) {
				
				// For each line...
				string line;
				do {
					// Read the line
					line = theReader.ReadLine ();
					
					if (line != null) {
						// Delimit line by commas
						string[] entries = line.Split (',');
						
						// Parse line and store values
						if (entries.Length > 0) {
							float rmAngle = float.Parse (entries[0]);
							float rdAngle = float.Parse (entries[1]);
							string resp = entries[2];
							float rt = float.Parse (entries[3]);
							//Debug.Log(rmAngle.ToString() + " " + rdAngle.ToString() + " " + resp);
							
							// Find condition to increment
							for (int i = 0; i < Conds.Count; i++) {
								if (Conds[i].roomAngle == rmAngle && Conds[i].rodAngle == rdAngle) {
									if (resp == "L") {
										recResponse(ref Conds, i, 0, rt);
										//Debug.Log ("Left recorded");
									}
									else {
										recResponse(ref Conds, i, 1, rt);
										//Debug.Log ("Right recorded");
									}
									trialcount++;
								}
							}
						}
					}
				} while (line != null);
				
				// Done reading, close the reader   
				theReader.Close ();
			}
			Debug.Log ("Data loaded");
			loadFile = true;
			justLoadedFile = true;
		}
		catch {
			Debug.Log ("File doesn't exist, starting new data file");
			loadFile = false;
			justLoadedFile = false;
		}
	}

	// Used to handle a response for a trial
	void recResponse(ref List<Condition> c, int idx, int resp, float rt) {
		
		// Format: "Room Angle,Rod Angle,Response,Reaction Time\n"
		
		// Update tally
		c[idx].incTrials();

		// Append to output string
		output += c[idx].roomAngle + "," + c[idx].rodAngle;
		if (resp == 0)
			output += ",0";
		else if (resp == 1)
			output += ",1";
		else
			output += ",2";
		output += "," + rt + "\n";
		
		Debug.Log (output);
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
		if (!upright && startSupine && Input.GetKeyDown (KeyCode.Return)) {
			startSupine = false;
			// Show rod
			rod.GetComponent<MeshRenderer>().enabled = true;
			// Remove text
			text.SetActive(false);
			Debug.Log ("(Supine) Enter pressed, starting trials");
		}

		// Allow for adjustment in upright position
		// Press 'ENTER' when ready
		if (upright && start && Input.GetKeyDown (KeyCode.Return)) {
			start = false;
			if (upright) {
				// Show rod
				rod.GetComponent<MeshRenderer>().enabled = true;
				// Remove text
				text.SetActive(false);
				Debug.Log ("(Upright) Enter pressed, starting trials");
			}
		}

		// Rotate to supine position
		if (!start && !upright && !supineRotated && firstSupineRotate) {
			position.transform.rotation = Quaternion.Euler (new Vector3 (-90, 0, 0));
			supineRotated = true;
			firstSupineRotate = false;
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

				// If paused
				if (pause) {
                    position.SetActive(true);
                    //fixation.SetActive(false);
                    blockTimer = 0.0f;
					//Debug.Log ("paused...");
					if (upright) {
						// Allow for adjustment in upright position
						// Press 'ENTER' when ready
						if (Input.GetKeyDown (KeyCode.Return)) {
							// Show rod
							rod.GetComponent<MeshRenderer>().enabled = true;
							// Remove text
							text.SetActive(false);
							Debug.Log ("(Upright) Enter pressed, starting trials");
							pause = false;
                            startafterpause = false;
                            pauseJustFinished = true;
						}
					}
					else {
						// Allow for adjustment in supine position
						// Press 'ENTER' when ready
						if (supineRotated && Input.GetKeyDown (KeyCode.Return)) {
							// Show rod
							rod.GetComponent<MeshRenderer>().enabled = true;
							// Remove text
							text.SetActive(false);
							Debug.Log ("(Supine) Enter pressed, starting trials");
							pause = false;
                            startafterpause = false;
                            pauseJustFinished = true;
						}

						// Allow for adjustment in upright position
						// Press 'ENTER' when ready
						if (!supineRotated && Input.GetKeyDown (KeyCode.Return)) {
							Debug.Log ("(Upright) Enter pressed, going to supine");
							// Go to supine adjustment state
							position.transform.rotation = Quaternion.Euler (new Vector3 (-90, 0, 0));
							supineRotated = true;
						}
						// Press 'r' to recenter while upright
						if (!supineRotated && Input.GetKeyDown (KeyCode.R)) {
							Debug.Log ("(Upright) R pressed, recentering");
							// Reset camera center to zero now
							UnityEngine.VR.InputTracking.Recenter();
						}
                        response = false;
                    }
				}
                
                if (pauseJustFinished && !startafterpause)
                {
                    startTrial = false;
                    //Debug.Log(startTrial);
                    //Debug.Log(blocked);
                    if (!blocked && !startTrial)
                    {
                        blocked = true;
                        position.SetActive(false);
                       // fixation.SetActive(true);
                        //Debug.Log ("View blocked");
                        // Alter orientations
                        if (Conds[idxChosen].roomAngle == 1.0f)
                        {
                            if (upright)
                            {
                                room.SetActive(false);
                                cylinder.GetComponent<MeshRenderer>().enabled = true;
                                rod.transform.rotation = Quaternion.Euler(new Vector3(0, 180, Conds[idxChosen].rodAngle));
                            }
                            else
                            {
                                room.SetActive(false);
                                cylinder.GetComponent<MeshRenderer>().enabled = true;
                                rod.transform.rotation = Quaternion.Euler(new Vector3(90, 180, Conds[idxChosen].rodAngle));
                            }
                        }
                        else
                        {
                            if (upright)
                            {
                                room.SetActive(true);
                                cylinder.GetComponent<MeshRenderer>().enabled = false;
                                room.transform.rotation = Quaternion.Euler(new Vector3(0, 180, Conds[idxChosen].roomAngle));
                                rod.transform.rotation = Quaternion.Euler(new Vector3(0, 180, Conds[idxChosen].rodAngle));
                            }
                            else
                            {
                                room.SetActive(true);
                                cylinder.GetComponent<MeshRenderer>().enabled = false;
                                room.transform.rotation = Quaternion.Euler(new Vector3(90, 180, Conds[idxChosen].roomAngle));
                                rod.transform.rotation = Quaternion.Euler(new Vector3(90, 180, Conds[idxChosen].rodAngle));
                            }
                        }
                        //Debug.Log ("Room/Rod rotated");
                        Debug.Log("Room: " + Conds[idxChosen].roomAngle + ", Rod: " + Conds[idxChosen].rodAngle);
                        Debug.Log("Trial Number: " + (trialcount + 1));
                    }
                    if (blocked)
                    {
                        blockTimer += Time.deltaTime;
                    }
                    if (blockTimer > 1.0f && !startTrial)
                    {
                        blocked = false;
                        position.SetActive(true);
                       // fixation.SetActive(false);

                        startTrial = true;
                        startafterpause = true;
                        //Debug.Log ("View unblocked");
                        //Debug.Log ("Start trial");
                    }
                    if (!randomChosen && !run0)
                    {
                        //Debug.Log ("Choosing random trial");
                        // Choose condition to run
                        int r = UnityEngine.Random.Range(0, incomplete.Count);
                        idxChosen = incomplete[r];
                        //Debug.Log ("Trial chosen");
                        randomChosen = true;
                    }
                }

                // If not paused
                if (!pause && startafterpause) {
					// If file was loaded, ignore Room 0 first run requirement
					if (loadFile) {
						run0 = false;
					}
					// If first run, keep within Room 0
					if (run0) {
						//Debug.Log ("First trial, run room 0");
						// Find range, then divide by angle increment amount
						float interval = 2.0f + 2.0f; // Room 0: [-2, 2], exclude 0
						int div = (int) (interval / 0.5f); // (max - min) / increment
						int r = UnityEngine.Random.Range (0, div); // trials for Room 0
						idxChosen = incomplete[r];
						Debug.Log ("Trial chosen");
						randomChosen = true;
						run0 = false;
					}
					// Choose an arbitrary incomplete condition to run
					if (!randomChosen && !run0) {
						//Debug.Log ("Choosing random trial");
						// Choose condition to run
						int r = UnityEngine.Random.Range (0, incomplete.Count);
						idxChosen = incomplete[r];
						//Debug.Log ("Trial chosen");
						randomChosen = true;
					}
					// Block for 1s and alter room/rod orientations
					if (!blocked && !startTrial && !pauseJustFinished) {
						blocked = true;
						position.SetActive (false);
                        //fixation.SetActive(true);
						//Debug.Log ("View blocked");

						// Reset camera center to zero before each trial if upright
						if (upright) {
							//Debug.Log ("Reset headset zero");
							UnityEngine.VR.InputTracking.Recenter();
						}

                        // Alter orientations
                        if (Conds[idxChosen].roomAngle == 1.0f)
                        {
                            if (upright)
                            {
                                room.SetActive(false);
                                cylinder.GetComponent<MeshRenderer>().enabled = true;
                                rod.transform.rotation = Quaternion.Euler(new Vector3(0, 180, Conds[idxChosen].rodAngle));
                            }
                            else
                            {
                                room.SetActive(false);
                                cylinder.GetComponent<MeshRenderer>().enabled = true;
                                rod.transform.rotation = Quaternion.Euler(new Vector3(90, 180, Conds[idxChosen].rodAngle));
                            }
                        }
                        else
                        {
                            if (upright)
                            {
                                room.SetActive(true);
                                cylinder.GetComponent<MeshRenderer>().enabled = false;
                                room.transform.rotation = Quaternion.Euler(new Vector3(0, 180, Conds[idxChosen].roomAngle));
                                rod.transform.rotation = Quaternion.Euler(new Vector3(0, 180, Conds[idxChosen].rodAngle));
                            }
                            else
                            {
                                room.SetActive(true);
                                cylinder.GetComponent<MeshRenderer>().enabled = false;
                                room.transform.rotation = Quaternion.Euler(new Vector3(90, 180, Conds[idxChosen].roomAngle));
                                rod.transform.rotation = Quaternion.Euler(new Vector3(90, 180, Conds[idxChosen].rodAngle));
                            }
                        }
                        //Debug.Log ("Room/Rod rotated");
                        Debug.Log ("Room: " + Conds[idxChosen].roomAngle + ", Rod: " + Conds[idxChosen].rodAngle);
						Debug.Log ("Trial Number: " + (trialcount+1));
					}
					if (blocked) {
						blockTimer += Time.deltaTime;
					}
					if (blockTimer > 1.0f && !startTrial)
					{
						blocked = false;
						position.SetActive (true);
                        //fixation.SetActive(false);
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
							recResponse(ref Conds, idxChosen, -1, 1.5f);
							response = true;
							trialcount++;
						}
						
						if (!mouse) {
							// Left or right arrow keys
							if (Input.GetKeyDown (KeyCode.LeftArrow)) {
								//Debug.Log ("left keypress");
								recResponse(ref Conds, idxChosen, 0, trialTimer);
								response = true;
								trialcount++;
								justLoadedFile = false;
								position.SetActive (false);
                               // fixation.SetActive(true);

                                // Save data to file output after every response
                                string filePath = OutputPath + SubjectString + ".txt";
								System.IO.File.WriteAllText(filePath, output);
								Debug.Log ("Data saved");
							}
							else if (Input.GetKeyDown (KeyCode.RightArrow)) {
								//Debug.Log ("right keypress");
								recResponse(ref Conds, idxChosen, 1, trialTimer);
								response = true;
								trialcount++;
								justLoadedFile = false;
								position.SetActive (false);
                               // fixation.SetActive(true);

                                // Save data to file output after every response
                                string filePath = OutputPath + SubjectString + ".txt";
								System.IO.File.WriteAllText(filePath, output);
								Debug.Log ("Data saved");
							}
						}
						else {
							// Left or right mouse buttons
							if (Input.GetButtonDown("Fire1")) {
								//Debug.Log ("left mouse button");
								recResponse(ref Conds, idxChosen, 0, trialTimer);
								response = true;
								trialcount++;
								justLoadedFile = false;
								position.SetActive (false);
                               // fixation.SetActive(true);

                                // Save data to file output after every response
                                string filePath = OutputPath + SubjectString + ".txt";
								System.IO.File.WriteAllText(filePath, output);
								Debug.Log ("Data saved");
							}
							else if (Input.GetButtonDown("Fire2")) {
								//Debug.Log ("right mouse button");
								recResponse(ref Conds, idxChosen, 1, trialTimer);
								response = true;
								trialcount++;
								justLoadedFile = false;
								position.SetActive (false);
                               // fixation.SetActive(true);

                                // Save data to file output after every response
                                string filePath = OutputPath + SubjectString + ".txt";
								System.IO.File.WriteAllText(filePath, output);
								Debug.Log ("Data saved");
							}
                        }
					}

					if (response && trialTimer < trialTime) {
						trialTimer += Time.deltaTime;
					}
					
					// Reset variables for next trial
					if (response && trialTimer >= trialTime) {
						blockTimer = 0.0f;
						startTrial = false;
						completeCheck = false;
						incomplete = new List<int> ();
						randomChosen = false;
						idxChosen = -1;
						response = false;
                        pauseJustFinished = false;
                        trialTimer = 0.0f;
						//Debug.Log ("Reset variables");
					}

					// If time for a break, set pause state
					if (PauseCount != 0 && !justLoadedFile && trialcount != 0 && (trialcount % PauseCount == 0) && !pause && !pauseJustFinished) {
						// Move to pause state
						Debug.Log ("Taking a break...");
						pause = true;
                        response = false;
                        trialTimer = 0.0f;
                        startafterpause = false;

                        // Remove rod
                        rod.GetComponent<MeshRenderer>().enabled = false;
						// Show text
						text.SetActive(true);
						
						// Go to upright & reset room
						if (upright) {
							room.transform.rotation = Quaternion.Euler (new Vector3 (0, 180, 0));
						}
						else {
							room.transform.rotation = Quaternion.Euler (new Vector3 (90, 180, 0));
							position.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
							supineRotated = false;
						}
					}
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
				text.GetComponent<TextMesh>().text = "All trials complete.\nNotify experimenter.";
			}
		}
	}
}

