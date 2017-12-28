using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentNavigationScript : MonoBehaviour {
    public Camera cam;
    public UnityEngine.AI.NavMeshAgent agentNavMesh;
    public int state;
    public Transform objectiveManager;
    public int index;  // Index for the current objective, -1 if no objective assigned
    ObjectivesManager objManScript;

	// Use this for initialization
	void Start () {
        state = 0;
        cam = Camera.main;
        agentNavMesh = GetComponent<UnityEngine.AI.NavMeshAgent>();
        objManScript = objectiveManager.GetComponent<ObjectivesManager>();
	}
	
	// Update is called once per frame
	void Update () {
        ////////////////////////////////////////////////////////////////
        // Control by mouse click
        /***************************************************************
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                agentNavMesh.SetDestination(hit.point);
            }
        }
        /***************************************************************/

        ////////////////////////////////////////////////////////////////
        // Control by objective array
        /***************************************************************/
        switch (state)
        {
            case 0: //  Idle: wait for initialization
                break;
            case 1: // Select one objective from array
                //Debug.Log(objManScript.objectivesList.Count);
                index = Random.Range(0, objManScript.objectivesList.Count - 1);
                Vector3 dest = objManScript.objectivesList[index].position;

                // Set new destination
                agentNavMesh.SetDestination(dest);
                // Show objective
                objManScript.objectivesList[index].GetComponent<ObjectiveScript>().Show();

                state = 2;
                break;
            case 2: // Wait agent to reach destination
                if (!agentNavMesh.pathPending)
                {
                    //if (agentNavMesh.velocity.sqrMagnitude ==  0.0f)
                    if (agentNavMesh.remainingDistance < 0.01f)
                    {
                        //Debug.Log("hide objective");
                        // Hide objective
                        objManScript.objectivesList[index].GetComponent<ObjectiveScript>().Hide();
                        // Select another objective from the array:
                        state = 1;
                    }
                }
                break;
            default:
                state = 1;
                break;
        }
        /***************************************************************/
    }
}
