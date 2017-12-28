using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentGridScript : MonoBehaviour
{

	public Transform gridPrefab, semiCirclePrefab;
	public List<bool> gridArray;
	public List<float> agentRays, goalRays;
	public int gridW, gridH;
	public int posX, posZ;
	public bool centered;
	public int nrRays;
	public float maxDistance;

	public SemiCircleScript agentRaysScript, goalRaysScript;

	// Use this for initialization
	public void BuildGrid ()
	{
		// Initializes delta to define if the agent is centered in a gridcell, or if it is in the intersection of gridcells
		float delta = centered ? -1.0f : -0.5f;

		// Build a grid in the order: left to right, top to bottom.
		gridArray = new List<bool> ();
		for (int h = gridH - 1; h >= 0; h--) {
			for (int w = 0; w < gridW; w++) {
				// Stores the position in 'pos'. It is computed based on Agent's coordinates posX and posZ and the delta (is centered?).
				Vector3 pos = transform.position;
				pos.x = pos.x + ((w - posX - delta) * Mathf.Ceil (gridPrefab.localScale.x));
				pos.z = pos.z + ((h - posZ - delta) * Mathf.Ceil (gridPrefab.localScale.z));

				// Instatiates the grid cell and make child of agent, so it will follow it.
				Transform t = Instantiate (gridPrefab, pos, Quaternion.identity);
				t.parent = this.transform;
				// Initialization creates a reference to this script in the grid to optimize status readin (is colliding?)
				t.GetComponent<GridScript> ().Initialize ();
				// The index gives the GridScript information of its position in the array.
				t.GetComponent<GridScript> ().index = gridArray.Count;
				// Initializes the array with all 'false'. this array is updated in the grid script using collision functions and the 'index' variable.
				gridArray.Add (false);
			}
		}		
	}

	public void BuildSemiCircle (Transform p)
	{
		agentRays = new List<float> (nrRays);
		goalRays = new List<float> (nrRays);
		for (int i = 0; i < nrRays; i++) {
			agentRays.Add (maxDistance);
			goalRays.Add (maxDistance);
		}

		// Instatiate a semi circle
		Transform t = Instantiate (semiCirclePrefab, transform.position, Quaternion.identity);
		t.parent = this.transform;
		agentRaysScript = t.GetComponent<SemiCircleScript> ();
		agentRaysScript.orientation = SemiCircleScript.SemiCircleOrientation.Agent;
		agentRaysScript.parentScript = this;
		agentRaysScript.nrRays = nrRays;
		agentRaysScript.maxDistance = maxDistance * transform.localScale.x;
		agentRaysScript.Initialize ();

		t = Instantiate (semiCirclePrefab, transform.position, Quaternion.identity);
		t.parent = p;
		goalRaysScript = t.GetComponent<SemiCircleScript> ();
		goalRaysScript.orientation = SemiCircleScript.SemiCircleOrientation.Objective;
		goalRaysScript.parentScript = this;
		goalRaysScript.nrRays = nrRays;
		goalRaysScript.maxDistance = maxDistance * transform.localScale.x;
		goalRaysScript.Initialize ();
	}
}
