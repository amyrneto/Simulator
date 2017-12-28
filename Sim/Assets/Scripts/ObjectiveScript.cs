using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveScript : MonoBehaviour {

    public bool valid;
    public bool free=true;

    void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Terrain"))
        {
            return;
        }
        else
        {
            valid = false;
            //Destroy(transform.gameObject);
            //Debug.Log("Destroyed:" + transform.position);
            //Debug.Log("Destroyed!");
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);

    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
}
