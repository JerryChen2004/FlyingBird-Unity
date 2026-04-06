using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private Image loadProgress;
    private float progress;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public async void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        audioManager.instance.musicSource.Pause();
        progress = 0;
        loadProgress.fillAmount = 0;

        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        loadingCanvas.SetActive(true);
        do
        {
            await Task.Delay(100);
            progress = scene.progress;
        } while (scene.progress < 0.9f);

        await Task.Delay(1000);
        scene.allowSceneActivation = true;
        loadingCanvas.SetActive(false);
        audioManager.instance.musicSource.UnPause();
    }

    private void Update()
    {
        loadProgress.fillAmount = Mathf.MoveTowards(loadProgress.fillAmount, progress, 3 * Time.deltaTime);
    }
}
