using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO.Pipes;
using System.IO;
public class PythonSocketConnection : MonoBehaviour
{

    [HideInInspector] public bool isTxStarted = false;

    [SerializeField] string IP = "127.0.0.1"; // local host
    [SerializeField] int rxPort = 8000; // port to receive data from Python on
    [SerializeField] int txPort = 8001; // port to send data to Python on

    int i = 0;

    UdpClient client;
    IPEndPoint remoteEndPoint;
    Thread recieveThread;

    void WriteToFile ()
    {

    }


    public async void SendData (string message)
    {

        string path = Application.temporaryCachePath + "/" + outputName;
        await System.Threading.Tasks.Task.Run( () =>
        {

            
            Debug.Log("Outputting to : " + path);

            File.WriteAllTextAsync(path, message);
         

        });

        Debug.Log("Outputted");

        try
        {
            byte[] data = Encoding.UTF8.GetBytes(Application.temporaryCachePath + "/" + outputName);
            client.Send(data, data.Length, remoteEndPoint);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }

    }

    public  void SendNetMessage (string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            client.Send(data, data.Length, remoteEndPoint);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }

    System.Diagnostics.Process p;

    string outputName;

    private void Awake()
    {
        outputName = DateTime.Now.Millisecond.ToString() + ".py";
        p = System.Diagnostics.Process.Start(Application.streamingAssetsPath + "/output/socket-python-completer/socket-python-completer.exe");

        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), txPort);

        client = new UdpClient(rxPort);

        recieveThread = new Thread(new ThreadStart(RecieveData));
        recieveThread.IsBackground = true;
        recieveThread.Start();


    }

    private void RecieveData ()
    {
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data);
                ProcessInput(text);
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }


    private void ProcessInput(string input)
    {

        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            GetComponent<PythonInterpreter>().RecievePythonIntellisenseSuggestions(input);
        });

        // PROCESS INPUT RECEIVED STRING HERE

        if (!isTxStarted) // First data arrived so tx started
        {
            isTxStarted = true;
        }
    }

    void OnDisable()
    {
        SendNetMessage("END");

        if (p != null)
        {
            p.Kill();
        }

        if (recieveThread != null)
            recieveThread.Abort();

        client.Close();
    }
}
