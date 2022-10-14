using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Reflection;

public class GameManager : MonoBehaviour, IOnEventCallback
{
    public static PhotonView photonView;
    public GridManager gridManager;
    public PythonInterpreter codeExecutor;
    public GameData gameData;
    public UIManager uiManager;
    public static Vector3 gridDimensions;
    public int numOfOreToSpawn;
    public GameObject orePrefab;
    public List<PlayerManager> players;
    [HideInInspector]
    public static PlayerManager activePlayer;
    public DragDropManager dragDropManager;
    int currentPhase;
    int playersReadied;
    public int numOfControlPoints;
    public int counter = 0;
    public static Dictionary<int, PlaceableObject> objectInstances = new Dictionary<int, PlaceableObject>(); 
    public static UnityEvent<int> OnPhaseChange = new UnityEvent<int>();

    // Start is called before the first frame update
    void Awake()
    {
        OnPhaseChange.AddListener(phaseEnter);
        State.gameManager = this;
        photonView = GetComponent<PhotonView>();
        activePlayer = FindObjectOfType<PlayerManager>();

        State.onLevelLoad.AddListener(initGameManager);
        GridManager.OnGridGenerated.AddListener(spawnControlPoints);
        State.initializeLevel();
    }

    void Start()
    {
       
    }

    public void initGameManager()
    {
        gridDimensions = new Vector3(gridManager.GridBreadth, GridManager.GridHeight, gridManager.GridWidth);
    }

    public void OnReadyUp()
    {
        if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected) { OnReadyUpRecieved(PhotonNetwork.LocalPlayer.UserId); }
        else { PhotonNetwork.RaiseEvent(0, PhotonNetwork.LocalPlayer.UserId, new RaiseEventOptions(), new SendOptions()); }
    }

    void OnReadyUpRecieved(string player)
    {
        playersReadied++;
        if (playersReadied >= 2)
        {
            playersReadied = 0;
            incrementPhase();
        }
        else
        {
            if (!PhotonNetwork.IsConnected) activePlayer.isLeftSide = false;
            GameManager.CallRPC(this, "playerReady", RpcTarget.All, player);
        }
    }

    public void incrementPhase()
    {
        currentPhase++;
        if (currentPhase == 2)
            currentPhase = 0;

        OnPhaseChange.Invoke(currentPhase);
    }

    [PunRPC]
    public void replicatedRunCode()
    {
        codeExecutor.RunCode();
    }

    public void phaseEnter(int phase)
    {
        switch (phase)
        {
            case 0:
                break;
            case 1:
                GameManager.CallRPC(this, "replicatedRunCode", RpcTarget.All);
                break;
        }
    }


    public static void CallRPC(Component component, string functionName, RpcTarget target, params object[] para)
    {

        if (PhotonNetwork.IsConnected)
        {
            component.GetComponent<PhotonView>().RPC(functionName, target, para);
        }
        else
        {
            CallMethod();
        }
        void CallMethod()
        {
            Type type = component.GetType();

            if (type != null)
            {
                MethodInfo methodInfo = type.GetMethod(functionName);

                if (methodInfo != null)
                {
                    object result = null;

                    if (para.Length == 0)
                    {
                        methodInfo.Invoke(component, null);
                    }
                    else
                    {       
                        methodInfo.Invoke(component, para);
                    }
                }
            }
        }
    }

    void spawnControlPoints()
    {
        for (int x = 0; x < numOfControlPoints; x++)
        {
            Vector2Int pos = new Vector2Int(UnityEngine.Random.Range(0, gridManager.GridBreadth / 2), UnityEngine.Random.Range(0, gridManager.GridWidth));
            GridManager.spawnOnGrid("ControlPoints/ControlPoint", pos);
        }
    }
 

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 0)
        {
            OnReadyUpRecieved((string)(photonEvent.CustomData));
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this); 
    }
}
