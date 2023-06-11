using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// state -> actions update -> transition(decision) check..
/// state에 필요한 기능들.. 애니메이션 콜백들(파라미터)..
/// 시야 체크, 찾아놓은 엄폐물 장소중 가장 가까운 위치를 찾는 기능..
/// </summary>
public class StateController : MonoBehaviour
{
    public GeneralStats generalStats;
    public ClassStats statdata;
    public string classID; //PISTOL, RIFLE, AK
    public ClassStats.Param classStats
    {
        //get할떄마다 클래스상에서 지정한 아이디를 엑셀데이터에서 불러옴
        get
        {
            foreach(ClassStats.Sheet sheet in statdata.sheets)
            {
                foreach(ClassStats.Param param in sheet.list)
                {
                    if (param.ID.Equals(this.classID))
                    {
                        return param;
                    }
                }
            }
            return null;
        }
    }

    public State currentState;
    //현재스테이트에서 decision했는데 transition할게 없고
    //remainstate로 저장해놓음
    public State remainState;
    public Transform aimTarget;
    //미리 정찰지점들을 지정해놓기
    public List<Transform> patrolWaypoints;
    public int bullets;

    [Range(0,50)]
    //플레이어가 볼수 있는 시야반경
    public float viewRadius;

    [Range(0,360)]
    public float viewAngle;

    [Range(0, 25)]
    //ingui, 눈으로 시야화할수있게(반경들)
    public float perceptionRadius;

    
    [HideInInspector]public float nearRadius;
    [HideInInspector]public NavMeshAgent nav;
    [HideInInspector]public int wayPointIndex; //way포인트
    [HideInInspector]public int maximumBurst = 7; //유효한 총알 개수
    [HideInInspector]public float blindEngageTime = 5f; //플레이어가 시야에 사라졌을때 플레이어를 찾는 시간(초과하면 다시 정찰로 돌아감)
    [HideInInspector]public bool targetInSight; //타겟이 내 시야안에 있는지
    [HideInInspector]public bool focusSight; //시야를 포커싱 할건지
    [HideInInspector]public bool reloading; //재장전 중이냐
    [HideInInspector]public bool hadClearShot; //방금전까지만해도 클리어샷 하고있었냐(장애물이 없었냐)
    [HideInInspector]public bool haveClearShot; //지금 클리어샷 하냐
    [HideInInspector]public int coverHash = -1; //숨을수있는 오브젝트 쭉 가져오고, 각 적마다 중복된 장애물에 숨지않도록
    [HideInInspector]public EnemyVariables variables;
    [HideInInspector] public Vector3 personalTarget = Vector3.zero; //각 적마다 타겟이 다를것

    private int magBullets; //잔탄량
    private bool aiActive; //적 죽었냐 살았냐
    private static Dictionary<int, Vector3> coverSpot; //static
    private bool strafing; //npc가 straf중이냐(카메라는 계속 특정지점을 중심으로 도는데 특정지점을 계속 향하고(쏘고)있느냐)
    private bool aiming; //조준중이냐
    private bool checkedOnLoop, blockedSight; //시야가 막혔냐, 특정루프를 막거나 체크하거나..

    [HideInInspector] public EnemyAnimations enemyAnimation;
    [HideInInspector] public CoverLookUp coverLookUp;

    public Vector3 CoverSpot
    {
        get { return coverSpot[this.GetHashCode()]; } //instance ID 리턴
        set { coverSpot[this.GetHashCode()] = value; } //covespot마다 한개의 각기 다른 id만 할당해서
    }
    

    public void TransitionToState(State nextState, Decision decision)
    {
        //nextstate가 remainstate라고 하면 현재 state유지,
        //nextstate가 remainstate가 아니라고 하면 현재 state를 nextstate와 바꿈
        if(nextState != remainState)
        {
            currentState = nextState;
        }
    }

    public bool Strafing
    {
        get => strafing;
        set
        {
            enemyAnimation.anim.SetBool("Strafe", value);
            strafing = value;
        }
    }
    
    public bool Aimimg
    {
        get => aiming;
        set
        {
            if(aiming != value)
            {
                enemyAnimation.anim.SetBool("Aim", value);
                aiming = value;
            }
        }
    }

    public IEnumerator UnstuckAim(float delay)
    {
        yield return new WaitForSeconds(delay * 0.5f);
        Aimimg = false;

        yield return new WaitForSeconds(delay * 0.5f);
        Aimimg = true;
    }


    private void Awake()
    {
        if(coverSpot == null)
        {
            coverSpot = new Dictionary<int, Vector3>();
        }
        coverSpot[this.GetHashCode()] = Vector3.positiveInfinity;
        nav = GetComponent<NavMeshAgent>();
        aiActive = true;
        enemyAnimation = gameObject.AddComponent<EnemyAnimations>();
        magBullets = bullets;
        variables.shotsInRounds = maximumBurst; //이번에 최대 쓸수있는 탄알 량 = 잔탄량
        nearRadius = perceptionRadius * 0.5f; //반경은 시야에 반.(nearRadius보다 작으면 무조건 공격)
        GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
        coverLookUp = gameController.GetComponent<CoverLookUp>();
        if(coverLookUp == null)
        {
            coverLookUp = gameController.AddComponent<CoverLookUp>();
            coverLookUp.SetUp(generalStats.coverMask);
        }
        Debug.Assert(aimTarget.root.GetComponent<HealthBase>(), "반드시 타겟에는 생명력관련 컴퍼넌트를" + "붙여주어야 합니다.");
    }

    public void Start()
    {
        currentState.OnEnableActions(this);
    }

    private void Update()
    {
        checkedOnLoop = false;

        if(!aiActive)
        {
            return;
        }

        currentState.DoActions(this);
        currentState.CheckTransitions(this);
    }

    private void OnDrawGizmos()
    {
        if(currentState !=null)
        {
            Gizmos.color = currentState.sceneGizmoColor;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 2.5f, 2f);
        }
    }

    //재장전 끝났습니다~ 
    public void EndReloadWeapon()
    {
        reloading = false;
        bullets = magBullets;
    }

    public void AlertCallBack(Vector3 target)
    {
        //죽지않았다면
        if (!aimTarget.root.GetComponent<HealthBase>().IsDead)
        {
            //경고들었다면 메세지보낸 타겟을 타겟으로지정
            this.variables.hearAlert = true;
            this.personalTarget = target;
        }
    }

    public bool IsNearOhterSpot(Vector3 spot, float margin = 1f)
    {
        foreach(KeyValuePair<int, Vector3> usedSpot in coverSpot)
        {
            //도착했다면
            if(usedSpot.Key != gameObject.GetHashCode() && Vector3.Distance(spot, usedSpot.Value) <= margin)
            {
                return true;
            }
        }
        return false;
    }

    //막혔는지
    public bool BlockedSight()
    {
        if(!checkedOnLoop)
        {
            checkedOnLoop = true;
            Vector3 target = default;
            try
            {
                target = aimTarget.position;
            }
            catch(UnassignedReferenceException)
            {
                Debug.LogError("조준 타겟을 지정해주세요 : " + transform.name);
            }

            Vector3 castOrigin = transform.position + Vector3.up * generalStats.aboveCoverHeight; //장애물 높이
            Vector3 dirToTarget = target - castOrigin;

            //장애물이나 엄폐물이 걸린다면 그건 시야를 막고있다는 뜻
            blockedSight = Physics.Raycast(castOrigin, dirToTarget, out RaycastHit hit, dirToTarget.magnitude, generalStats.coverMask | generalStats.obstacleMask);

        }
        return blockedSight;
    }

    private void OnDestroy()
    {
        //죽은 npc가 점유하고있던 스팟을 다시 초기화
        coverSpot.Remove(this.GetHashCode());
    }


}
