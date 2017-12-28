using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentNavigationScript : MonoBehaviour {
    public Vector3 deltaVelocity;
    public Vector3 oldVelocity;
    public UnityEngine.AI.NavMeshAgent agentNavMesh;
    public int state, agentIndex;
    public Transform objectiveManager;
    public int index;  // Index for the current objective, -1 if no objective assigned
    ObjectivesManager objManScript;
    private Camera cam;
    
	// Use this for initialization
	void Start () {
        // Starts in idle state.
        state = 0;
        // Pick a reference to the main camera. This is only used in the mouse click control (state=3).
        cam = Camera.main;
        // Gets NavMeshAgent reference to set destination.
        agentNavMesh = GetComponent<UnityEngine.AI.NavMeshAgent>();
        // Initializes agent oldVelocity to compute the velocity variation frame by frame.
        oldVelocity = agentNavMesh.velocity;
        // Gets reference to objectiveManager script to access list of objectives and choose one.
        objManScript = objectiveManager.GetComponent<ObjectivesManager>();
        
    }
	
    
	// Update is called once per frame
	void Update () {
        /////////////////////////////////////////////////////////////////////
        // Agent state achine control by objective array and mouse click
        /***************************************************************/
        switch (state)
        {
            case 0: //  Idle: wait for initialization. The agent does not select new destinations, thus it does not move.
                break;
            case 1: // Select one objective from array.
                List<int> objFree = new List<int>();
                for(int i=0; i< objManScript.objectivesList.Count; i++) {
                    if (objManScript.objectivesList[i].GetComponent<ObjectiveScript>().free) {
                        objFree.Add(i);
                    }
                }
                index = Random.Range(0, objManScript.objectivesList.Count - 1);
                Vector3 dest = objManScript.objectivesList[index].position;

                // Set new destination.
                agentNavMesh.SetDestination(dest);
                // Show objective by enabling its MeshRenderer component.
                objManScript.objectivesList[index].GetComponent<ObjectiveScript>().Show();

                // Move to next state.
                state = 2;
                break;
            case 2: // Wait agent to reach destination
                if (!agentNavMesh.pathPending)
                {
                    // The destination is considered reached once the distance is smaller than a constant.
                    if (agentNavMesh.remainingDistance < 0.01f)
                    {
                        // Hide objective
                        objManScript.objectivesList[index].GetComponent<ObjectiveScript>().Hide();
                        // Move to previous state to select another objective from the array:
                        state = 1;
                    }
                }
                break;
            case 3: // Control by mouse click: allows user to control the agent by clicking the terrain with the mouse.
                // If the mouse button is clicked
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    // Performa a raycast to determine the mouse position.
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    // If hits
                    if (Physics.Raycast(ray, out hit))
                    {
                        // Set the hitting point as agent's new destination
                        agentNavMesh.SetDestination(hit.point);
                    }
                }
                break;
            default:    // In case of initialization error, goes to Idle state (0).
                state = 0;
                break;
        }
        /***************************************************************/
    }
}
