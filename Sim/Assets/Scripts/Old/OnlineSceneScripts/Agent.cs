using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Agent : MonoBehaviour {

    float velocityMagnitude;
    Vector2 velocity;
    Vector2 desiredVelocity;
    Vector2 diplacement;
    Vector2 velocityDifferenceNearest;
    Vector2 velocityDifferenceDesiredNearest;
    Vector2 target;
    public GameObject assigneGoal;
    float fullTime=10;
    float euclideanDistance;
    float xDist;
    float zDist;
    string state;
    float maccX=0;
    float maccY=0;

    bool initialized = false;

    public Vector2 DesiredVelocity
    {
        get
        {
            return desiredVelocity;
        }

        set
        {
            desiredVelocity = value;
        }
    }

    public Vector2 Velocity
    {
        get
        {
            return velocity;
        }

        set
        {
            velocity = value;
        }
    }

    public void giveControllerYourState()
    {

    }


    public void applyAcceleration(float accX,float accY)
    {
        maccX = accX/2;
        maccY = accY/2;

    }

    public string stateString()
    {   

        AgentController controller = GameObject.Find("AgentController").GetComponent<AgentController>();
        string toRet="";
        intendedVelocity();

        getDisplacemnt(GameObject.Find("AgentController").GetComponent<AgentController>());
        Vector2 velocityDifferenceDesired = getDifferenceToDesiredVelocity();
        velocityDifferenceDesiredNearest = desiredVelocity - controller.agents[getNearestNeighbor(controller)].GetComponent<Agent>().desiredVelocity;
        velocityDifferenceNearest=velocity- controller.agents[getNearestNeighbor(controller)].GetComponent<Agent>().velocity;
        toRet = diplacement.x + "," + diplacement.y + "," + velocityDifferenceDesiredNearest.x + "," + velocityDifferenceDesiredNearest.y + ","
            + velocityDifferenceNearest.x + "," + velocityDifferenceNearest.y + "," + Velocity.magnitude + "," + velocityDifferenceDesired.x + "," + velocityDifferenceDesired.y + "\n";
        //Debug.Log(toRet);
        return toRet;
    }

     
    void init()
    {
        float x = Mathf.Pow(transform.position.x - assigneGoal.transform.position.x, 2);
        float z = Mathf.Pow(transform.position.z - assigneGoal.transform.position.z, 2);
        euclideanDistance = Mathf.Sqrt(x + z);

        xDist = -(transform.position.x - assigneGoal.transform.position.x);
        zDist = -(transform.position.z - assigneGoal.transform.position.z);
        Velocity = new Vector2(xDist / fullTime, zDist / fullTime);
        //        target=

    }


    public int getNearestNeighbor(AgentController controller)
    {
        float x;
        float z;
        int index = 0;
        int currentIndex = 0;
        float distance = 100000f;
        foreach(GameObject currentAgent in controller.agents)
        {
            Agent current = currentAgent.GetComponent<Agent>();
            if (current != this)
            {
                 x = Mathf.Pow(transform.position.x - current.transform.position.x, 2);
                 z = Mathf.Pow(transform.position.z - current.transform.position.z, 2);
                if (Mathf.Sqrt(x + z) < distance)
                {
                    distance = Mathf.Sqrt(x + z);
                    index = currentIndex;
                }
            }
            else
            {
               // Debug.Log("SameGUy");
            }
            currentIndex++;
        }
        if (distance > 2) ;
           // return -1;

        return index;
    }

    public void intendedVelocity()
    {
        xDist = -(transform.position.x - assigneGoal.transform.position.x);
        zDist = -(transform.position.z - assigneGoal.transform.position.z);
        DesiredVelocity = new Vector2(xDist / fullTime, zDist / fullTime);
    }

    public Vector2 getDifferenceToDesiredVelocity()
    {
        return Velocity - DesiredVelocity;
    }

    public Vector2 getDisplacemnt(AgentController controller)
    {
        Vector3 pos = controller.agents[getNearestNeighbor(controller)].transform.position;
        Vector3 diff = pos - transform.position;
        //Debug.Log(diff);
        diplacement = new Vector2(diff.x, diff.z);
        Vector2 monadiaioVelocity = new Vector2(Velocity.x, Velocity.y);
        monadiaioVelocity.Normalize();
        
        //Debug.Log(monadiaioVelocity);
        //Debug.Log(monadiaioVelocity.normalized);
        diplacement = new Vector2((diplacement.x * monadiaioVelocity.y - diplacement.y * monadiaioVelocity.x), diplacement.x * monadiaioVelocity.x + (diplacement.y * monadiaioVelocity.y));

       // Debug.Log(diplacement+"displacement");



        return diplacement;
        
    }

    // Use this for initialization
    void Start () {
        if (assigneGoal == null)
        {
            return;
        }
        init();  
    }

    // Update is called once per frame
    void Update () {

        if(assigneGoal!=null && !initialized)
        {
            initialized = true;
            init();
        }


        velocity.x += maccX * Time.deltaTime;
        velocity.y += maccY * Time.deltaTime;

      //  Debug.Log(velocity.x);

        fullTime -= Time.deltaTime;
        if (fullTime < 0)
        {
            fullTime = 1;
           // Velocity = new Vector2();
        }
        //Debug.Log(Velocity);
        //Debug.Log(Velocity.magnitude);

        
        transform.Translate(new Vector3(Velocity.x,0,Velocity.y) * Time.deltaTime);
        //transform.
    }
}
