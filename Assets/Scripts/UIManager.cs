using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public DragDropManager dragDropManager;
    public GameObject readyButton;
    public GameObject nextRoundButton;
    public TMP_Text playerLabel;
    public TMP_Text creditCounter;
    public TMP_Text gridLabel;
    

    void Start()
    {
        GameManager.OnPhaseChange.AddListener(phaseUIChange);        
    }

    void phaseUIChange(int phase)
    {
        switch (phase)
        {
            case 0:
                prepPhase();
                break;
            case 1:
                battlePhase();
                break;
            case 2:
                resultsPhase();
                break;
        }
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.GetComponentInChildren<GridTile>() != null)
            {
                gridLabel.gameObject.SetActive(true);
                gridLabel.rectTransform.position = Camera.main.WorldToScreenPoint(hit.transform.position + Vector3.up * 1.5f);
                gridLabel.text = hit.transform.GetComponentInChildren<GridTile>().gridTile.gridPosition.x.ToString() + "," + hit.transform.GetComponentInChildren<GridTile>().gridTile.gridPosition.y;
            }
            else
            {
                gridLabel.gameObject.SetActive(false);
            }
        }
        else
        {
            gridLabel.gameObject.SetActive(false);
        }
    }

    public void togglePlayerUI()
    {
        dragDropManager.updateChoices();
        if (GameManager.activePlayer.isLeftSide)
            playerLabel.text = "Player 1";
        else
            playerLabel.text = "Player 2";
        creditCounter.text = GameManager.activePlayer.creditsLeft.value.ToString();
    }

    public void updateCreditUI(int value)
    {
        creditCounter.text = value.ToString();
    }

    void prepPhase()
    {
        nextRoundButton.SetActive(false);
        dragDropManager.gameObject.SetActive(true);
        readyButton.SetActive(true);
        playerLabel.enabled = true;
        creditCounter.enabled = true;
    }

    void battlePhase()
    {
        readyButton.SetActive(false);
        dragDropManager.gameObject.SetActive(false);
        playerLabel.enabled = false;
        creditCounter.enabled = false;
    }

    void resultsPhase()
    {
        nextRoundButton.SetActive(true);
    }
}
