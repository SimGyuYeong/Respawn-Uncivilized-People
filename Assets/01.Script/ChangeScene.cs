using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class ChangeScene : MonoBehaviour
{
    public GameObject loading;
    public static ChangeScene Instance;

    private Image _fadeImage;
    private GameObject bb;

    private void Awake()
    {
        if(FightManager.sendChatID < 3)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            _fadeImage = transform.GetComponentInChildren<Image>();
        }
    }

    public IEnumerator SceneChange(string sceneName)
    {
        GameObject a = loading;
        _fadeImage.DOFade(1, .2f);
        _fadeImage.raycastTarget = true;
        yield return new WaitForSeconds(.2f);
        bb = Instantiate(a, transform);
        SceneManager.LoadScene(sceneName);
        yield return new WaitForSeconds(3f);
        _fadeImage.raycastTarget = false;
        Destroy(bb);
        _fadeImage.DOFade(0, .2f);
    }
}