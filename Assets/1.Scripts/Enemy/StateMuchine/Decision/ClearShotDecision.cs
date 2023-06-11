using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 더블체크를 하는데 근처에 장애물이나 엄폐물이 가깝게 있는지 체크 한번 하고
/// 타겟목표까지 장애물이나 엄폐물이 있는지 체크.. 만약 충돌검출된 충돌체가 타겟(플레이어)이라면 막힌게 없다는 뜻.
/// </summary>

[CreateAssetMenu(menuName = "PluggableAI/Decisions/Clear Shot")]

public class ClearShotDecision : Decision
{
    [Header("Extra Decision")]
    public FocusDecision targetNear;
    
    //지금 클리어샷 가능한지
    public bool HaveClearShot(StateController controller)
    {
        //목 정도 위치
        Vector3 shotOrigin = controller.transform.position + Vector3.up * (controller.generalStats.aboveCoverHeight + controller.nav.radius);

        //사격방향
        Vector3 shotDirection = controller.personalTarget - shotOrigin;

        //1. 내 위치에 구형태의 raycast를 만들어서 바로 가까운곳에 장애물이있는지 체크
        bool blockedShot = Physics.SphereCast(shotOrigin, controller.nav.radius, shotDirection, out RaycastHit hit, controller.nearRadius, controller.generalStats.coverMask | controller.generalStats.obstacleMask);

        if(!blockedShot)
        {
            //2. 총구위치에서 장애물이있는지 없는지 체크
            blockedShot = Physics.Raycast(shotOrigin, shotDirection, out hit, shotDirection.magnitude, controller.generalStats.coverMask | controller.generalStats.obstacleMask);
                //무언가 막히긴했는데
                if(blockedShot)
                {
                    //막히긴했는데 플레이어인 경우
                    blockedShot = !(hit.transform.root == controller.aimTarget.root);
                    
                    Debug.Log("클리어샷!");
                }
        }
        return !blockedShot;
    }
    

    public override bool Decide(StateController controller)
    {
        Debug.Log("Clear shot");
        return targetNear.Decide(controller) || HaveClearShot(controller);
    }
}
