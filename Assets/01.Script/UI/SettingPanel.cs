using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    [SerializeField] bool is16v9;
    [SerializeField] bool hasHz;
    [SerializeField] Toggle fullscreenToggle;
    [SerializeField] Dropdown resolutionDropdown;
    [SerializeField] Dropdown screenModeDropdown;

    List<Resolution> resolutions = new List<Resolution>();

    public int ResolutionIndex
    {
        get => PlayerPrefs.GetInt("ResolutionIndex", 0);
        set => PlayerPrefs.SetInt("ResolutionIndex", value);
    }

    public FullScreenMode ScreenMode
    {
        get => (FullScreenMode)PlayerPrefs.GetInt("ScreenMode", 1);
        set => PlayerPrefs.SetInt("ScreenMode", (int)value);
    }

    private void Start()
    {
        Application.runInBackground = true; //백그라운드에서도 게임이 실행된다.
        SetResolution();
    }

    void SetResolution()
    {
        resolutions.AddRange(Screen.resolutions); //Resolution 배열에 현재 모니터가 지원하는 해상도를 모두 대입
        resolutionDropdown.ClearOptions(); //해상도 Dropdown 목록을 초기화

        int optionNum = 0;
        foreach(Resolution item in resolutions)
        {
            Dropdown.OptionData option = new Dropdown.OptionData(); //드롭다운 옵션에 추가할 데이터를 클래스로 선언
            option.text = item.width + " X " + item.height + " " + item.refreshRate + "HZ"; //추가할 옵션 데이터의 텍스를 해상도로 변경
            resolutionDropdown.options.Add(option); //드롭다운 목록에 해상도 추가

            if(item.width == Screen.width && item.height == Screen.height)
            {
                resolutionDropdown.value = optionNum;
            }
            optionNum++;
        }

        Dropdown.OptionData a = new Dropdown.OptionData();
        resolutionDropdown.options.Add(a);

        int mode = (int)ScreenMode;
        if (mode == 3) mode = 2;
        screenModeDropdown.value = mode;
        screenModeDropdown.RefreshShownValue();

        resolutionDropdown.RefreshShownValue(); //드롭다운 목록 새로고침
    }

    public void ResolutionChange(int resolutionIndex)
    {
        ResolutionIndex = resolutionIndex;
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, ScreenMode);
    }

    public void ScreenModeChange(int index)
    {
        if (index == 2) index = 3;
        ScreenMode = (FullScreenMode)index;
        ResolutionChange(ResolutionIndex);
    }
}
