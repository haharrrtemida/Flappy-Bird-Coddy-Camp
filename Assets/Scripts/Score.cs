using UnityEngine;
using UnityEngine.Events;

public static class Score
{
    private const string HIGHSCORE_KEY = "HIGHSCORE";
    
    public static int CurrentScore { get; private set; }
    public static event UnityAction OnScoreChanged;

    public static void Init()
    {
        CurrentScore = 0;
        Bird.Instance.OnDied -= Bird_OnDied;
        Bird.Instance.OnDied += Bird_OnDied;
    }

    private static void Bird_OnDied()
    {
        TrySetNewHighscore();
    }

    public static int GetHighscore()
    {
        return PlayerPrefs.GetInt(HIGHSCORE_KEY);
    }

    public static void ScoreUp()
    {
        CurrentScore++;
        OnScoreChanged?.Invoke();
    }

    private static bool TrySetNewHighscore()
    {
        bool result = false;
        if (CurrentScore > GetHighscore())
        {
            PlayerPrefs.SetInt(HIGHSCORE_KEY, CurrentScore);
            PlayerPrefs.Save();
            SoundManager.PlaySound(SoundManager.Sound.Highscore);
            result = true;
        }
        return result;
    }
}