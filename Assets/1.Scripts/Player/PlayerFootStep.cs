using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FC;
/// <summary>
/// 발자국소리를 출력
/// </summary>
public class PlayerFootStep : MonoBehaviour
{
    public SoundList[] stepSounds;
    private Animator myAnimator;
    private int index;
    private Transform leftFoot, rightFoot;
    private float dist;
    private int groundedBool, coverBool, aimBool, crouchFloat; //쪼그려 앉아있는지
    private bool grounded;



    public enum Foot
    {
        Right,
        Left,
    }

    private Foot Step = Foot.Left;
    private float oldDist, maxDist = 0;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        leftFoot= myAnimator.GetBoneTransform(HumanBodyBones.LeftFoot);
        rightFoot = myAnimator.GetBoneTransform(HumanBodyBones.RightFoot);
        groundedBool = Animator.StringToHash(AnimatorKey.Grounded);
        coverBool = Animator.StringToHash(AnimatorKey.Cover);
        aimBool = Animator.StringToHash(AnimatorKey.Aim);
        crouchFloat = Animator.StringToHash(AnimatorKey.Crouch);
    }

    private void PlayFootStep()
    {
        //이동하고있따는것
        if(oldDist < maxDist)
        {
            return;
        }
        oldDist = maxDist = 0;
        int oldIndex = index;

        while(oldIndex == index) 
        {
            index = Random.Range(0, stepSounds.Length -1);  
        }
        SoundManager.Instance.PlayOneShotEffect((int)stepSounds[index], transform.position, 0.2f);
    }

    private void Update()
    {
        if(!grounded && myAnimator.GetBool(groundedBool))
        {
            PlayFootStep();
        }
        grounded = myAnimator.GetBool(groundedBool);
        float factor = 0.15f;

        //움직이고 있다면
        if(grounded && myAnimator.velocity.magnitude > 1.6f)
        {
            oldDist = maxDist;
            switch (Step)
            {
                case Foot.Left:
                    //발의 높이
                    dist = leftFoot.position.y - transform.position.y;
                    maxDist = dist > maxDist ? dist : maxDist;
                    if(dist <= factor)
                    {
                        PlayFootStep();
                        Step = Foot.Right;
                    }
                    break;
                case Foot.Right:
                    dist = rightFoot.position.y - transform.position.y;
                    maxDist = dist > maxDist ? dist : maxDist;
                    if (dist <= factor)
                    {
                        PlayFootStep();
                        Step = Foot.Left;
                    }
                    break;
            }
        }
    }









}
