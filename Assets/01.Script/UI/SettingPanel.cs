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
        Application.runInBackground = true; //��׶��忡���� ������ ����ȴ�.
        SetResolution();
    }

    void SetResolution()
    {
        resolutions.AddRange(Screen.resolutions); //Resolution �迭�� ���� ����Ͱ� �����ϴ� �ػ󵵸� ��� ����
        resolutionDropdown.ClearOptions(); //�ػ� Dropdown ����� �ʱ�ȭ

        int optionNum = 0;
        foreach(Resolution item in resolutions)
        {
            Dropdown.OptionData option = new Dropdown.OptionData(); //��Ӵٿ� �ɼǿ� �߰��� �����͸� Ŭ������ ����
            option.text = item.width + " X " + item.height + " " + item.refreshRate + "HZ"; //�߰��� �ɼ� �������� �ؽ��� �ػ󵵷� ����
            resolutionDropdown.options.Add(option); //��Ӵٿ� ��Ͽ� �ػ� �߰�

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

        resolutionDropdown.RefreshShownValue(); //��Ӵٿ� ��� ���ΰ�ħ
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
