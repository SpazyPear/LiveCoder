using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Threading.Tasks;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update

    public bool LobbyJoined;

    void Start()
    {
        //DontDestroyOnLoad(this);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        print("connected");
        PhotonNetwork.JoinLobby();
    }

    public override async void OnJoinedLobby()
    {
        print("lobby joined");

        /*if (Application.isEditor)
            PhotonNetwork.CreateRoom("Room", new Photon.Realtime.RoomOptions { MaxPlayers = 2 }, null);
        else
        {
            await Task.Delay(3000);
            PhotonNetwork.JoinRoom("Room");
        }*/
        LobbyJoined = true;
    }

    public override void OnJoinedRoom()
    {
        print("room joined");
        PhotonNetwork.LoadLevel("Multiplayer");
    }

    public void JoinRoom(string room)
    {
        PhotonNetwork.JoinRoom(room);
    }

    public void CreateRoom(string room)
    {
        PhotonNetwork.CreateRoom(room, new Photon.Realtime.RoomOptions { MaxPlayers = 2 }, null);
    }

}
