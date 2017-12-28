using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyPredictionsScript : MonoBehaviour {

    public string stateString="";
    /// <summary>
    /// True to use python Communication for movement.False for navmesh decisions.Maybe we can make it a public variable,so we can change it from editor.
    /// </summary>
    //public const bool LIVEPREDICTIONS = false; // This is not usefull
    /// <summary>
    /// Access to pythoncomms script,for sending and receiving states and actions.
    /// </summary>
    private PythonComms python;
    private AgentNavigationScript agentNavigationScript;

    // Use this for initialization
    void Start()
    {
        agentNavigationScript = GetComponent<AgentNavigationScript>();
        initLivePredictions();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        applyPredictions();
        setStatesLivePredictions(stateString);
    }

    /// <summary>
    /// Called in Start,starts seperate thread that communicates with python process.
    /// </summary>
    void initLivePredictions()
    {
        python = GetComponent<PythonComms>();
        python.startPredictions();
    }

    /// <summary>
    /// Set the agent states to be sent in Python.
    /// </summary>
    /// <param name="s"></param>
    void setStatesLivePredictions(string s)
    {
        python.AgentsStates1 = s;
    }

    /// <summary>
    /// If we use predictions,checks whether new predictions were done,and apply them on each agent(hopefully).
    /// </summary>
    void applyPredictions()
    {
        ///The values below should change with different datasets.
        float minX = -3.193372f;
        float maxX = 4.864759f;
        float minZ = -3.138325f;
        float maxZ = 2.95114f;
        if (python.Receive == null)
            return;
        //for (int i = 0; i < agentsGridScriptsList.Count; i++)
        //{   //Updates acceleration that was predicted.
            //Predicted values are normalized,so i transform them back.
            float newX = float.Parse(python.Receive[agentNavigationScript.agentIndex].Split(',')[0]) * (maxX - minX) + minX;
            float newZ = float.Parse(python.Receive[agentNavigationScript.agentIndex].Split(',')[1]) * (maxZ - minZ) + minZ;
            //Debug.Log(agentsNavScriptsList[i].agentNavMesh.velocity);

            ///Tried both,even though the i think the current one is better.
            agentNavigationScript.agentNavMesh.velocity = agentNavigationScript.agentNavMesh.velocity + new Vector3(newX, 0, newZ);//*Time.deltaTime;
        //}
        python.Receive = null;
    }
    
}
