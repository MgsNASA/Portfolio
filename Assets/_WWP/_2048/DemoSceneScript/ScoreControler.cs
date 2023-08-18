using TMPro;
using UnityEngine;
using WWP;

public class ScoreControler : MonoBehaviour
{
    private int _targetPoints;
    public int Points { get; private set; }
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI targetPointsText;
    private GameManager _gameManager;
    public int levelCount;
    public int Level
    {
        get { return PlayerPrefs.GetInt("Level", 1); }
        set { PlayerPrefs.SetInt("Level", value); }
    }
   
   

    public void Init(GameManager gameManager)
    {
        _gameManager = gameManager;
        _targetPoints = 2048 * levelCount;
        targetPointsText.text = _targetPoints.ToString();
        SetPoints(0);
    }

    public void AddPoints(int points)
    {
        SetPoints(Points + points);
    }

    public void SetPoints(int points)
    {
        Points = points;
        pointsText.text = Points.ToString();
        CheckPoint();
    }

    private void CheckPoint()
    {
        if (Points >= _targetPoints)
        {
            _gameManager.EndGame(0, new GameManager.EndGameInfo
            {
                win = true,
                value = Points,
                level = Level
            });
            Level += 1;
        }
    }
}
