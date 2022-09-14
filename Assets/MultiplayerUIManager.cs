using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MultiplayerUIManager : MonoBehaviour
{
    public TMP_InputField JoinInput;
    public TMP_InputField CreateInput;
    public Button CreateButton;
    public Button JoinButton;
    public NetworkManager NetworkManager;

    public void Join()
    {
        NetworkManager.JoinRoom(JoinInput.text);
    }

    public void Create()
    {
        NetworkManager.CreateRoom(CreateInput.text);
    }
}
