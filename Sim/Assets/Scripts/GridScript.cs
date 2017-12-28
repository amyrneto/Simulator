using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridScript : MonoBehaviour {

    public bool isColiding;
    public int index;
    private int nrColisions = 0;
    private AgentGridScript parentScript;

    public void Initialize()
    {
        parentScript = transform.parent.GetComponent<AgentGridScript>();
    }

    void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Obstacle") || (other.name.Contains("Agent") && other.name != transform.parent.name))
        {
            isColiding = true;
            nrColisions++;
            transform.GetComponent<MeshRenderer>().enabled = true;
            parentScript.gridArray[index] = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("Obstacle") || (other.name.Contains("Agent") && other.name != transform.parent.name) )
        {
            nrColisions--;
            if (nrColisions <= 0)
            {
                nrColisions = 0;
                isColiding = false;
                transform.GetComponent<MeshRenderer>().enabled = false;
                parentScript.gridArray[index] = false;
            }
        }
    }
}
