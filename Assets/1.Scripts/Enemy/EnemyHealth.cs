using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using FC;

public class EnemyHealth : HealthBase
{
    public float health = 100f;
    public GameObject healthHUD; //생명력 바?
    public GameObject bloodSample; //피튀기는
    public bool headShot;

    private float totalHealth;
    private Transform weapon;
    private Transform hud;
    private RectTransform healthBar;
    private float originalBarSCale;
    private HealthHUD healthUI;

    private Animator anim;
    private StateController controller;
    private GameObject gameController;

    private void Awake()
    {
        hud = Instantiate(healthHUD, transform).transform;
        if (!hud.gameObject.activeSelf)
        {
            hud.gameObject.SetActive(true);
        }
        totalHealth = health;
        healthBar = hud.transform.Find("Bar").GetComponent<RectTransform>();
        healthUI = hud.GetComponent<HealthHUD>();
        originalBarSCale = healthBar.sizeDelta.x;
        anim = GetComponent<Animator>();
        controller = GetComponent<StateController>();
        gameController = GameObject.FindGameObjectWithTag("GameController");

        foreach(Transform child in anim.GetBoneTransform(HumanBodyBones.RightHand))
        {
            weapon = child.Find("muzzle");
            if (weapon != null)
            {
                break;
            }
        }
        weapon = weapon.parent;
    }

    private void UpdateHealthBar()
    {
        float scaleFactor = health / totalHealth;
        healthBar.sizeDelta = new Vector2(scaleFactor * originalBarSCale, healthBar.sizeDelta.y);
    }

    //hp가 0이라면
    private void RemoveAllForces()
    {
        foreach(Rigidbody body in GetComponentsInChildren<Rigidbody>())
        {
            body.isKinematic = false;
            body.velocity = Vector3.zero;
        }
    }

    public void Kill()
    {
        foreach(MonoBehaviour mb in GetComponents<MonoBehaviour>())
        {
            if(this != mb)
            {
                Destroy(mb);
            }
        }
        Destroy(GetComponent<NavMeshAgent>());
        RemoveAllForces();
        controller.focusSight = false;
        anim.SetBool(AnimatorKey.Aim, false);
        anim.SetBool(AnimatorKey.Crouch, false);
        anim.enabled = false;
        Destroy(weapon.gameObject); //무기없애기
        Destroy(hud.gameObject);
        IsDead = true;
    }

    public override void TakeDamage(Vector3 location, Vector3 direction, float damage, Collider bodyPart = null, GameObject origin = null)
    {
        //죽어있진 않지만 헤드샷맞았다면
        if(!IsDead && headShot && bodyPart.transform == anim.GetBoneTransform(HumanBodyBones.Head))
        {
            damage *= 10;
            gameController.SendMessage("HeadShotCallback", SendMessageOptions.DontRequireReceiver);
        }

        //피튀기는
        Instantiate(bloodSample, location, Quaternion.LookRotation(-direction), transform);
        health -= damage;
        if(!IsDead)
        {
            anim.SetTrigger("Hit");
            healthUI.SetVisible();
            UpdateHealthBar();
            controller.variables.feelAlert = true;
            Debug.Log("맞았다!");
            controller.personalTarget = controller.aimTarget.position;
        }
        if (health <= 0)
        {
            if (!IsDead)
            {
                Kill();
            }
            //레[ㄱ돌? 통나무처럼 쓰러짐
            Rigidbody rigid = bodyPart.GetComponent<Rigidbody>();
            rigid.mass = 40;
            rigid.AddForce(100f *direction.normalized, ForceMode.Impulse);
        }
    }

}
