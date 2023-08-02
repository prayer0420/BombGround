using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AddObstacle : MonoBehaviour
{

    public LayerMask coverLayer; // "cover" 레이어를 가진 객체를 선택하기 위한 레이어 마스크

    private void Start()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>(); // 모든 객체를 가져옵니다.

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == coverLayer)
            {
                NavMeshObstacle obstacle = obj.GetComponent<NavMeshObstacle>();
                if (obstacle == null)
                {
                    obstacle = obj.AddComponent<NavMeshObstacle>(); // NavMeshObstacle 컴포넌트가 없는 경우 추가합니다.
                }
                obstacle.enabled = true; // NavMeshObstacle 컴포넌트를 활성화합니다.
            }
        }
    }
}
