using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TypingEffecrt : MonoBehaviour
{

    public Text main;
    private string m_text = "전쟁이 발발 한지 어느덧 2년... \n 현재 아군의 마지막 전진기지는 적의 총공세를 받고 있다. \n 아군의 증원이 오기전까지 적의 공격을 지연시키기위해 \n 적의 작전지속지원시설을 폭파하라는 임무를 받았다...";
    public Text next;
    private string next_text = "아무키나 눌러주세요..";
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(typing());
    }


    IEnumerator typing()
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < m_text.Length; i++)
        {
            main.text = m_text.Substring(0,i+1);

            yield return new WaitForSeconds(0.15f);

        }
        next.text = next_text;
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene("PlayGround");
        }
    }
}
