using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AgentTrain : MonoBehaviour
{


    public const bool ONLINE = false;
    public const int ANGLECATEGORIES = 5;
    protected Vector3 target;
    protected float velocity = 1f;
    protected Vector2 velocityVector;
    public GameObject sonToFollow;
    float justReceived = 0;
    float[] boundsx = { -12, 12 };
    float[] boundsz = { -12, 12 };
    public const float MAXSPEED = 5;
    public const float MINSPEED = -1.5f;
    protected float deciding = 0;

    public float[] features;// = new float[ANGLECATEGORIES+3];
    float NNacceleration = 0;
    float NNrotation = 0;

    public class NearAgent: IComparable<NearAgent>
    {
        public float distance;
        float angleFromOriginalAgent;
        float angleOfMovementRelativeToAgentsMovement;
        float speedOfNearAgent;

        public NearAgent()
        {
            distance = 8;
            angleFromOriginalAgent = -100;
            speedOfNearAgent = 0;
            angleOfMovementRelativeToAgentsMovement = -0.5f;
        }

        public NearAgent(float distance,float angleFromOriginalAgent,float angleOfMovementRelativeToAgentsMovement, float speedOfNearAgent)
        {
            this.distance = distance;
            this.angleFromOriginalAgent = angleFromOriginalAgent;
            this.speedOfNearAgent = speedOfNearAgent;
            this.angleOfMovementRelativeToAgentsMovement = angleOfMovementRelativeToAgentsMovement;
        }
        public override String ToString()
        {
            return distance + "," + angleFromOriginalAgent + "," + speedOfNearAgent + "," + angleOfMovementRelativeToAgentsMovement+",";
        }



        

        public int CompareTo(NearAgent other)
        {
            if (distance < other.distance)
                return -1;
            if (distance == other.distance)
                return 0;
            return 1;
        }
    }


    public GameObject prefabGoal;
    protected GameObject goal;
    protected float angleToGoal;
    protected float distanceVar;
    protected float previousDistanceVar=0;
    protected List<NearAgent> allNearAgents = new List<NearAgent>();

    


    public List<NearAgent> kNearest(int k)
    {
        
        while (allNearAgents.Count < k)
        {
            allNearAgents.Add(new NearAgent());
            if (allNearAgents.Count == k)
                return allNearAgents;
        }



        allNearAgents.Sort();

        List<NearAgent> toRet = new List<NearAgent>();
        for (int i = 0; i < k; i++)
        {
            toRet.Add(allNearAgents[i]);
        }

        return toRet;
    }
    
    public void changeSpeed(float byValue)
    {
//        Debug.Log(byValue);
        if ((sonToFollow.transform.localPosition.z + byValue) <= MAXSPEED && (sonToFollow.transform.localPosition.z + byValue) >= MINSPEED)
            sonToFollow.transform.localPosition = sonToFollow.transform.localPosition + new Vector3(0, 0, byValue);
        else
        {
            if ((sonToFollow.transform.localPosition.z + byValue) < MINSPEED)
                sonToFollow.transform.localPosition = new Vector3(0, 0, MINSPEED);
            if((sonToFollow.transform.localPosition.z + byValue) > MAXSPEED)
            {
                sonToFollow.transform.localPosition = new Vector3(0, 0, MAXSPEED);
            }
        }
    }

    public void initFeatures()
    {
        for (int i = 0; i < features.Length; i++)
        {

            features[i] = 0f;
        }

    }
    public void initFeatures2()
    {
        for (int i = 0; i < ANGLECATEGORIES; i++)
        {

            features[ANGLECATEGORIES*2+i] = features[ANGLECATEGORIES  + i];
            features[ANGLECATEGORIES + i] = features[i];
            features[i] = 0;

        }

    }


    public Vector2 getXandZ()
    {
        float xStart = UnityEngine.Random.Range(boundsx[0], boundsx[1]);
        float zStart = UnityEngine.Random.Range(boundsz[0], boundsz[1]);

        float x = Mathf.Pow(xStart - transform.position.x, 2);
        float z = Mathf.Pow(zStart - transform.position.z, 2);
        float euclidian = Mathf.Sqrt(x + z);
        //Debug.Log(euclidian);

        while (euclidian < 5)
        {
            //Debug.Log(euclidian);
            xStart = UnityEngine.Random.Range(boundsx[0], boundsx[1]);
            zStart = UnityEngine.Random.Range(boundsz[0], boundsz[1]);
            x = Mathf.Pow(xStart - transform.position.x, 2);
            z = Mathf.Pow(zStart - transform.position.z, 2);
            euclidian = Mathf.Sqrt(x + z);
        }

        return new Vector2(xStart, zStart);


    }

    public bool bounds(float[] x, float[]y,float checkX,float checkY)
    {
        bool[] test = new bool[x.Length];
        for(int i = 0; i < x.Length; i++)
        {
            if(i==0 && x[i]>checkX&& y[i] > checkY)
            {
                test[i] = true;
            }
            if(i == 1 && x[i] < checkX && y[i] < checkY)
            {
                test[i] = true;
            }
        }
        if (test[0] && test[1])
            return true;
        return false;
    }
    public string distanceCompare()
    {
        if (previousDistanceVar > distanceVar)
            return "0";
        return "1";
    }

    public void distanceToGoal()
    {
        if (goal != null) {
            previousDistanceVar = distanceVar;
            distanceVar = distance(goal.transform.position, transform.position);
        }
        //Debug.Log(distanceVar);  
    }

    public void createGoal() {

        Vector2 goalPosition = getXandZ();
        goal = Instantiate(prefabGoal, new Vector3(goalPosition.x, 1, goalPosition.y), new Quaternion());
        getAngleToGoal();

    }

    protected void getAngleToGoal()
    {
        if (goal != null)
        {
            goal.transform.SetParent(transform);
            angleToGoal= Mathf.Atan2(goal.transform.localPosition.x, goal.transform.localPosition.z) * (180 / Mathf.PI);
            goal.transform.SetParent(null);
        }
       
    }

    protected virtual bool destroyGoal()
    {
        float x = Mathf.Pow(goal.transform.position.x - transform.position.x, 2);
        float z = Mathf.Pow(goal.transform.position.z - transform.position.z, 2);
        float euclidian = Mathf.Sqrt(x + z);
        if (euclidian < 1f)
        {
            deciding = 1;
            Destroy(goal);
            createGoal();
            return true;
        }
        return false;
    }

    public static Quaternion rotate(Vector3 refr, float x, float y, float z)
    {
        return Quaternion.Euler(refr.x + x, refr.y + y, refr.z + z);
    }
    /* public void velocityToRealWorld()
     {
         float angle = transform.rotation.eulerAngles.y+90;
         velocityVector = new Vector2(velocity * Mathf.Cos(angle), velocity * Mathf.Sin(angle));
        // Debug.Log(angle);
         Debug.Log(velocityVector);
     }
     */

    public string state()
    {
        //if (goal == null)
        //   return null;
        distanceToGoal();
        string toWrite = "";
        for (int i = 0; i < features.Length; i++)
        {
            
                toWrite += features[i] + ",";
        }
        toWrite += angleToGoal/180 + ",";
        toWrite += sonToFollow.transform.localPosition.z + ",";
        toWrite += deciding + ",";
        toWrite += distanceVar + ",";
        toWrite += distanceCompare() + "\n";

        //Debug.Log(toWrite);
        return toWrite;
    }

    public void setRotationAcceleration(float toSetRot,float toSetAccel)
    {
        //Debug.Log(toSet);
        NNrotation1 = toSetRot;
        justReceived = toSetRot * 50 * AutoTrainController.timeFromSendReceive;
        NNacceleration = toSetAccel;
    }

    public void getSpeed()
    {
        Vector3 tmpson = sonToFollow.transform.position - transform.position;

        velocityVector = new Vector2(tmpson.x, tmpson.z);

    }
    public void move()
    {
        if (!ONLINE)
        {
           // angleToGoal = 0;
            NNrotation1 = angleToGoal * Time.deltaTime;
        }
        if(NNacceleration!=0)
            changeSpeed(NNacceleration * (Time.deltaTime+(justReceived/ (NNrotation1 * 50))));

        Vector3 tmpson = sonToFollow.transform.position - transform.position;
        tmpson.y = 0;
        
        if (goal != null) { 
            transform.rotation = rotate(transform.rotation.eulerAngles, 0, (NNrotation1 * 50 * (Time.deltaTime))+justReceived, 0);
            justReceived = 0;
        }

        transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        transform.position += tmpson * Time.deltaTime;

        velocityVector = new Vector2(tmpson.x, tmpson.z);


    }





    bool filterDegree(int category)
    {
        if (category < 0 || category > ANGLECATEGORIES-1)
            return false;
        return true;
    }

    /*
    /// <summary>
    /// Detects the agents position relative to player.
    /// Features are the future distance divided by time to meet.
    /// </summary>
    protected void detectAgents()
    {
        initFeatures();
        int angles = 0;
        float distance = 0f;
        foreach (GameObject agent in AutoTrainController.agents)
        {
            if (agent == this.gameObject);
            else
            {
                agent.transform.SetParent(transform);
                angles = (int)((Mathf.Atan2(agent.transform.localPosition.x, agent.transform.localPosition.z) * (180 / Mathf.PI)) + 100) / 10;
               
                if (filterDegree(angles))
                {
                    //Debug.Log(angles);
                   distance = Mathf.Sqrt(Mathf.Pow(agent.transform.localPosition.x, 2) + Mathf.Pow(agent.transform.localPosition.z, 2));

                    //Debug.Log(giveCosth(agent.GetComponent<AgentTrain>().velocityVector));
                    //features[angles] += ((1 / distance) * -giveCosth(agent.GetComponent<AgentTrain>().velocityVector));
                    float tmp = agentRunOverMe(agent.GetComponent<AgentTrain>());
                    if (tmp > feature[angles])
                        feature[angles] = agentRunOverMe(agent.GetComponent<AgentTrain>());
                    Debug.Log(feature[angles]);
                }
                agent.transform.SetParent(null);
            }
          
            


        }
    }
    */

    /*
/// <summary>
/// Detects the agents position relative to player.
/// Features are the future distance divided by time to meet.
/// </summary>
protected void detectAgents2()
{
    initFeatures();
    int angles = 0;
    float distance = 0f;
    foreach (GameObject agent in AutoTrainController.agents)
    {
        if (agent == this.gameObject);
        else
        {
            agent.transform.SetParent(transform);
            angles = (int)((Mathf.Atan2(agent.transform.localPosition.x, agent.transform.localPosition.z) * (180 / Mathf.PI)) + 100) / 10;
            if (filterDegree(angles))
            {
                //Debug.Log(angles);
                distance = Mathf.Sqrt(Mathf.Pow(agent.transform.localPosition.x, 2) + Mathf.Pow(agent.transform.localPosition.z, 2));                    
                features[angles] += ((1 / (1+(distance*distance))));
                if (agentRunOverMe2(agent.GetComponent<AgentTrain>())==1)
                {
                    features[ANGLECATEGORIES] = 1;
                    if (angles >= 10)
                    {
                        features[ANGLECATEGORIES+1] = 1;
                    }
                    else
                    {
                        features[ANGLECATEGORIES + 2] = 1;
                    }
                }
            }
            agent.transform.SetParent(null);
        }
    }
}
*/

    /// <summary>
    /// Creates features based on Density in angles.Feature of nearest agent in each angle and it's rotation*velocityMagnitude.
    /// Features are the future distance divided by time to meet.
    /// </summary>
    protected void detectAgents3()
    {
        initFeatures();
        int angles = 0;
        float distance = 0f;
        foreach (GameObject agent in AutoTrainController.agents)
        {
            if (agent == this.gameObject) ;
            else
            {
                agent.transform.SetParent(transform);
                angles = (int)((Mathf.Atan2(agent.transform.localPosition.x, agent.transform.localPosition.z) * (180 / Mathf.PI)) + 100) / 40;
                if (filterDegree(angles))
                {
                    //Debug.Log(angles);
                    distance = Mathf.Sqrt(Mathf.Pow(agent.transform.localPosition.x, 2) + Mathf.Pow(agent.transform.localPosition.z, 2));
                    features[angles] += ((1 / (1 + (distance * distance))));
                    if(features[ANGLECATEGORIES + angles] < (1/distance))
                    {
                        features[ANGLECATEGORIES + angles] =1/distance;
                        // features[ANGLECATEGORIES*2 + angles] = agent.GetComponent<AgentTrain>().NNrotation1;

                        getSpeed();
                        agent.GetComponent<AgentTrain>().getSpeed();
                        features[ANGLECATEGORIES*2 + angles] = giveCosth(agent.GetComponent<AgentTrain>().velocityVector);

                    }

                    if (agentRunOverMe2(agent.GetComponent<AgentTrain>()) == 1)
                    {
                        features[ANGLECATEGORIES*3] = 1;
                        if (angles >= ANGLECATEGORIES/2)
                        {
                            features[ANGLECATEGORIES * 3 + 1] = 1;
                        }
                        else
                        {
                            features[ANGLECATEGORIES * 3 + 2] = 1;
                        }
                    }
                }
                agent.transform.SetParent(null);
            }
        }
    }






    protected void detectAgents4()
    {
        initFeatures2();
        int angles = 0;
        float distance = 0f;
        foreach (GameObject agent in AutoTrainController.agents)
        {
            if (agent == this.gameObject) ;
            else
            {
                agent.transform.SetParent(transform);
                angles = (int)((Mathf.Atan2(agent.transform.localPosition.x, agent.transform.localPosition.z) * (180 / Mathf.PI)));
                //Debug.Log(angles);
                if(angles<0)
                    Debug.Log(-((agent.transform.localEulerAngles.y) - 180) / 180);
                else
                    Debug.Log(((agent.transform.localEulerAngles.y) - 180) / 180);

                angles = (int)((Mathf.Atan2(agent.transform.localPosition.x, agent.transform.localPosition.z) * (180 / Mathf.PI)) + 100) / 40;
                if (filterDegree(angles))
                {
                    //Debug.Log(angles);
                    distance = Mathf.Sqrt(Mathf.Pow(agent.transform.localPosition.x, 2) + Mathf.Pow(agent.transform.localPosition.z, 2));
                    if (features[ANGLECATEGORIES + angles] < (1 / distance))
                    {
                        //features[ANGLECATEGORIES + angles] = 1 / distance;
                        // features[ANGLECATEGORIES*2 + angles] = agent.GetComponent<AgentTrain>().NNrotation1;

                        getSpeed();
                        agent.GetComponent<AgentTrain>().getSpeed();
                        features[angles] =  Mathf.Acos(giveCosth(agent.GetComponent<AgentTrain>().velocityVector))*(180 / Mathf.PI);
                        //Debug.Log(features[angles]);
                        Vector3 ok = new Vector3(velocityVector.x, 0, velocityVector.y);
                        Vector3 ok1 = new Vector3(agent.GetComponent<AgentTrain>().velocityVector.x, 0, agent.GetComponent<AgentTrain>().velocityVector.y);
                        
                        //Debug.Log(Vector3.Cross(ok, ok1).y);

                    }
                    
                }
                //Debug.Log(((agent.transform.localEulerAngles.y)-180)/180);

                agent.transform.SetParent(null);
                //Debug.Log(-(transform.rotation.eulerAngles.y-180));
                //Debug.Log(-(agent.transform.rotation.eulerAngles.y-180));


                //Debug.Log(((-(transform.rotation.eulerAngles.y - 180)) - (-(agent.transform.rotation.eulerAngles.y - 180))));
                
            }
        }
    }


    protected void detectAgents5()
    {
        //initFeatures2();
        int angles = 0;
        float relativeAngle = 0;
        float distance = 0f;
        allNearAgents = new List<NearAgent>();
        foreach (GameObject agent in AutoTrainController.agents)
        {
            if (agent == this.gameObject) ;
            else
            {
                agent.transform.SetParent(transform);
                angles = (int)((Mathf.Atan2(agent.transform.localPosition.x, agent.transform.localPosition.z) * (180 / Mathf.PI)));
                distance = Mathf.Sqrt(Mathf.Pow(agent.transform.localPosition.x, 2) + Mathf.Pow(agent.transform.localPosition.z, 2));
                if (angles>-100 && angles < 100 &&distance<(8*4))
                {
                    
                    if (angles < 0)
                        relativeAngle=(-((agent.transform.localEulerAngles.y) - 180) / 180);
                    else
                        relativeAngle=(((agent.transform.localEulerAngles.y) - 180) / 180);
                    
                    agent.GetComponent<AgentTrain>().getSpeed();
                    allNearAgents.Add(new NearAgent(distance/4, angles, relativeAngle, agent.GetComponent<AgentTrain>().velocityVector.magnitude));
                    //Debug.Log(allNearAgents[0].ToString());
                }
                agent.transform.SetParent(null);
            }
        }
    }











    float distance(Vector3 i,Vector3 j)
    {
        return Mathf.Sqrt(Mathf.Pow(i.x-j.x, 2) + Mathf.Pow(i.z-j.z, 2));
    }
    /*
    float agentRunOverMe(AgentTrain agent)
    {
        Vector3 myAgentSon = sonToFollow.transform.position - transform.position;
        Vector3 otherAgentSon = agent.sonToFollow.transform.position - agent.transform.position;
        Vector3 myAgentPos = transform.position;
        Vector3 otherAgentpos = agent.transform.position;
        myAgentSon.y = 0;
        otherAgentSon.y = 0;
        // float lowestDistance = distance(myAgentPos, otherAgentpos);
        float lowestDistance = 1000;

        bool startedIncreasing = false;
        float timeOfLowestDistance=1;
        int i = 0;
        float timeLookedForward = 0f;
        float tmpLowestDistance = lowestDistance;

        do
        {
            i++;
            timeLookedForward += 0.1f;
            lowestDistance = distance(myAgentPos + myAgentSon * timeLookedForward, otherAgentpos + otherAgentSon * timeLookedForward);
            //Debug.Log(lowestDistance);

            if (lowestDistance > tmpLowestDistance)
            {
                lowestDistance = tmpLowestDistance;
                startedIncreasing = true;
            }
            else
            {
                tmpLowestDistance = lowestDistance;
                timeOfLowestDistance = i;
            }
        } while (!startedIncreasing && lowestDistance > LOWESTDISTANCETHRESHHOLD && timeLookedForward < 3);


        
        return filterLowestDistance(lowestDistance)/ Mathf.Sqrt(Mathf.Sqrt(timeOfLowestDistance));
        
    }
    */

    float agentRunOverMe2(AgentTrain agent)
    {
        Vector3 myAgentSon = sonToFollow.transform.position - transform.position;
        Vector3 otherAgentSon = agent.sonToFollow.transform.position - agent.transform.position;
        Vector3 myAgentPos = transform.position;
        Vector3 otherAgentpos = agent.transform.position;
        myAgentSon.y = 0;
        otherAgentSon.y = 0;
        // float lowestDistance = distance(myAgentPos, otherAgentpos);
        float lowestDistance = 1000;

        bool startedIncreasing = false;
        float timeOfLowestDistance = 1;
        int i = 0;
        float timeLookedForward = 0f;
        float tmpLowestDistance = lowestDistance;
        float angles = 0;
        do
        {
            i++;
            timeLookedForward += 0.1f;
            lowestDistance = distance(myAgentPos + myAgentSon * timeLookedForward, otherAgentpos + otherAgentSon * timeLookedForward);
            //Debug.Log(lowestDistance);

            if (lowestDistance > tmpLowestDistance)
            {
                lowestDistance = tmpLowestDistance;
                startedIncreasing = true;
            }
            else
            {
                tmpLowestDistance = lowestDistance;
                if (tmpLowestDistance < LOWESTDISTANCETHRESHHOLD)
                {
                    return 1;
                }
                timeOfLowestDistance = i;
            }
        } while (!startedIncreasing && lowestDistance > LOWESTDISTANCETHRESHHOLD && timeLookedForward < 3);

        

        return 0;

    }

    const float LOWESTDISTANCETHRESHHOLD= 0.35f;

    public float NNrotation1
    {
        get
        {
            return NNrotation;
        }

        set
        {
            NNrotation = value;
        }
    }

    float filterLowestDistance(float lowestDistance) {
        if (lowestDistance < LOWESTDISTANCETHRESHHOLD)
        {
            return 1;
        }
        else
        {
            return LOWESTDISTANCETHRESHHOLD / lowestDistance;
        }
    }


    float giveCosth(Vector2 relativeToThisOne)
    {
        return (velocityVector.x * relativeToThisOne.x + velocityVector.y * relativeToThisOne.y)/(velocityVector.magnitude*relativeToThisOne.magnitude);
    }


    void Start()
    {
        //features = new float[(ANGLECATEGORIES*3 + 3)];
        features = new float[(ANGLECATEGORIES * 3)];
        createGoal();
    }

    protected void UpdateDecidingCourse()
    {
        if (deciding > 0)
            deciding -= Time.deltaTime;
        else deciding = 0;
    }

    void Update()
    {
        UpdateDecidingCourse();
        move();
        getAngleToGoal();
        if(ONLINE)
            detectAgents5();
        destroyGoal();
        
        

    }

}
