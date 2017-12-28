using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Player : AgentTrain
{
    
    int width ;
    public GameObject camera;
    public GameObject angleText;
    Text angleTxt = null;
    float towards;
    const float THRESHOLD = 0.005f;
    float accel = 0;

    private void Start()
    {
        features = new float[ANGLECATEGORIES*3+3];
        width = Screen.currentResolution.width;
        createGoal();
    }


    void rotateAndmoveMouse()
    {
        float x = (Input.mousePosition.x);
        x = (x - width / 2) / width;
        float tanx = x;
        changeSpeed(accel * Time.deltaTime);
        towards = Mathf.Tan(tanx / 1) * 2f;
        transform.rotation = rotate(transform.rotation.eulerAngles, 0, Mathf.Tan(tanx / 1)*100f*Time.deltaTime, 0);
        Vector3 tmpson = sonToFollow.transform.position - transform.position;
        tmpson.y = 0;
        velocityVector = new Vector2(tmpson.x, tmpson.z);
        
        transform.position += tmpson * Time.deltaTime;
    }
   
    string state()
    {
        
        float compareToThreshold = 0;
        if (goal == null)
            return null;
        string toWrite = "";
        foreach (NearAgent a in kNearest(5))
            toWrite += a.ToString();
        /*for (int i = 0; i < features.Length; i++)
        {
            toWrite += features[i]+",";
            compareToThreshold += features[i];   
        }*/
        Debug.Log(toWrite);
        toWrite += angleToGoal/180+",";
        toWrite += sonToFollow.transform.localPosition.z + ",";
            //    toWrite += sonToFollow.transform.localPosition.z + ",";
        toWrite += deciding + ",";
        //toWrite += distanceVar + ",";
        //toWrite += distanceCompare() + ",";

        if (sonToFollow.transform.localPosition.z >= 4.99f)
        {
            toWrite += 0 + ",";
        }
        else if(sonToFollow.transform.localPosition.z <= 0.01f)
        {
            toWrite += 0 + ",";
        }
        else
        {
            toWrite += accel + ",";
        }
        toWrite += towards + "\n";
        //Debug.Log(features.Length);
        //if (compareToThreshold < THRESHOLD)
         //   toWrite = "";
        //Debug.Log(toWrite);
        return toWrite;
    }
      

    void appendToFile()
    {
        if (goal == null)
            return;
        File.AppendAllText("trainingData.txt", state());
    }

    void UpdateText()
    {
        if (angleTxt == null)
        {
            angleTxt = angleText.GetComponent<Text>();
        }
        if (goal != null)
        {
            angleTxt.text = (int)angleToGoal+"Degrees |"+ (int)distanceVar +"from goal";
        }
        else
        {
            angleTxt.text = "";
        }
    }


    public const float MAXACCELERATION=3.5f;


    protected override bool destroyGoal()
    {
        bool a=base.destroyGoal();
        if(a)
            accel = -MAXACCELERATION;
        return false;
    }

    public void Update()
    {
        UpdateDecidingCourse();

        if (Input.GetKey(KeyCode.W))
        {
            accel = MAXACCELERATION;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            accel = -MAXACCELERATION;
        }
        else
            accel = 0;

        if(!ONLINE)
            detectAgents5();
        rotateAndmoveMouse();
        getAngleToGoal();
        distanceToGoal();
        UpdateText();
        
        destroyGoal();
        if (!ONLINE)
            appendToFile();
        //RotateObject();
        // Debug.Log(velocityVector.magnitude);
        //velocityToRealWorld();
        //Debug.Log(sonToFollow.transform.position);

        //transform.Translate(tmpson * Time.deltaTime);



    }
}