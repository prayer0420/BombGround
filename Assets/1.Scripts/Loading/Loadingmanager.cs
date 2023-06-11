using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loadingmanager : MonoBehaviour
{
    public Image progressBar;
    void Start()

    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return null;
        //다음 씬 넘어가기(비동기적 작용)
        AsyncOperation op = SceneManager.LoadSceneAsync(2);
        //아직은 활성화 하지 않겠음
        op.allowSceneActivation = false;
        float timer = 0.0f;
        //OP가 다 불러와지지 않는 동안에
        while (!op.isDone)
        {
            yield return null;
            //프레임을 돌림
            timer += Time.deltaTime;

            if (op.progress < 0.9f)

            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                if (progressBar.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                if (progressBar.fillAmount == 1.0f)
                {
                    //다음씬을 활성화시켜라.
                    Debug.Log("로딩씬");
                    op.allowSceneActivation = true;
                    //코루틴 종료
                    yield break;
                }
            }
        }
    }

}
