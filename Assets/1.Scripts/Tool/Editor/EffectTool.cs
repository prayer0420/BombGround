﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using UnityObject = UnityEngine.Object;


/// <summary>
/// 이펙트 데이타 안에있는 속성들을 설정할 수 있는  Tool
/// </summary>
public class EffectTool : EditorWindow
{

    //UI 그리는데 필요한 변수들

    public int uiWidthLarge = 300;
    public int uiWidthMiddle = 220;
    private int selection = 0;
    //scroll
    private Vector2 SP1 = Vector2.zero;
    private Vector2 SP2 = Vector2.zero;

    //이펙트 클립
    private GameObject effectSource = null;
    //이펙트 데이터
    private static EffectData effectData;

    //editor window 
    [MenuItem("Tools/Effect Tool")]
    static void Init()
    {
        effectData = ScriptableObject.CreateInstance<EffectData>();
        effectData.LoadData();

        EffectTool window = GetWindow<EffectTool>(false, "Effect Tool");
        window.Show();
    }

    //window안에 그리는 것
    private void OnGUI()
    {
        if(effectData == null)
        {
            return;
        }
        EditorGUILayout.BeginVertical();
        {
            //상단, add, remove, copy
            UnityObject source = effectSource;
            EditorHelper.EditorToolTopLayer(effectData, ref selection, ref source, this.uiWidthMiddle);
            effectSource = (GameObject)source;

            EditorGUILayout.BeginHorizontal();
            {
                //중간, 데이터목록
                EditorHelper.EditorToolListLayer(ref SP1, effectData, ref selection, ref source, this.uiWidthLarge);
                effectSource = (GameObject)source;

                //설정 부분
                EditorGUILayout.BeginVertical();
                {
                    SP2 = EditorGUILayout.BeginScrollView(this.SP2);
                    {
                        if (effectData.GetDataCount() > 0)
                        {
                            EditorGUILayout.BeginVertical();
                            {
                                EditorGUILayout.Separator(); //구분자

                                EditorGUILayout.LabelField("ID", selection.ToString(), GUILayout.Width(uiWidthLarge));
                                effectData.names[selection] = EditorGUILayout.TextField("이름", effectData.names[selection], GUILayout.Width(uiWidthLarge * 1.5f));

                                effectData.effectClips[selection].effectType = (EffectType)EditorGUILayout.EnumPopup("이펙트 타입",
                                    effectData.effectClips[selection].effectType, GUILayout.Width(uiWidthLarge));
                                EditorGUILayout.Separator();

                                if (effectSource == null && effectData.effectClips[selection].effectName != string.Empty)
                                {
                                    effectData.effectClips[selection].PreLoad();
                                    effectSource = Resources.Load(effectData.effectClips[selection].effectPath + effectData.effectClips[selection].effectName) as GameObject; ;
                                }
                                effectSource = (GameObject)EditorGUILayout.ObjectField("이펙트", this.effectSource, typeof(GameObject), false, GUILayout.Width(uiWidthLarge * 1.5f));

                                if (effectSource != null)
                                {
                                    effectData.effectClips[selection].effectPath = EditorHelper.GetPath(this.effectSource);
                                    effectData.effectClips[selection].effectName = effectSource.name;
                                }
                                else
                                {
                                    effectData.effectClips[selection].effectPath = string.Empty;
                                    effectData.effectClips[selection].effectName = string.Empty;
                                    effectSource = null;
                                }
                                EditorGUILayout.Separator();
                            }
                        EditorGUILayout.EndVertical();
                    }
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();

            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();


        EditorGUILayout.Separator();


        //하단
        EditorGUILayout.BeginHorizontal();
        {
            if(GUILayout.Button("Reload Settings"))
            {
                effectData = CreateInstance<EffectData>();
                effectData.LoadData();
                selection = 0;
                this.effectSource = null;
            }
            if (GUILayout.Button("Save"))
            {
                effectData.SavaData();
                CreatEnumStructure();
                //파일이 추가되었으니 유니티 에디터를 다시한번 리프레시 해라
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
        }
        EditorGUILayout.EndHorizontal();
    }


    //enum만들어주기
    public void CreatEnumStructure()
    {
        string enumName = "EffectList";
        StringBuilder builder = new StringBuilder();
        builder.AppendLine();
        for (int i = 0; i < effectData.names.Length; i++)
        {
            if (effectData.names[i] != string.Empty)
            {
                builder.AppendLine("       " + effectData.names[i] + " = " + i + " , ");
            }
        }
        EditorHelper.CreateEnumStructure(enumName, builder);
    }

}
