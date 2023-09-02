using UnityEngine;
using UnityEngine.UI;

public class ScoreText : MonoBehaviour
{
    [SerializeField] private Text _scoreText;

    private void Start()
    {
        Score.OnScoreChanged += OnScoreChanged;
    }

    private void OnScoreChanged()
    {
        _scoreText.text = Score.CurrentScore.ToString();
    }

    private void OnDestroy()
    {
        Score.OnScoreChanged -= OnScoreChanged;
    }
}