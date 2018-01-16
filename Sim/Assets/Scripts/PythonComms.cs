using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.IO;
using System.Diagnostics;

public class PythonComms : MonoBehaviour {

    public string pythonExe;
    public string pyFile;

    bool quit = false;
    string previousStates;
    string AgentsStates;
    string[] receive;
    Thread predictor;
    public string[] Receive
    {
        get
        {
            return receive;
        }

        set
        {
            receive = value;
        }
    }
    public string AgentsStates1
    {
        get
        {
            return AgentsStates;
        }

        set
        {
            AgentsStates = value;
        }
    }
    public Thread Predictor
    {
        get
        {
            return predictor;
        }

        set
        {
            predictor = value;
        }
    }
    public void startPredictions()
    {
        predictor = new Thread(() =>
        {
            Thread.CurrentThread.IsBackground = true;
            Process myProcess = new Process();
            //Provide the start information for the process
			myProcess.StartInfo.FileName = @"C:\Users\kokos\Anaconda3\python.exe";
			myProcess.StartInfo.Arguments = @"..\AmyrSimData\Trainee2.py";                   
            ///You have to give your own paths like above(change it in unity editor if you want)
            //myProcess.StartInfo.FileName = @pythonExe;
            //myProcess.StartInfo.Arguments = @pyFile;
            myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.RedirectStandardInput = true;
            myProcess.StartInfo.RedirectStandardOutput = true;
            StreamReader myStreamReader;
            StreamWriter myStreamWriter = null;
            myProcess.Start();
            Thread.Sleep(10000);
            myStreamWriter = myProcess.StandardInput;
            myStreamWriter.WriteLine(AgentsStates1);
				while (true&&!quit)
            {
                while (AgentsStates1 == null&&!quit) ;
                myProcess.WaitForInputIdle();
                UnityEngine.Debug.Log(AgentsStates1);
                myStreamWriter.WriteLine(AgentsStates1);
                previousStates = AgentsStates1;
                myStreamWriter.Flush();
                string myString = myProcess.StandardOutput.ReadLine();

                if (myString.Length > 1)
                {
                    while (receive != null && !quit) ;
                    receive = myString.Split(':');
                    UnityEngine.Debug.Log(receive[0]);

                    while (previousStates.Equals(AgentsStates1) && !quit)
                        Thread.Sleep(14);
                }
            }
        });
        predictor.Start();
    }   
    void OnApplicationQuit()
    {
        quit = true;
    }




}
