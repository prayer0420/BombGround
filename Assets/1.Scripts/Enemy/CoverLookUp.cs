using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 숨을만한곳을 찾아주는 컴포넌트
/// 플레이어보다 멀리있는건 제외
/// </summary>
public class CoverLookUp : MonoBehaviour
{
    private List<Vector3[]> allCoverSpots; //현재 레벨에서 존재하는 모든 coverspot들

    private GameObject[] covers;
    private List<int> coverHashCodes; //cover unity ID;

    private Dictionary<float, Vector3> filteredSpots; //npc로부터 특정위치에서 멀어지는건 필터링된 지점들


    //특정 layer에 대한 게임오브젝트를 다 가져옴
    private GameObject[] GetObjectsInLayerMask(int layerMask)
    {
        List<GameObject> ret = new List<GameObject>();

        //모든 씬에서 활성화된 모든 게임 오브젝트에 대해 반복
        foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            //현재 게임 오브젝트가 활성화된 상태이고, 레이어 마스크에 해당하는 레이어와 일치하는 경우
            if (go.activeInHierarchy && layerMask == (layerMask | (1<< go.layer)))
            {
                ret.Add(go);
            }
        }

        //배열로 변환하여 반환
        return ret.ToArray();

    }


    //현재 내가 서있는 지점에서 특정포인트 찍은다음에 그 위치가 나한테 유요한가?(navmesh한테 물어봄)하면서 최적의 위치를 찾음
    //주어진 범위 내에서 특정 포인트가 NavMesh 위에 있는지 확인하고, 유효한 위치를 찾아 리스트에 추가하는 역할
    private void ProcessPoint(List<Vector3> vector3s, Vector3 nativePoint, float range)
    {
        //navmesh위에 충돌정보
        NavMeshHit hit; 
        //navmesh한테 갈수있는곳인지 물어봄
        if(NavMesh.SamplePosition(nativePoint, out hit, range, NavMesh.AllAreas))
        {
            vector3s.Add(hit.position);
        }
    }

    //주어진 게임 오브젝트의 충돌체를 기반으로 장애물이 없는 위치를 찾아 반환하는 역할
    private Vector3[] GetSpots(GameObject go, LayerMask obstacleMask)
    {
        List<Vector3> bounds = new List<Vector3>(); //장애물이 없는 위치를 저장할 리스트 bounds를 선언합니다.

        foreach (Collider col in go.GetComponents<Collider>()) //주어진 게임 오브젝트의 모든 충돌체(Collider)를 순회합니다.
        {
            
            float baseHeight = (col.bounds.center - col.bounds.extents).y; //기본높이, 충돌체의 아랫면 높이
            float range = 2 * col.bounds.extents.y; // 충돌체의 전체 높이

            Vector3 deslocalscaleForward = go.transform.forward * go.transform.localScale.z * 0.5f;  //충돌체의 전방으로부터의 이동 벡터
            Vector3 deslocalRight = go.transform.right * go.transform.localScale.x * 0.5f; //돌체의 우측 방향으로부터의 이동 벡터

            if (go.GetComponent<MeshCollider>())
            {
                //최대 바운즈 크기
                float maxBounds = go.GetComponent<MeshCollider>().bounds.extents.z +
                    go.GetComponent<MeshCollider>().bounds.extents.x;

                Vector3 originForward = col.bounds.center + go.transform.forward * maxBounds; //충돌체의 전방 방향에서 maxBounds만큼 떨어진 위치를 나타냅니다.
                Vector3 originRihgt = col.bounds.center + go.transform.right * maxBounds; //충돌체의 우측 방향에서 maxBounds만큼 떨어진 위치

                //장애물이 있는지
                //originForward 위치에서 충돌체의 중심 위치(col.bounds.center)로 향하는 방향으로, maxBounds 범위 내에서 obstacleMask에 해당하는 장애물과의 충돌을 검사
                //meshcollider중심부터 앞뒤좌우 Ray를 쏘고 meshcollider의 형태,scale에 따라 벡터를 구함(충돌체의 총 크기)
                if (Physics.Raycast(originForward, col.bounds.center - originForward, out RaycastHit hit, maxBounds, obstacleMask))
                {
                    //장애물과의 충돌이 발생한 경우, 이동 벡터를 수정
                    deslocalscaleForward = hit.point- col.bounds.center;
                }
                //originRight 위치에서 충돌체의 중심 위치(col.bounds.center)로 향하는 방향으로, maxBounds 범위 내에서 obstacleMask에 해당하는 장애물과의 충돌을 검사
                if (Physics.Raycast(originRihgt, col.bounds.center - originRihgt, out hit, maxBounds, obstacleMask))
                {
                    //장애물과의 충돌이 발생한 경우, 이동 벡터를 수정
                    deslocalRight = hit.point - col.bounds.center; 
                }
            }
            //meshcollider가 없고 1,1,1짜리면
            else if(Vector3.Equals(go.transform.localScale, Vector3.one))
            {
                deslocalscaleForward = go.transform.forward * col.bounds.extents.z;
                deslocalRight = go.transform.right * col.bounds.extents.x;
            }

            float edgeFactor = 0.75f; //가운데 사각형

            //12개 샘플링(직사각형을 기준으로 윗줄 5개, 아랫줄 5개, 왼쪽1개, 오른쪽 1개)
            ProcessPoint(bounds, col.bounds.center + deslocalRight + deslocalscaleForward * edgeFactor, range); //우상단
            ProcessPoint(bounds, col.bounds.center + deslocalscaleForward + deslocalRight * edgeFactor, range); //우상단
            ProcessPoint(bounds, col.bounds.center + deslocalscaleForward, range); 

            ProcessPoint(bounds, col.bounds.center + deslocalscaleForward - deslocalRight * edgeFactor, range);
            ProcessPoint(bounds, col.bounds.center - deslocalRight + deslocalscaleForward * edgeFactor, range);
            ProcessPoint(bounds, col.bounds.center + deslocalRight, range);

            ProcessPoint(bounds, col.bounds.center + deslocalRight - deslocalscaleForward * edgeFactor, range);
            ProcessPoint(bounds, col.bounds.center - deslocalscaleForward + deslocalRight * edgeFactor, range);
            ProcessPoint(bounds, col.bounds.center - deslocalscaleForward, range);

            ProcessPoint(bounds, col.bounds.center - deslocalscaleForward - deslocalRight * edgeFactor, range);
            ProcessPoint(bounds, col.bounds.center - deslocalRight - deslocalscaleForward * edgeFactor, range);
            ProcessPoint(bounds, col.bounds.center - deslocalRight, range);

        }
        return bounds.ToArray();
    }

    //커버 마스크(LayerMask)에 해당하는 게임 오브젝트들을 가져와서 커버 스팟을 설정
    public void SetUp(LayerMask coverMask)
    {
        covers = GetObjectsInLayerMask(coverMask);
        coverHashCodes = new List<int>();
        allCoverSpots = new List<Vector3[]>();
        foreach(GameObject cover in covers)
        {
            allCoverSpots.Add(GetSpots(cover, coverMask));
            coverHashCodes.Add(cover.GetHashCode());
        }
    }

    //목표물이 경로에 있는지 확인, 대상이 각도안에 있고 지점보다 가까이 있느냐?
    private bool TargetInPath(Vector3 origin, Vector3 spot, Vector3 target, float angle)
    {
        Vector3 dirToTarget = (target - origin).normalized; //타겟을 향한 방향
        Vector3 dirToSpot = (spot - origin).normalized; //지점을 향한 방향
        
        
        if(Vector3.Angle(dirToSpot, dirToTarget) <= angle)
        {
            
            float targetDis = (target - origin).sqrMagnitude; //타겟까지의 거리
            float spotDis = (spot - origin).sqrMagnitude; //지점까지의 거리
            return (targetDis <= spotDis);
        }
        return false;
    }

    //가장 가까운 유효한 지점을 찾는다. 거리도 같이
    private ArrayList FilterSpots(StateController controller)
    {
        //초기값
        float minDist = Mathf.Infinity;
        filteredSpots = new Dictionary<float, Vector3>();
        int nextCoverHash = -1;

        //모든 엄폐물을 다 갖고와서
        for (int i = 0; i < allCoverSpots.Count; i++)
        {
            //엄폐물이 활성화 되어있지 않거나, 
            if (!covers[i].activeSelf || coverHashCodes[i] == controller.coverHash)
            {
                continue;
            }
            //갈수있는 위치를 다가져와서 최소위치 찾기
            foreach(Vector3 spot in allCoverSpots[i])
            {
                Vector3 vectorDist = controller.personalTarget - spot;
                float searchDist = (controller.transform.position - spot).sqrMagnitude;

                //내가보이는것보다 작거나    ,영역안에있냐
                if(vectorDist.sqrMagnitude <= controller.viewRadius * controller.viewRadius && 
                   Physics.Raycast(spot, vectorDist, out RaycastHit hit, vectorDist.sqrMagnitude,
                   controller.generalStats.coverMask))
                {
                    //플레이어가 npc와 스팟 사이에 있지 않은지 확인하고, 보이는 각도의 1/4각을 사용
                    //타겟보다 멀리있는건 거른다.
                    if(hit.collider == covers[i].GetComponent<Collider>() && !TargetInPath(controller.transform.position, spot, controller.personalTarget, controller.viewAngle / 4))
                    {
                        if (!filteredSpots.ContainsKey(searchDist))
                        {
                            filteredSpots.Add(searchDist, spot);
                        }
                        else
                            continue;
                        if(minDist > searchDist)
                        {
                            minDist = searchDist;
                            nextCoverHash = coverHashCodes[i];
                        }
                    }
                }
            }
        }
        ArrayList returnArray = new ArrayList();
        returnArray.Add(nextCoverHash);
        returnArray.Add(minDist);
        return returnArray;
    }

    public ArrayList GetBestCoverSpot(StateController controller)
    {
        ArrayList nextCoverData = FilterSpots(controller);
        int nextCoverHash = (int)nextCoverData[0];
        float minDist = (float)nextCoverData[1];

        ArrayList returnArray = new ArrayList();
        if(filteredSpots.Count == 0)
        {
            returnArray.Add(-1);
            returnArray.Add(Vector3.positiveInfinity); //큰값
        }
        else
        {
            returnArray.Add(nextCoverHash);
            returnArray.Add(filteredSpots[minDist]);
        }

        return returnArray;
    }

}
