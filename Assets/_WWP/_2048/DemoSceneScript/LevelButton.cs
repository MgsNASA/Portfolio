using UnityEngine;
using UnityEngine.UI;
using WWP;

public class LevelButton : MonoBehaviour {


    private Button _button;
    [SerializeField]
    private int buttoncount;
    [SerializeField]
    private ScoreControler _scoreController;
    [SerializeField]
    private GameManager _gameManager;

    // Start is called before the first frame update
    void Awake ( ) {
        _button = GetComponent<Button> ();
        ChangeColorButton ();



    }
    private void LoadLevel (  ) 
    {
        _scoreController.levelCount = buttoncount;
        _gameManager.RestartGame ();
        //_gameManager.StartGame ();
   
    }
    public void ChangeColorButton ( ) {
        if ( buttoncount <= _scoreController.Level ) {
            _button.interactable = true;
            _button.onClick.AddListener (LoadLevel);
        } else {
            _button.interactable = false;
            ColorBlock colors = _button.colors;
            colors.normalColor = new Color (0.2f, 0.2f, 0.2f);
            _button.colors = colors;
        }
    }
}
