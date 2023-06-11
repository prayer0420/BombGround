using System;

// feel shot decision.. 

// cover decision..

//repeater decision

//patrol decision

//attack
[Serializable]

public class EnemyVariables
{
    
    public bool feelAlert; //경고를 감지했냐(사운드,경고메세지)
    public bool hearAlert; //소리를 감지했나
    public bool advanceCoverDecision; //더 좋은 엄폐물을 판단

    public int waitRounds; //교전 중에 얼마나 기다릴까(플레이어가 쏜 3발정도는 기다리고 4발째 일어나서 대응사격)
    public bool repeatShot; //반복적으로 공격할거냐
    public float waitInCoverTime; //엄폐물에서 얼마나 기다릴꺼냐
    public float coverTime; //이번교전에서 얼마나 숨어있냐
    public float patrolTimer; //정찰중 한지점에서 대기하는 시간
    public float shotTimer; //총 쏘는 딜레이
    public float startShootTimer;
    public float currentShots; //현재 발사한 총알
    public float shotsInRounds; //현재 교전중에서 얼마나 쐈는지
    public float blindEngageTimer; //플레이어 교전중에 사라졌을때 그 플레이어에 대해서 얼마나 인지하고있느냐(플레이어 찾아야함)
    

}
