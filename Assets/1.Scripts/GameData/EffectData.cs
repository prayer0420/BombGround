using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using System.IO;

/// <summary>
/// 이펙트 클립 리스트와 이펙트 파일 이름과 경로를 가지고 있으며 
/// 파일을 읽고 쓰는 기능을 가지고 있다.
/// </summary>

public class EffectData : BaseData
{
    public new const string dataDirectory = "/9.ResourcesData/Resources/Data/";


    public EffectClip[] effectClips = new EffectClip[0];

    public string clipPath = "Effects/";
    private string xmlFilePath = "";
    private string xmlFileName = "effectData.xml";
    private string dataPath = "Data/effectData";
    //XML 구분자
    private const string EFFECT = "effect"; //저장 키
    private const string CLIP = "clip"; // 저장 키

    private EffectData() { }

    //읽어오고, 저장하고, 데이터를 삭제하고, 특정 클립을 얻어오고, 복사하는 기능
    
    //읽어오기
    public void LoadData()
    {
        xmlFilePath = Application.dataPath + dataDirectory;
        TextAsset asset = (TextAsset)ResourceManager.Load(dataPath);

        //잘못 불러온 경우
        if (asset == null || asset.text == null)
        {
            this.AddData("NewEffect");
            return;
        }

        //xml읽어오기
        using(XmlTextReader reader = new XmlTextReader(new StringReader(asset.text)))
        {
            int currentID = 0;
            while(reader.Read())
            {
                if(reader.IsStartElement())
                {
                    switch(reader.Name)
                    {
                        case "length":
                            int length = int.Parse(reader.ReadString());
                            this.names = new string[length];
                            this.effectClips = new EffectClip[length];
                            break;
                        case "id":
                            currentID = int.Parse(reader.ReadString()) ;
                            this.effectClips[currentID] = new EffectClip();
                            this.effectClips[currentID].realId = currentID ;
                            break;
                        case "name":
                            this.names[currentID] = reader.ReadString();
                            break;
                        case "effectType":
                            this.effectClips[currentID].effectType  = (EffectType) Enum.Parse(typeof(EffectType), reader.ReadString());
                            break;
                        case "effectName":
                            this.effectClips[currentID].effectName = reader.ReadString();
                            break;
                        case "effectPath":
                            this.effectClips[currentID].effectPath = reader.ReadString();   
                            break;

                    }
                }
            }
        }
    }

    //xml저장하기
    public void SavaData()
    {
        using(XmlTextWriter xml = new XmlTextWriter(xmlFilePath + xmlFileName, System.Text.Encoding.Unicode))
        {
            //xml의 데이터를 쓰려면
            xml.WriteStartDocument();
            //헤드
            xml.WriteStartElement(EFFECT);

            //데이타의 길이 얻어오기
            xml.WriteElementString("length", GetDataCount().ToString());
            for (int i = 0; i < this.names.Length; i++)
            {
                EffectClip clip = this.effectClips[i];
                xml.WriteStartElement(CLIP);
                xml.WriteElementString("id", i.ToString());
                xml.WriteElementString("name", this.names[i]);
                xml.WriteElementString("effectType", clip.effectType.ToString());
                xml.WriteElementString("effectPath", clip.effectPath);
                xml.WriteElementString("effectName", clip.effectName);
                xml.WriteEndElement();
            }
            xml.WriteEndElement();
            xml.WriteEndDocument();
        }
    }

    public override int AddData(string newName)
    {
        //data가없으면
        if (this.names == null)
        {
            //초기화
            this.names = new string[] { newName };
            
            this.effectClips = new EffectClip[] { new EffectClip() };
        }
        else
        {
            this.names = ArrayHelper.Add(newName, this.names);
            this.effectClips = ArrayHelper.Add(new EffectClip(), this.effectClips);
        }

        return GetDataCount();
    }
    

    public override void RemoveData(int index)
    {
        this.names = ArrayHelper.Remove(index, this.names);

        if(this.names.Length == 0)
        {
            this.names = null;
        }

        this.effectClips = ArrayHelper.Remove(index, effectClips);
    }


    public void ClearData()
    {
        foreach(EffectClip clip in this.effectClips)
        {
            clip.ReleaseEffect();
        }
        this.effectClips = null;
        this.name = null;
    }

    public EffectClip GetCopy(int index)
    {
        if(index<0 || index >= this.names.Length)
        {
            return null;
        }
        EffectClip original = this.effectClips[index];
        EffectClip clip = new EffectClip();
        clip.effectFullPath = original.effectFullPath;
        clip.effectName = original.effectName;
        clip.effectType = original.effectType;
        clip.effectPath = original.effectFullPath;
        clip.realId = this.effectClips.Length;
        return clip;
    }


    /// <summary>
    /// 원하는 인덱스를 프리로딩해서 찾아준다
    /// </summary>
    
    public EffectClip GetClip(int index)
    {
        if(index<0 || index >= this.effectClips.Length)
        {
            return null;
        }
        effectClips[index].PreLoad();
        return effectClips[index];
    }

    public override void Copy(int index)
    {
        this.names = ArrayHelper.Add(this.names[index], this.names);
        this.effectClips = ArrayHelper.Add(GetCopy(index), this.effectClips);
    }
}
