using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridScript : MonoBehaviour {

    public bool isColiding;
    private int nrColisions = 0;

    void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Agent") )
        {
            Debug.Log("Agent colision!");
            return;
        }

        isColiding = true;
        nrColisions++;
        transform.GetComponent<MeshRenderer>().enabled = true;

    }

    void OnTriggerExit(Collider other)
    {
        nrColisions--;
        if(nrColisions <= 0)
        {
            nrColisions = 0;
            isColiding = false;
            transform.GetComponent<MeshRenderer>().enabled = false;
        }
    }

}
