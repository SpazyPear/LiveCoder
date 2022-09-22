using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MultiplayerDebugPlay : MonoBehaviour
{
    public string buildPath;
    Process build;
    NetworkManager networkManager;

    void Awake()
    {

        networkManager = FindObjectOfType<NetworkManager>();
        //NetworkManager.OnConnectedToLobbyCallback += OnConnectionEstablished;

        /*#if UNITY_EDITOR
                build = Process.Start(buildPath);
        #endif*/

    }

#if UNITY_EDITOR
    void Build()
    {
        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        string[] scenes = new string[sceneCount];
        for (int i = 0; i < sceneCount; i++)
        {
            scenes[i] = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
        }

        BuildPipeline.BuildPlayer(scenes, buildPath + "/LiveCoder.exe", BuildTarget.StandaloneWindows, BuildOptions.None);

        build = Process.Start(buildPath);

    }
#endif

    void OnConnectionEstablished(object sender, System.EventArgs e)
    {
        if (Application.isEditor)
        {
            networkManager.CreateRoom("debug");
        }
        else
        {
            networkManager.JoinRoom("debug");
        }
    }

    void OnApplicationQuit()
    {
        if (Application.isEditor)
        {
            Process.GetProcessesByName("LiveCoder")[0].Close();
        }
    }

}
