using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ObjectivesManager : MonoBehaviour
{
	public enum GridOptions
	{
		Grid,
		SemiCircle
	}

	public GridOptions gridType;

	public int numAgents, obstacleStep, maxFlushLines;
	public List<Transform> objectivesList;
	public Terrain terrain;
	public Transform objectivePrefab, agentPrefab;
	public string fileName;

	public int gridW, gridH;
	public int posX, posZ;
	public bool centered;

	public int nrRays;
	public float maxDistance;

	private int state, flushCounter;
	private List<Transform> agentsList;
	private bool simulating = false;
	private StreamWriter outputFile;
	private List<AgentGridScript> agentsGridScriptsList;
	private List<AgentNavigationScript> agentsNavScriptsList;
	//private List<ApplyPredictionsScript> agentsApplyPredictions;
    

	// Use this for initialization
	void Start ()
	{
		state = 0;
		flushCounter = 0;

		objectivesList = new List<Transform> ();
		agentsList = new List<Transform> ();

		agentsGridScriptsList = new List<AgentGridScript> ();
		agentsNavScriptsList = new List<AgentNavigationScript> ();

		for (int i = (int)terrain.terrainData.bounds.min.x; i < (int)terrain.terrainData.bounds.max.x; i += obstacleStep) {
			for (int j = (int)terrain.terrainData.bounds.min.z; j < (int)terrain.terrainData.bounds.max.z; j += obstacleStep) {
				Transform t;
				//t = Instantiate (objectivePrefab, new Vector3 (terrain.GetPosition().x + (float)i + (obstacleStep/2), objectivePrefab.localScale.y, terrain.GetPosition().z + (float)j + (obstacleStep / 2)), Quaternion.identity);
				t = Instantiate (objectivePrefab, new Vector3 (terrain.GetPosition ().x + (float)i + (obstacleStep / 2), 0.0f, terrain.GetPosition ().z + (float)j + (obstacleStep / 2)), Quaternion.identity);
				t.parent = this.transform;
				objectivesList.Add (t);
			}
		}

		for (int i = 0; i < numAgents; i++) {
			// Set a random position for the agent
			int index = (int)Random.Range (0.0f, objectivesList.Count - 1);
			Vector3 pos = objectivesList [index].position;
			pos.y = agentPrefab.localScale.y / 2.0f;
			Transform t;
			t = Instantiate (agentPrefab, pos, Quaternion.identity);
			AgentGridScript gs = t.GetComponent<AgentGridScript> ();
			AgentNavigationScript ns = t.GetComponent<AgentNavigationScript> ();
			t.name = "Agent#" + agentsList.Count;
			ns.agentIndex = agentsList.Count;
			t.GetComponent<AgentNavigationScript> ().objectiveManager = this.transform;
			if (gridType == GridOptions.Grid) {
				gs.posX = posX;
				gs.posZ = posZ;
				gs.gridH = gridH;
				gs.gridW = gridW;
				gs.centered = centered;
				gs.BuildGrid ();
			} else if (gridType == GridOptions.SemiCircle) {
				Transform[] comp = t.GetComponentsInChildren<Transform> ();
				Transform p = t;
				for (int c = 0; c < comp.Length; c++) {
					if (comp [c].name == "GoalDirection") {
						p = comp [c];
					}
				}
				//Transform p = t.GetComponentsInChildren<Transform> ();
				gs.nrRays = nrRays;
				gs.maxDistance = maxDistance;
				gs.BuildSemiCircle (p);
			}
			agentsGridScriptsList.Add (gs);
			agentsNavScriptsList.Add (ns);
			agentsList.Add (t);
		}
		outputFile = new StreamWriter (fileName);
		if (gridType == GridOptions.Grid) {
			outputFile.WriteLine (gridW + ";" + gridH + ";" + posX + ";" + posZ + ";" + (centered ? 1 : 0));
		} else if (gridType == GridOptions.SemiCircle) {
			outputFile.WriteLine (nrRays + ";" + maxDistance * agentsList [0].localScale.x + ";");
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		switch (state) {
		case 0: //  Initialize State
			for (int i = 0; i < objectivesList.Count; i++) {
				float currentHigh = objectivesList [i].position.y;
				float newHigh = agentsList [0].position.y;
				objectivesList [i].Translate (0.0f, newHigh - currentHigh, 0.0f);
			}
			state = 1;
			break;
		case 1: // Initializes the simulation.
			// Ensures it is not recording yet. Initialization must take place before that.
			simulating = false;
			// Build list with valid goals
			BuildList ();
			// Move to next state to initialize agents.
			state = 2;
			break;
		case 2:
            // Start the agent(s) by setting state on navigatino script to '1'
			for (int i = 0; i < agentsList.Count; i++) {
				agentsList [i].GetComponent<AgentNavigationScript> ().state = 1;
			}
            // Signal file recording to begin.
			simulating = true;
            // Move to waiting state.
			state = 3;
			break;
		case 3: // Waiting state: the state machine remain in this state until 'ESC' key is pressed.S
			//if (gridType == GridOptions.SemiCircle) {
			//	for (int i = 0; i < agentsGridScriptsList.Count; i++) {
			//		agentsGridScriptsList [i].agentRaysScript.UpdateOutput ();
			//		agentsGridScriptsList [i].goalRaysScript.UpdateOutput ();
			//	}
			//}
			if (Input.GetKeyDown (KeyCode.Escape)) {
				// If 'ESC' key is pressed, move to terminating state.
				state = 4;
			}
			break;
		case 4: // Terminating state: finalizes the simulation.
            // Signal that simulation has stopped. This makes the data recording to stop.
			simulating = false;
            // Closes the file to write down content.
			outputFile.Close ();
            // Stop the agent(s) by setting their navigation scripts to Idle state. The agents may still pursuit unreached destination.
			for (int i = 0; i < agentsList.Count; i++) {
				agentsList [i].GetComponent<AgentNavigationScript> ().state = 0;
			}
            // Moves to Idle state.
			state = 5;
			break;
		case 5:		// Idle state
			break;
		default:
			state = 5;
			break;
		}		
	}

	void FixedUpdate ()
	{
		// The 'simulating' variable controls weather the agent(s) are moving, or not.
		if (simulating) {
			string s = "";   // String that contains the line to be written in the outputFile.

			if (gridType == GridOptions.Grid) {
				// Fills with zeros and ones depending on grid cells status (colliding or not).
				for (int i = 0; i < agentsGridScriptsList.Count; i++) {
					for (int j = 0; j < agentsGridScriptsList [i].gridArray.Count; j++) {
						// If is colliding writes '1', otherwise writes '0'.
						s += agentsGridScriptsList [i].gridArray [j] ? "1" : "0";
						s += ";";
					}
					// Compute relative direction.
					Vector3 direction = agentsList [i].transform.InverseTransformPoint (objectivesList [agentsNavScriptsList [i].index].position);
					s += Mathf.Atan2 (direction.x, direction.z) * (180 / Mathf.PI) + ";";
					//Debug.Log ("direction:" + direction);

					// Compute relative velocity
					agentsNavScriptsList [i].deltaVelocity = agentsNavScriptsList [i].agentNavMesh.velocity - agentsNavScriptsList [i].oldVelocity;
					//Char ':' for seperating agents state and action each frame.
					s += agentsNavScriptsList [i].deltaVelocity.x.ToString () + ";" + agentsNavScriptsList [i].deltaVelocity.y.ToString () + ";" + agentsNavScriptsList [i].deltaVelocity.z.ToString () + ":"; 
					agentsNavScriptsList [i].oldVelocity = agentsNavScriptsList [i].agentNavMesh.velocity;
				}

				// Finally, writes down the string 's' in the file and flushes it every 10 frames to avoid memory overflow.
				//agentsApplyPredictions.stateString = s;
				outputFile.WriteLine (s);
				flushCounter++;
				if (flushCounter > maxFlushLines) {
					outputFile.Flush ();
					flushCounter = 0;
				}
			} else if (gridType == GridOptions.SemiCircle) {
				s = "";
				for (int i = 0; i < agentsGridScriptsList.Count; i++) {
					agentsGridScriptsList [i].agentRaysScript.UpdateOutput ();
					agentsGridScriptsList [i].goalRaysScript.UpdateOutput ();
					s += agentsGridScriptsList [i].agentRays [0];
					for (int ray = 1; ray < agentsGridScriptsList [i].nrRays; ray++) {
						s += ";" + agentsGridScriptsList [i].agentRays [ray];
					}
					for (int ray = 0; ray < agentsGridScriptsList [i].nrRays; ray++) {
						s += ";" + agentsGridScriptsList [i].goalRays [ray];
					}
					// Compute relative direction.
					Vector3 direction = agentsList [i].transform.InverseTransformPoint (objectivesList [agentsNavScriptsList [i].index].position);
					s += Mathf.Atan2 (direction.x, direction.z) * (180 / Mathf.PI) + ";";

					// Compute relative velocity
					agentsNavScriptsList [i].deltaVelocity = agentsNavScriptsList [i].agentNavMesh.velocity - agentsNavScriptsList [i].oldVelocity;
					//Char ':' for seperating agents state and action each frame.
					s += agentsNavScriptsList [i].deltaVelocity.x.ToString () + ";" + agentsNavScriptsList [i].deltaVelocity.y.ToString () + ";" + agentsNavScriptsList [i].deltaVelocity.z.ToString () + ":"; 
					agentsNavScriptsList [i].oldVelocity = agentsNavScriptsList [i].agentNavMesh.velocity;
				}
				outputFile.WriteLine (s);
			}
		}
	}

	void BuildList ()
	{
		for (int i = 0; i < objectivesList.Count; i++) {
			Transform t = objectivesList [i];
			ObjectiveScript obSc = t.GetComponent<ObjectiveScript> ();
			if (!obSc.valid) {
				objectivesList.RemoveAt (i);
				Destroy (t.gameObject);
				i = 0;
			} else {
				//t.Translate (0.0f, agentsList [0].position.y, 0.0f, Space.Self);
				t.GetComponent<Collider> ().enabled = false;
				obSc.Hide ();
			}
		}
	}

	// Writes file befor quiting.
	void OnApplicationQuit ()
	{
		outputFile.Close ();
	}
}
