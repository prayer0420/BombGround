using FC;
using NPOI.SS.Formula.Functions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bomb : MonoBehaviour
{
    public GameObject bombPrefab; // 폭탄 프리팹
    public Transform bombAttachmentPoint; // 폭탄을 부착할 위치 (부모 오브젝트)
    public Slider loadingBar; // 로딩 바 UI

    private bool isPressing; // 버튼이 길게 눌렸는지 여부
    private float loadingTime = 5.0f; // 로딩에 걸리는 시간 (초)
    private float currentLoadingTime; // 현재 로딩 시간

    private Rigidbody bombRigidbody; // 폭탄의 Rigidbody
    private Animator animator;

    public static bool bombing;
    public static bool bombend;
    public bool itemEquipped; //장착했는지 안했는지 확인

    private void Start()
    {
        //// 폭탄의 Rigidbody 컴포넌트를 가져옴
        //bombRigidbody = bombPrefab.GetComponent<Rigidbody>();
        //// Rigidbody가 중력에 의해 떨어지지 않도록 설정
        //bombRigidbody.useGravity = false;
        //// Rigidbody의 isKinematic 속성을 true로 설정하여 외부 힘에 의해 영향을 받지 않게 함
        //bombRigidbody.isKinematic = true;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (itemEquipped)
        {


            // 마우스 왼쪽 버튼을 길게 누르는지 확인
            if (Input.GetMouseButton(0))
            {
                if (!isPressing)
                {
                    isPressing = true;
                    currentLoadingTime = 0f;
                    loadingBar.gameObject.SetActive(true);
                }

                //애니메이션 활성화
                bombing = true;

                // 로딩 바 업데이트
                currentLoadingTime += Time.deltaTime;
                float progress = currentLoadingTime / loadingTime;
                loadingBar.value = Mathf.Clamp01(progress);

                // 로딩이 100%에 도달하면 폭탄 설치
                if (progress >= 1f)
                {
                    PlaceBomb();
                    loadingBar.value = 0f;
                    loadingBar.gameObject.SetActive(false);
                    isPressing = false;
                    bombing = false;

                    //gameObject.SetActive(false);
                }
            }
            else
            {
                // 버튼을 뗄 때 로딩 초기화
                loadingBar.value = 0f;
                loadingBar.gameObject.SetActive(false);
                isPressing = false;
                bombing = false;

            }
        }
    }

    private void PlaceBomb()
    {
        Vector3 bombPosition = bombAttachmentPoint.position + bombAttachmentPoint.forward * 0.2f;
        Quaternion bombRotation = bombAttachmentPoint.rotation;

        // 부착 지점에서 폭탄 생성
        GameObject bomb = Instantiate(bombPrefab, bombPosition, bombRotation);
        // 부착 지점의 자식으로 설정
        bomb.transform.parent = bombAttachmentPoint;
        bomb.SetActive(true);

        // Rigidbody 설정
        Rigidbody bombRigidbody = bomb.GetComponent<Rigidbody>();
        bombRigidbody.useGravity = false; // 중력 비활성화
        bombRigidbody.isKinematic = true; // 외부 힘에 의해 안움직임
        Destroy(gameObject);



    }
}
