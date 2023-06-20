using FC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

/// <summary>
/// 플레이어의 생명력을 담당
/// 피격시 피격이펙트를 표시하거나 UI업데이트를 한다.
/// 죽었을 경우 모든 동작 스크립트 동작을 멈춘다.
/// </summary>
public class PlayerHealth : HealthBase
{
    public float health = 100f;
    public float recoveryHp = 0;
    public float criticalHealth = 30f;
    public Transform healthHUD;
    public SoundList deathSound;
    public SoundList hitSound;
    public GameObject hurtPrefab;
    public float decayFactor = 0.8f;

    private float totalHealth;
    private RectTransform healthBar, placeHolderBar;
    private Text healthLabel;
    private float originalBarScale;
    private bool critical;

    private BlinkHUD criticalHUD;
    private HurtHUD hurtHUD;

    public GameObject LoseUI;
    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        totalHealth = health;

        healthBar = healthHUD.Find("HealthBar/Bar").GetComponent<RectTransform>();
        placeHolderBar = healthHUD.Find("HealthBar/Placeholder").GetComponent <RectTransform>();
        healthLabel = healthHUD.Find("HealthBar/Label").GetComponent<Text>();
        originalBarScale = healthBar.sizeDelta.x;
        healthLabel.text = "" + (int)health;

        criticalHUD = healthHUD.Find("Bloodframe").GetComponent<BlinkHUD>();
        hurtHUD = this.gameObject.AddComponent<HurtHUD>();
        hurtHUD.Setup(healthHUD, hurtPrefab, decayFactor, transform);
    }
    

    private void Update()
    {
        Debug.Log(health);

        if (placeHolderBar.sizeDelta.x > healthBar.sizeDelta.x) 
        {
            placeHolderBar.sizeDelta = Vector2.Lerp(placeHolderBar.sizeDelta, healthBar.sizeDelta, 2f * Time.deltaTime);
        }
        else if (placeHolderBar.sizeDelta.x < healthBar.sizeDelta.x)
        {
            placeHolderBar.sizeDelta = Vector2.Lerp(healthBar.sizeDelta, placeHolderBar.sizeDelta, 2f * Time.deltaTime);
        }
    }

    public bool IsFullLife()
    {
        return Mathf.Abs(health - totalHealth) < float.Epsilon;
    }

    //줄어든 hpbar 계속 업데이트
    private void UpdateHealthBar()
    {
        healthLabel.text = "" + (int)health;
        float scaleFactor = health/totalHealth;
        healthBar.sizeDelta = new Vector2(scaleFactor * originalBarScale, healthBar.sizeDelta.y);
    }

    private void Kill()
    {
        Debug.Log("플레이어 죽음");
        IsDead = true;
        gameObject.layer = TagAndLayer.GetLayerByName(TagAndLayer.LayerName.Default);
        gameObject.tag = TagAndLayer.TagName.Untagged;
        healthHUD.gameObject.SetActive(false);
        myAnimator.SetBool(AnimatorKey.Aim, false);
        myAnimator.SetBool(AnimatorKey.Cover, false);
        myAnimator.SetFloat(AnimatorKey.Speed, 0);

        foreach(GeneriBehaviour behaviour in GetComponentsInChildren<GeneriBehaviour>())
        {
            behaviour.enabled = false;
        }

        SoundManager.Instance.PlayOneShotEffect((int)deathSound, transform.position, 5f);
        //healthHUD.parent.Find("WeaponHUD").gameObject.SetActive(false);
    }

    public override void TakeDamage(Vector3 location, Vector3 direction, float damage, Collider bodyPart = null, GameObject origin = null)
    {
        health -= damage;
        UpdateHealthBar();
        if (hurtPrefab && healthHUD)
        {
            hurtHUD.DrawHurtUI(origin.transform, origin.GetHashCode());
        }

        if (health < 0)
        {
            Kill();
            LoseUI.SetActive(true);
            MoveBehaviour.CanMove = false;
        }
        else if (health<criticalHealth && !critical)
        {
            critical = true;
            criticalHUD.StartBlink();
        }

        SoundManager.Instance.PlayOneShotEffect((int)hitSound, location, 1f);
        Debug.Log(health + ("맞고 남은 데미지"));
    }


    public override void RecoveryHp(float hp)
    {
        health += hp;
        UpdateHealthBar();
    }

    //public override void RecoveryHp(int plushp)
    //{
    //    health += (float)plushp;
    //    Debug.Log(health);

    //}
}




