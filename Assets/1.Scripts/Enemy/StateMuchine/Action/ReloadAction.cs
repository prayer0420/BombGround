using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FC;


[CreateAssetMenu(menuName = "PluggableAI/Actions/Reload")]

public class ReloadAction : Action
{
    public override void Act(StateController controller)
    {
        if(!controller.reloading && controller.bullets<= 0)
        {
            controller.enemyAnimation.anim.SetTrigger(AnimatorKey.Reload);
            controller.reloading = true;
            SoundManager.Instance.PlayOneShotEffect((int)SoundList.reloadWeapon, controller.enemyAnimation.gunMuzzle.position, 2f); //재장전 사운드
        }

    }
}
