using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ObjectivesManager : MonoBehaviour {

    public int numAgents, obstacleStep;
    public List<Transform> objectivesList;
    public Terrain terrain;
    public Transform objectivePrefab, agentPrefab;
    public string fileName;

    public int gridW, gridH;
    public int posX, posZ;
    public bool centered;

    private int state;
    private List<Transform> agentsList;
    private bool simulating=false;
    private StreamWriter outputFile;

	// Use this for initialization
	void Start () {
        outputFile = new StreamWriter(fileName);
        outputFile.WriteLine(gridW + "," + gridH + "," + posX  + "," + posZ + "," + centered);
        state = 0;
        objectivesList = new List<Transform>();
        agentsList = new List<Transform>();
        for (int i= (int)terrain.terrainData.bounds.min.x; i< (int)terrain.terrainData.bounds.max.x; i+= obstacleStep)
        {
            for (int j = (int) terrain.terrainData.bounds.min.z; j < (int)terrain.terrainData.bounds.max.z; j+= obstacleStep)
            {
                Transform t;
                t = Instantiate (objectivePrefab, new Vector3 (terrain.GetPosition().x + (float)i + (obstacleStep/2), objectivePrefab.localScale.y, terrain.GetPosition().z + (float)j + (obstacleStep / 2)), Quaternion.identity);
                objectivesList.Add(t);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        switch (state)
        {
            case 0: // Build list with valid goals
                simulating = false;
                BuildList();
                state = 1;
                break;
            case 1: //  Instantiate agents
                for (int i = 0; i < numAgents; i++)
                {
                    // Set a random position for the agent
                    int index = (int) Random.Range(0.0f, objectivesList.Count - 1);
                    Vector3 pos = objectivesList[index].position;
                    pos.y = agentPrefab.localScale.y;
                    Transform t;
                    t = Instantiate(agentPrefab, pos, Quaternion.identity);
                    t.GetComponent<AgentNavigationScript>().objectiveManager = this.transform;
                    t.GetComponent<AgentGridScript>().posX = posX;
                    t.GetComponent<AgentGridScript>().posZ = posZ;
                    t.GetComponent<AgentGridScript>().gridH = gridH;
                    t.GetComponent<AgentGridScript>().gridW = gridW;
                    t.GetComponent<AgentGridScript>().centered = centered;
                    t.GetComponent<AgentGridScript>().BuildGrid();
                    agentsList.Add(t);
                }
                state = 2;
                break;
            case 2:
                // Start the agent(s)
                for (int i = 0; i < agentsList.Count; i++)
                {
                    agentsList[i].GetComponent<AgentNavigationScript>().state = 1;
                }
                state = 3;
                break;
            case 3: // Idle
                simulating = true;
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    state = 4;
                }
                break;
            case 4:
                simulating = false;
                outputFile.Close();
                break;
            case 5:
                break;
            default:
                state = 3;
                break;
        }
		
	}

   void FixedUpdate()
   {
        string s;
        if (simulating)
        {
            s = Time.timeSinceLevelLoad.ToString();
            s += ", ";
            outputFile.WriteLine(s);
        }
   }

    void BuildList()
    {
        //Debug.Log("Build List"+ objectivesList.Count);
        //foreach(Transform t in objectivesList)
        for (int i=0; i< objectivesList.Count; i++)
        {
            Transform t = objectivesList[i];
            ObjectiveScript obSc = t.GetComponent<ObjectiveScript>();
            if (!obSc.valid)
            {
                objectivesList.RemoveAt(i);
                Destroy(t.gameObject);
                i = 0;
            }
            else
            {
                t.GetComponent<Collider>().enabled = false;
                obSc.Hide();
            }
        }
        //Debug.Log("Build List" + objectivesList.Count);
    }
}
