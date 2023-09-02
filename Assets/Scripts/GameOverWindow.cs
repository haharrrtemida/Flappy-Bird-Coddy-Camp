using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverWindow : MonoBehaviour
{
    private const string HIGHSCORE_TEXT = "HIGHSCORE: ";
    private const string NEW_HIGHSCORE_TEXT = "NEW HIGHSCORE";

    [SerializeField] private Text _scoreText;
    [SerializeField] private Text _highscoreText;

    private void Start()
    {
        Bird.Instance.OnDied += Bird_OnDied;
        Hide();
    }


    private void Bird_OnDied()
    {
        _scoreText.text = Score.CurrentScore.ToString();
        CheckHighscore();
        Show();
    }

    private void CheckHighscore()
    {
        _highscoreText.text = HIGHSCORE_TEXT + Score.GetHighscore();
        if (Score.CurrentScore > Score.GetHighscore())
        {
            _highscoreText.text = NEW_HIGHSCORE_TEXT;
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnOpenMenuButtonPress()
    {
        StartCoroutine(LoadScene(0));
    }    

    public void OnRetryButtonPress()
    {
        StartCoroutine(LoadScene(1));
    }

    private IEnumerator LoadScene(int index)
    {
        SoundManager.PlaySound(SoundManager.Sound.ButtonClick);
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(index);
    }

    private void OnDestroy()
    {
        Bird.Instance.OnDied -= Bird_OnDied;
    }
}