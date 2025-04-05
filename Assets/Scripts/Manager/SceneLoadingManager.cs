using AarquieSolutions.Base.Singleton;
using UnityEngine.SceneManagement;

public enum Scene {Menu, Main };

public class SceneLoadingManager : Singleton<SceneLoadingManager>
{
    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);        
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);        
    }

    public void LoadScene(Scene scene)
    {
        LoadScene((int)scene);
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    
}
