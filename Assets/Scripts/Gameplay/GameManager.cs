using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using Unity.VisualScripting;
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
    public static Dictionary<int, Entity> unitInstances = new Dictionary<int, Entity>(); 
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


    public void SetIDs(int phase)
    {
        if (phase == 1)
        {
            int counter = 0;
            foreach (Entity e in GameObject.FindObjectsOfType<Entity>())
            {
                e.ID = counter++;
            }
        }
    }

    public void initGameManager(object sender, EventArgs e)
    {
        gridDimensions = new Vector3(gridManager.GridBreadth, gridManager.GridHeight, gridManager.GridWidth);
    }

    public void OnReadyUp()
    {
        if (PhotonNetwork.IsMasterClient) { OnReadyUpRecieved(PhotonNetwork.LocalPlayer.UserId); }
        else { PhotonNetwork.RaiseEvent(0, PhotonNetwork.LocalPlayer.UserId, new RaiseEventOptions(), new SendOptions()); }
    }

    public void OnReadyUpRecieved(string player)
    {
        playersReadied++;
        if (playersReadied >= 2)
        {
            playersReadied = 0;
            incrementPhase();
        }
        else
        {
            photonView.RPC("playerReady", RpcTarget.All, player);
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
                photonView.RPC("replicatedRunCode", RpcTarget.All);
                break;
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
