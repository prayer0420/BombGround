using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBase : MonoBehaviour
{
    public class DamageInfo
    {
        public Vector3 loacation, direction;
        public float damage;
        public Collider bodyPart;
        public GameObject origin;
        

        public DamageInfo(Vector3 loacation, Vector3 direction, float damage, Collider bodyPart= null, GameObject origin = null)
        {
            this.loacation = loacation;
            this.direction = direction;
            this.damage = damage;
            this.bodyPart = bodyPart;
            this.origin = origin;
        }
    }

    //[HideInInspector]:밖에 노출되지 않음
    [HideInInspector] public bool IsDead;
    protected Animator myAnimator;

    //피격입었을때 이벤트 발생
    public virtual void TakeDamage(Vector3 location, Vector3 direction, float damage, Collider bodyPart = null, GameObject origin = null)
    {

    }

    public void HitCallBack(DamageInfo damageInfo)
    {
        this.TakeDamage(damageInfo.loacation, damageInfo.direction, damageInfo.damage, damageInfo.bodyPart, damageInfo.origin);
    }

    public virtual void RecoveryHp(float hp)
    {

    }
}
