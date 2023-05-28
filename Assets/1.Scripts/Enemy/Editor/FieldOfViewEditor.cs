using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StateController))]
public class FieldOfViewEditor : Editor
{
    //각도를 방향으로
    Vector3 DirFromAngle(Transform transform, float angleInDegrees, bool anglesGlobal)
    {
        if (!anglesGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0f, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    //부채꼴
    private void OnSceneGUI()
    {
        StateController fov = target as StateController;
        //로드되지않았거나..에러가 났다면
        if(fov == null || fov.gameObject == null)
        {
            return;
        }

        Handles.color = Color.white;

        
        //PerCeption Area(Circle)
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.perceptionRadius);
        //near
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.perceptionRadius * 0.5f);

        Vector3 viewAngleA = DirFromAngle(fov.transform, -fov.viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(fov.transform, fov.viewAngle / 2, false);

        //시야각 부채꼴 모양으로 그리기
        Handles.DrawWireArc(fov.transform.position, Vector3.up, viewAngleA, fov.viewAngle, fov.viewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.viewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.viewRadius);

        Handles.color = Color.yellow;

        //총구로부터 타겟까지 선
        if(fov.targetInSight && fov.personalTarget !=  Vector3.zero)
        {
            Handles.DrawLine(fov.enemyAnimation.gunMuzzle.position, fov.personalTarget);
        }
    }

}

