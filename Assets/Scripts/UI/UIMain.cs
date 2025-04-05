using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMain : MonoBehaviour
{
    [SerializeField] private GameObject endSceneScreen;

    private void Awake()
    {
        GameManager.Instance.OnSceneOver += ShowSceneOverScreen;
    }

    private void ShowSceneOverScreen()
    {
        endSceneScreen.SetActive(true);
    }

    public void Pause()
    {
        GameManager.Instance.Pause();
    }

    public void Resume()
    {
        GameManager.Instance.Resume();
    }

    public void LoadMenuScene()
    {
        Time.timeScale = 1;
        SceneLoadingManager.Instance.LoadScene(Scene.Menu);
    }

    public void LoadMapScene(int _index)
    {
        Time.timeScale = 1;
        SceneLoadingManager.Instance.LoadScene(_index);
    }

    public void ReloadScene()
    {
        Time.timeScale = 1;
        SceneLoadingManager.Instance.ReloadCurrentScene();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnSceneOver -= ShowSceneOverScreen;
        }
    }
}
