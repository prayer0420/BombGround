using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    //설정 창
    public GameObject settingPanel;
    //나가기 버튼
    public Button ExitButton;
    //돌아가기 버튼
    public Button BackButton;

    //설정창이 켜져있는지 아닌지
    public static bool SettingActivated = false;

    public MoveBehaviour moveBehaviour;
    // Start is called before the first frame update
    void Start()
    {
        moveBehaviour = FindObjectOfType<MoveBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SettingActivated)
            {
                ResumeButtonClick(); 
            }
            else
            {
                MoveBehaviour.CanMove = false;
                settingPanel.SetActive(true);
                SettingActivated = true;
            }
        }
    }

    public void QuitButtonClick()
    {
        SceneManager.LoadScene(0);
        settingPanel.SetActive(false);
    }

    public void ResumeButtonClick()
    {
        settingPanel.SetActive(false);
        MoveBehaviour.CanMove = true;
        SettingActivated = false;
    }

    public void NewGameButtonClick()
    {
        SceneManager.LoadScene("PlayGround");
        settingPanel.SetActive(false);
        MoveBehaviour.CanMove = true;
    }



}
