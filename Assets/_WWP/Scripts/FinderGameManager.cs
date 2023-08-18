using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WWP.Game
{
    public class FinderGameManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] _levels;
        [SerializeField] private string[] _characterNames;
        private int _currentLevel;
        private GameManager _gameManager;
        [SerializeField] private RectTransform _placeCharacterArea;
        [SerializeField] private TextMeshProUGUI _taskText;

        private void Start()
        {
            _gameManager = FindObjectOfType<GameManager>();
            _currentLevel = Random.Range(0, _levels.Length);
            for (int i = 0; i < _levels.Length; i++)
            {
                Button bg = _levels[i].transform.GetChild(0).GetComponent<Button>();
                bg.onClick.AddListener(OnBgClick);

                Button character = _levels[i].transform.GetChild(1).GetComponent<Button>();
                character.onClick.AddListener(OnCharacterClick);
            }
            Init();
        }

        public void Init()
        {
            foreach (GameObject level in _levels)
            {
                level.SetActive(false);
            }
            _levels[_currentLevel].SetActive(true);
            _taskText.text = $"Find the {_characterNames[_currentLevel]}";
            Vector3 position = new Vector3(
                Random.Range(_placeCharacterArea.rect.xMin, _placeCharacterArea.rect.xMax),
                Random.Range(_placeCharacterArea.rect.yMin, _placeCharacterArea.rect.yMax))
                + _placeCharacterArea.position;
            _levels[_currentLevel].transform.GetChild(1).position = position;
        }

        private void OnBgClick()
        {
            _gameManager.EndGame(0, new GameManager.EndGameInfo
            {
                win = false
            });
        }

        private void OnCharacterClick()
        {
            _currentLevel++;
            if (_currentLevel >= _levels.Length ) _currentLevel = 0;
            _gameManager.EndGame(0, new GameManager.EndGameInfo
            {
                win = true
            });
        }
    }
}
