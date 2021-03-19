using UnityEngine;
using UnityEngine.SceneManagement;
using MLAPI;
using MLAPI.SceneManagement;

public class MultiSceneTest : MonoBehaviour 
{
    [SerializeField]
    private bool m_EnableSceneSwitchTimer;

    [SerializeField]
    private float m_SwitchSceneInterval = 5.0f;
    private float m_SwitchSceneTimer;

    // Start is called before the first frame update
    void Start()
    {
        m_SwitchSceneTimer = Time.realtimeSinceStartup + m_SwitchSceneInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_EnableSceneSwitchTimer && m_SwitchSceneTimer <  Time.realtimeSinceStartup)
        {
            SwitchToNextScene();
        }
        else
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SwitchToNextScene();
        }
    }

    private Scene m_ActiveScene;

    private void SwitchToNextScene()
    {
        m_ActiveScene = SceneManager.GetActiveScene();
        var sceneIndex = NetworkManager.Singleton.NetworkConfig.RegisteredScenes.FindIndex(FindCurrentScene);
        sceneIndex++;
        if(sceneIndex >= NetworkManager.Singleton.NetworkConfig.RegisteredScenes.Count)
        {
            sceneIndex = 0;
        }
        NetworkSceneManager.SwitchScene(NetworkManager.Singleton.NetworkConfig.RegisteredScenes[sceneIndex]);
    }

    private bool FindCurrentScene(string sceneName)
    {
        if(m_ActiveScene != null)
        {
            return m_ActiveScene.name == sceneName;
        }

        return false;
    }
}
