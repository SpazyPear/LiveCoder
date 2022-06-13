using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

public class PlayerHandler : MonoBehaviour
{
    PlayerManager selectedPlayer;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            print("Clicking");
            if (Physics.Raycast(ray, out hit))
            {
                print(hit.transform.name);
                if (hit.transform.root != null && hit.transform.root.GetComponent<PlayerManager>() != null)
                {
                    print($"Player {hit.transform.root.name} has been selected");
                    selectedPlayer = hit.transform.root.GetComponent<PlayerManager>();
                }
            }
        }
    }

    public void MovePlayer(int x, int y)
    {
        if (selectedPlayer != null)
            selectedPlayer.movePlayer(x, y);
    }
    public void DebugLog(string debug)
    {
        Debug.Log(debug);
    }

    public void OnScriptSetup(Script script)
    {

        script.Globals["MovePlayer"] = (System.Action<int, int>)MovePlayer;
        script.Globals["DebugLog"] = (System.Action<string>)DebugLog;
    }
}
