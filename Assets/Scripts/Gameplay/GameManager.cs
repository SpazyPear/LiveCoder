using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

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
    }

    void Start()
    {
        State.onLevelLoad += initGameManager;
        State.initializeLevel();
    }

    public void initGameManager(object sender, EventArgs e)
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
    IEnumerator replicatedRunCode()
    {
        codeExecutor.RunCode();
        yield return null;
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


    public static void CallRPC(MonoBehaviour component, string functionName, RpcTarget target, params object[] para)
    {

        if (PhotonNetwork.IsConnected)
        {
            component.GetComponent<PhotonView>().RPC(functionName, target, para);
        }
        else
        {
            component.StartCoroutine(functionName, para);
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
