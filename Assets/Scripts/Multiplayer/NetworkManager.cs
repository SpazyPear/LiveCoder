using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Threading.Tasks;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update

    public bool Host;
    public bool LevelLoaded;

    void Start()
    {
        DontDestroyOnLoad(this);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        print("connected");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        print("lobby joined");
    }

    public override void OnJoinedRoom()
    {
        print("room joined");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (Host && !LevelLoaded)
        {
            LevelLoaded = true;
            PhotonNetwork.LoadLevel("Multiplayer");
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Application.Quit();
    }

    public void JoinRoom(string room)
    {
        PhotonNetwork.JoinRoom(room);
    }

    public void CreateRoom(string room)
    {
        Host = true;
        PhotonNetwork.CreateRoom(room, new Photon.Realtime.RoomOptions { MaxPlayers = 2 }, null);
    }

}
