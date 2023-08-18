using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WWP;
using WWP.Game;

public class UnderLevels : MonoBehaviour
{
    [SerializeField]
    private GameObject [] gameObjects;
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private EndGamePanel endGamePanel;
    [SerializeField]
    private int countLevel;
    GameManager.EndGameInfo info;

    public void  nextLevel ( ) {
        if ( countLevel == 8 ) {
            gameManager.EndGame (0, info);
            //endGamePanel.Show (true);
        }
        gameObjects [countLevel].SetActive (false);
        countLevel++;
        gameObjects [countLevel].SetActive (true);
        gameManager.resetTimer ();
     
    }
    public void GameOverLevel ( ) {
        //endGamePanel.Show (false);
        //StopTimer.StopTimer ();
    }
    public void restartlevel ( ) {
        countLevel = 0;
        for (int i =0;i< gameObjects.Length;i++ ) {
            gameObjects [i].SetActive (false);
        }
      
        gameObjects [countLevel].SetActive (true);

    }
}
