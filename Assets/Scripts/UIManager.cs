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

    public void togglePlayerUI()
    {
        dragDropManager.updateChoices();
        if (GameManager.activePlayer.isAttacking)
            playerLabel.text = "Attacking Player";
        else
            playerLabel.text = "Defending Player";
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
