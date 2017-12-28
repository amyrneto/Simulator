 using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ConnectWithPython : MonoBehaviour {

    public static Socket clientSocket;


    public  static void init()
    {
         
        IPEndPoint serverAddress = new IPEndPoint(IPAddress.Parse("192.168.0.4"), 1002);
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        clientSocket.Connect(serverAddress);
        string msg = "test,test,test";
       // clientSocket.Send(System.Text.Encoding.UTF8.GetBytes(msg));
       
        
    }

    public static void initLocal()
    {

        IPEndPoint serverAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1002);
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        clientSocket.Connect(serverAddress);
        string msg = "test,test,test";
        //clientSocket.Send(System.Text.Encoding.UTF8.GetBytes(msg));
    }

    public static void send(string msg)
    {
        //Debug.Log(System.Text.Encoding.UTF8.GetBytes(msg).Length);
        
        clientSocket.Send(System.Text.Encoding.UTF8.GetBytes(msg));
        
    }

    public static bool tryReceive(List<GameObject> agents,GameObject player)
    {
        
        byte[] buff=new byte[1000];
        clientSocket.ReceiveTimeout = 15;
        clientSocket.Receive(buff);
        //Debug.Log(System.Text.Encoding.UTF8.GetString(buff));
        string[] lines = System.Text.Encoding.UTF8.GetString(buff).Split('\n');
        Debug.Log(System.Text.Encoding.UTF8.GetString(buff));
        int i = 0;
        AgentController.waitingNews = false;
        foreach (GameObject agent in agents)
        {
            if (agent == player) ;
            else
            {
                agent.GetComponent<AgentTrain>().setRotationAcceleration(float.Parse(lines[i].Split(',')[0]), float.Parse(lines[i].Split(',')[1]));
                i++;
            }
            
        }
        Debug.Log("Returning");
        return true;
    }

    
    public static bool tryReceive(List<GameObject> agents)
    {

        byte[] buff = new byte[1000];
        clientSocket.ReceiveTimeout = 15;
        clientSocket.Receive(buff);
        //Debug.Log(System.Text.Encoding.UTF8.GetString(buff));
        string[] lines = System.Text.Encoding.UTF8.GetString(buff).Split('\n');
        int i = 0;
        AgentController.waitingNews = false;
        foreach (GameObject agent in agents)
        {
            agent.GetComponent<Agent>().applyAcceleration(float.Parse(lines[i].Split(',')[0]), float.Parse(lines[i].Split(',')[1]));
            i++;
        }
        Debug.Log("Returning");
        return true;
    }

    
    private class myEvent : SocketAsyncEventArgs
    {
        protected override void OnCompleted(SocketAsyncEventArgs e)
        {
            base.OnCompleted(e);
            Debug.Log(System.Text.Encoding.UTF8.GetString(e.Buffer));

            
        }
    }
}
