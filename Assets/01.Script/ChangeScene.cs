using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public GameObject loading;
    public static ChangeScene Instance;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public IEnumerator SceneChange(string sceneName)
    {
        Instantiate(loading);
        SceneManager.LoadScene(sceneName);
        yield return new WaitForSeconds(3f);
        Destroy(loading);
    }
}
