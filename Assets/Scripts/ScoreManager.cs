using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    #region Singleton

    public static ScoreManager instance { get; private set; }

    private void Singleton()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    #endregion

    public int Score { get; private set; }
    
    void Awake() => Singleton();

    [SerializeField] int _timeScoreIncrement = 5;
    [SerializeField] float delay = 2.5f;

    WaitForSeconds _delay;

    [SerializeField] TMP_Text ScoreText;
    
    void Start()
    {
        _delay = new(delay);
        DisplayScore();
        StartAddingScorePerSecond();
    }

    public void AddScore(int score)
    {
        Score += score;
        DisplayScore();
    }

    public void ReduceScore(int score)
    {
        Score -= score;
        DisplayScore();
    }

    void StartAddingScorePerSecond()
    {
        StartCoroutine(AddScoreOverTime());
        IEnumerator AddScoreOverTime()
        {
            yield return _delay;
            AddScore(_timeScoreIncrement);
            StartAddingScorePerSecond();
        }
    }

    void DisplayScore() => ScoreText.text = $"SCORE: {Score}";
}
