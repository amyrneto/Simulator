using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalDirectionScript : MonoBehaviour {

	public Transform goal;

	private AgentNavigationScript parentNavScript;
	private ObjectivesManager objManagerScript;
	// Use this for initialization
	void Start () {
		parentNavScript = transform.parent.GetComponent<AgentNavigationScript> ();
		objManagerScript = parentNavScript.objectiveManager.GetComponent<ObjectivesManager> ();
	}

	// Update is called once per frame
	void Update () {
		goal = objManagerScript.objectivesList [parentNavScript.index];
		transform.LookAt (goal);
	}
}
