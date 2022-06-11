using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject player;
    public GridManager gridManager;
    public Tweener tweener;
    public float playerSpeed = 2f;
    public bool isMoveThreadRunning;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        /*if (!isMoveThreadRunning)
        {
            continuosMove();
        }*/
    }

    async void continuosMove()
    {
        isMoveThreadRunning = true;
        movePlayer(0, 1);
        await tweener.waitForComplete();
        await Task.Delay(500);
        isMoveThreadRunning = false;
    }

    public void initializePlayer(object sender, EventArgs e)
    {
        Vector3 spawnPos = new Vector3(0, 2, Mathf.Floor(State.GridContents.GetLength(0) / 2f) * gridManager.TileSize);
        player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
    }

    public void movePlayer(int XDirection, int YDirecton)
    {
        tweener.AddTween(player.transform, player.transform.position, new Vector3(player.transform.position.x + XDirection * gridManager.TileSize, player.transform.position.y, player.transform.position.z + YDirecton * gridManager.TileSize), playerSpeed);
    }
}
