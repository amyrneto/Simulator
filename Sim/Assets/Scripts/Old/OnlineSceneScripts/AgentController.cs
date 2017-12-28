using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AgentController : MonoBehaviour {

    //public Ga toInstantiate;
    public GameObject toInstantiate;
    public GameObject goal;

    public int ok;

    float minx=-6;
    float miny = -6f;

    float maxx = 6;
    float maxy = 6f;
    float time = timeReset;
    const float timeReset = 0.05f;
    float tryTime = 0.05f;

    public static bool waitingNews = false;
    

    public List<GameObject> agents=new List<GameObject>();
    List<GameObject> goals = new List<GameObject>();
    // Use this for initialization
    void Start () {


        //        ConnectWithPython.init();
        ConnectWithPython.initLocal();

        float xStart= Random.Range(minx,maxx);
        float zStart = Random.Range(miny, maxy);

            while ((xStart > -5f && xStart < 5f) )
        {
            xStart = Random.Range(minx, maxx);
             zStart = Random.Range(miny, maxy);
        }

        agents.Add(Instantiate(toInstantiate, new Vector3(xStart, 0, zStart), new Quaternion()));

         xStart = Random.Range(minx, maxx);
         zStart = Random.Range(miny, maxy);
        while ((xStart > -5f && xStart < 5f))
        {
            xStart = Random.Range(minx, maxx);
            zStart = Random.Range(miny, maxy);
        }

        float x = Mathf.Pow(xStart - agents[0].transform.position.x, 2);
        float z = Mathf.Pow(zStart - agents[0].transform.position.z, 2);
        float euclidian = Mathf.Sqrt(x + z);
        //Debug.Log(euclidian);

        while (euclidian < 5)
        {
            xStart = Random.Range(minx, maxx);
            zStart = Random.Range(miny, maxy);
            while ((zStart > -5f && zStart < 5f))
            {
                xStart = Random.Range(minx, maxx);
                zStart = Random.Range(miny, maxy);
            }

            x = Mathf.Pow(xStart - agents[0].transform.position.x, 2);
             z = Mathf.Pow(zStart - agents[0].transform.position.z, 2);
             euclidian = Mathf.Sqrt(x + z);
            // Debug.Log(euclidian);
        }
    


        agents[0].GetComponent<Agent>().assigneGoal = Instantiate(goal, new Vector3(xStart, 0.5f, zStart), new Quaternion());
        goals.Add(agents[0].GetComponent<Agent>().assigneGoal);

        bool donotAdd = true;
        int agentToCreate =5;

        for (int i = 0; i < agentToCreate; i++)
        {

            do
            {
                donotAdd = false;
                xStart = Random.Range(minx, maxx);
                zStart = Random.Range(miny, maxy);

                while ((xStart > -5f && xStart < 5f))
                {
                    xStart = Random.Range(minx, maxx);
                    zStart = Random.Range(miny, maxy);
                }


                foreach (GameObject toCheck in agents)
                {
                     x = Mathf.Pow(xStart - toCheck.transform.position.x, 2);
                     z = Mathf.Pow(zStart - toCheck.transform.position.z, 2);
                     euclidian = Mathf.Sqrt(x + z);
                   // Debug.Log(euclidian);
                    if (euclidian < 1)
                        donotAdd = true;
                }



            } while (donotAdd);
            agents.Add(Instantiate(toInstantiate, new Vector3(xStart, 0, zStart), new Quaternion()));

            xStart = Random.Range(minx, maxx);
            zStart = Random.Range(miny, maxy);
            while ((xStart > -5f && xStart < 5f) )
            {
                xStart = Random.Range(minx, maxx);
                zStart = Random.Range(miny, maxy);
            }

             x = Mathf.Pow(xStart - agents[i + 1].transform.position.x, 2);
             z = Mathf.Pow(zStart - agents[i + 1].transform.position.z, 2);
             euclidian = Mathf.Sqrt(x + z);
            while (euclidian < 5)
            {
                xStart = Random.Range(minx, maxx);
                zStart = Random.Range(miny, maxy);
                while ((xStart > -5f && xStart < 5f))
                {
                    xStart = Random.Range(minx, maxx);
                    zStart = Random.Range(miny, maxy);
                }

                x = Mathf.Pow(xStart - agents[i+1].transform.position.x, 2);
                z = Mathf.Pow(zStart - agents[i+1].transform.position.z, 2);
                euclidian = Mathf.Sqrt(x + z);
              //  Debug.Log(euclidian);
            }

            agents[i+1].GetComponent<Agent>().assigneGoal = Instantiate(goal, new Vector3(xStart, 0.5f, zStart), new Quaternion());
            goals.Add(agents[i+1].GetComponent<Agent>().assigneGoal);






            donotAdd = true;

        }



    }
	
	// Update is called once per frame
	void Update () {

        time -= Time.deltaTime;
        tryTime -= Time.deltaTime;
        Debug.Log(waitingNews);

        if (waitingNews&&tryTime<0)
        {
            time = timeReset;
            tryTime = 0.05f;
            waitingNews = !ConnectWithPython.tryReceive(agents);
            
            //return;
            Debug.Log("wait");
        }


        else if (time < 0 &&!waitingNews)
        {
            string toSend = "";
            time = timeReset;
            tryTime = 0.05f;
            
            foreach (GameObject agent in agents)
            {
                toSend += agent.GetComponent<Agent>().stateString();
            }
            waitingNews = true;
            ConnectWithPython.send(toSend);
            //waitingNews = ConnectWithPython.tryReceive(agents);
           // Debug.Log(toSend);
        }


	}
}
