using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyPredictionsScript : MonoBehaviour {

    string stateString="";
    /// <summary>
    /// True to use python Communication for movement.False for navmesh decisions.Maybe we can make it a public variable,so we can change it from editor.
    /// </summary>
    //public const bool LIVEPREDICTIONS = false; // This is not usefull
    /// <summary>
    /// Access to pythoncomms script,for sending and receiving states and actions.
    /// </summary>
    private PythonComms python;
    private List<AgentNavigationScript> agentNavigationScriptList;

    public string StateString
    {
        get
        {
            return stateString;
        }

        set
        {
            stateString = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        agentNavigationScriptList = GetComponent<ObjectivesManager>().AgentsNavScriptsList;
        initLivePredictions();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        setStatesLivePredictions(StateString);
        applyPredictions(); 
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
        //Debug.Log("In Apply");
        python.AgentsStates1 = s;
        //Debug.Log(python.AgentsStates1);
    }
    /// <summary>
    /// If we use predictions,checks whether new predictions were done,and apply them on each agent(hopefully).
    /// </summary>
    void applyPredictions()
    {
        ///The values below should change with different datasets.
        if (python.Receive == null)
            return;
        for (int i = 0; i < agentNavigationScriptList.Count; i++)
        {   //Updates acceleration that was predicted.
            //Predicted values are normalized,so i transform them back.
            agentNavigationScriptList[i].agentNavMesh.isStopped = true;
            //float newX = float.Parse(python.Receive[i].Split(',')[0]) * (maxX - minX) + minX;
            //float newZ = float.Parse(python.Receive[i].Split(',')[1]) * (maxZ - minZ) + minZ;
            float newX = float.Parse(python.Receive[i].Split(',')[0]);
            float newZ = float.Parse(python.Receive[i].Split(',')[1]);
            //Debug.Log(agentsNavScriptsList[i].agentNavMesh.velocity);
            //Tried both,even though the i think the current one is better.
            agentNavigationScriptList[i].agentNavMesh.velocity = agentNavigationScriptList[i].agentNavMesh.velocity + new Vector3(newX, 0, newZ);//*Time.deltaTime;
        }
        python.Receive = null;
    }
}
