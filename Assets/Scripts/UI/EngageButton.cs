using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngageButton : MonoBehaviour
{
    public void HandleEngagement()
    {
        PlayerManager playerManager = FindObjectOfType<PlayerManager>();
        if (playerManager != null)
        {
            PlayerAttack playerAttack = playerManager.CurrentPlayerAttack;
            if (playerAttack != null)
            {
                playerAttack.IsEngagedClicked = true;
                Debug.Log("Unit made a decision.");
            }
        }
    }
}
