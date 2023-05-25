using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : SingletonMonobehaviour<EffectManager>
{

    private Transform effectRoot = null;



    // Start is called before the first frame update
    void Start()
    {
        if(effectRoot == null)
        {
            effectRoot = new GameObject("EffectRoot").transform;
            effectRoot.SetParent(transform);
        }            
    }

    public GameObject EffectOneShot(int index, Vector3 position)
    {
        //프리로딩을 받아서 생성함
        EffectClip clip = DataManager.EffectData().GetClip(index);
        GameObject effectInstance = clip.Instantiate(position);
        effectInstance.SetActive(true);
        return effectInstance;
    }

}
