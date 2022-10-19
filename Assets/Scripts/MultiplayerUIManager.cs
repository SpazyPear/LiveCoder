using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class MultiplayerUIManager : MonoBehaviour
{
    public TMP_InputField JoinInput;
    public TMP_InputField CreateInput;
    public Button CreateButton;
    public Button JoinButton;
    public NetworkManager NetworkManager;
    public TMP_Text WaitingText;

    private void Start()
    {
        NetworkManager.OnConnectedToLobbyCallback += OnConnectedToLobby;
    }

    public void Join()
    {
        NetworkManager.JoinRoom(JoinInput.text);
    }

    public void Create()
    {
        NetworkManager.CreateRoom(CreateInput.text);
        WaitingText.enabled = true;
    }

    public void OnConnectedToLobby(object sender, EventArgs e)
    {
        JoinButton.interactable = true;
        CreateButton.interactable = true;
    }
}
