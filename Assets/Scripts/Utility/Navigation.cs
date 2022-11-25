using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Navigation : MonoBehaviour
{
    #region Singleton

    public static Navigation instance { get; private set; }

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

    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] TextMeshProUGUI _loadingText;
    [SerializeField] Slider _loadingSlider;

    void Awake()
    {
        Singleton();
    }

    public void ChangeScene(string sceneName)
    {
        _canvasGroup.alpha = 1;
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            _loadingText.text = Mathf.RoundToInt(progress * 100) + "%";
            _loadingSlider.value = progress;

            yield return new WaitForEndOfFrame();
        }

        _canvasGroup.alpha = 0;
        //LightProbes.Tetrahedralize();
    }

    public void Return() => SceneManager.LoadSceneAsync(0);

    public void Quit() => Application.Quit();
}
