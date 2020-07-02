using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    SelectionIndicator selectionIndicator;
    TurnManager turnManager;

    public IEnumerator InitEnemyTurn()
    {
        selectionIndicator = FindObjectOfType<SelectionIndicator>();
        turnManager = FindObjectOfType<TurnManager>();
        selectionIndicator.HideSelectionIndicator();
        Cursor.visible = false;
        Debug.Log("Start of Enemy Turn.");
        yield return new WaitForSeconds(3f);
        Debug.Log("End of Enemy Turn.");
        Cursor.visible = true;
        selectionIndicator.ShowSelectionIndicator();
        turnManager.currentTurn = Turn.playerTurn;
    }
}
