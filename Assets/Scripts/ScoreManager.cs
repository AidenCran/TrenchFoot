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
    
    const int TimeScoreIncrement = 20;
    const float Delay = 5f;

    WaitForSeconds _delay;

    [SerializeField] TMP_Text scoreText;

    PauseMenu _pauseMenu;

    AudioSource _source;
    AudioClip _addScore;
    AudioClip _reduceScore;
    
#pragma warning disable CS0414
    bool _isReadyToAddScore;
#pragma warning restore CS0414

    void Awake() => Singleton();
    
    void Start()
    {
        _pauseMenu = PauseMenu.instance;

        if (!TryGetComponent(out _source)) _source = gameObject.AddComponent<AudioSource>();

        _addScore = Resources.Load<AudioClip>("SFX/SFX Points UP");
        _reduceScore = Resources.Load<AudioClip>("SFX/SFX Points DOWN");
        
        _pauseMenu.OnPauseEvent.AddListener((isPaused) =>
        {
            // If Unpaused
            if (!isPaused) StartAddingScorePerSecond();
        });
        
        _delay = new(Delay);
        DisplayScore();
        StartAddingScorePerSecond();
    }

    void AddScore(int score)
    {
        Score += score;
        DisplayScore();
        _source.PlayOneShot(_addScore);
    }

    public void ReduceScore(int score)
    {
        Score -= score;
        DisplayScore();
        _source.PlayOneShot(_reduceScore);
    }

    void StartAddingScorePerSecond()
    {
        StartCoroutine(AddScoreOverTime());
        IEnumerator AddScoreOverTime()
        {
            yield return _delay;
            if (_pauseMenu.IsPaused)
            {
                _isReadyToAddScore = true;
                yield break;
            }
            AddScore(TimeScoreIncrement);
            StartAddingScorePerSecond();
        }
    }

    void DisplayScore() => scoreText.text = $"SCORE: {Score}";
}
