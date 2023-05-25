using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using System.IO;

/// <summary>
/// 사운드 클립을 배열로 소지, 사운드 데이터를 저장하고 로드하고,
/// 프리로딩을 갖고 있다.
/// </summary>
public class SoundData : BaseData
{
    public new const string dataDirectory = "/9.ResourcesData/Resources/Data/";

    public SoundClip[]  soundClips = new SoundClip[0];

    private string clipPath = "Sound/";
    private string xmlFilePath = ""; //저장용 경로
    private string xmlFileName = "soundData.xml"; //저장용 이름
    private string dataPath = "Data/soundData"; //리소스 로드용 
    private static string SOUND = "sound";
    private static string CLIP = "clip";
    public SoundData() { }
    
    public void SavaData()
    {
        using(XmlTextWriter xml = new XmlTextWriter(xmlFilePath + xmlFileName, System.Text.Encoding.Unicode))
        {
            xml.WriteStartDocument();
            //헤드
            xml.WriteStartElement(SOUND);

            xml.WriteElementString("length", GetDataCount().ToString());
            xml.WriteWhitespace("\n");

            //반복문으로 클립데이터 넣어주기
            for (int i = 0; i < this.names.Length; i++)
            {
                SoundClip clip = this.soundClips[i];
                xml.WriteStartElement(CLIP);
                xml.WriteElementString("id", i.ToString());
                xml.WriteElementString("name", this.names[i]);
                xml.WriteElementString("loops", clip.checkTime.Length.ToString());
                xml.WriteElementString("maxvol", clip.maxVolume.ToString());
                xml.WriteElementString("pitch", clip.pitch.ToString());
                xml.WriteElementString("dopplerLevel", clip.dopplerLevel.ToString());
                xml.WriteElementString("rolloffMode", clip.rolloffMode.ToString());
                xml.WriteElementString("minDistance", clip.minDistance.ToString());
                xml.WriteElementString("maxDistance", clip.maxDistance.ToString());
                xml.WriteElementString("sparialBlend", clip.spartialBlend.ToString());

                if(clip.isLoop == true)
                {
                    xml.WriteElementString("loop", "true");
                    
                }

                xml.WriteElementString("clippath", clip.clipPath);
                xml.WriteElementString("clipname", clip.clipName);
                xml.WriteElementString("checktimecount", clip.checkTime.Length.ToString() );

                //checktime합쳐서 저장
                string str = "";
                foreach(float t in clip.checkTime )
                {
                    str += t.ToString() + "/";
                }

                xml.WriteElementString("checktime", str);
                str = "";
                xml.WriteElementString("settimecount", clip.checkTime.Length.ToString());
                foreach(float t in clip.setTime)
                {
                    str += t.ToString() + "/";
                }
                xml.WriteElementString("settime", str);

                xml.WriteElementString("type", clip.playType.ToString());


                xml.WriteEndElement();
            }

            xml.WriteEndElement();
            xml.WriteEndDocument();
        }
    }


    public void LoadData()
    {
        xmlFilePath = Application.dataPath + dataDirectory;
        TextAsset asset = (TextAsset)Resources.Load(dataPath,typeof(TextAsset)); 
        if(asset == null || asset.text == null)
        {
            this.AddData("New Sound");
            return;
        }
        using (XmlTextReader reader = new XmlTextReader(new StringReader(asset.text)))
        {
            int currentID = 0;
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch(reader.Name)
                    {
                        case "length":
                            int length = int.Parse(reader.ReadString());
                            this.names = new string[length];
                            this.soundClips = new SoundClip[length];
                             break;
                        case "clip":

                            break;
                        case "id":
                            currentID = int.Parse(reader.ReadString());
                            soundClips[currentID] = new SoundClip();
                            soundClips[currentID].realId = currentID;
                            break;
                        case "name":
                            this.names[currentID] = reader.ReadString();
                            break;

                        case "loops":
                            int count = int.Parse(reader.ReadString());
                            soundClips[currentID].checkTime = new float[count];
                            soundClips[currentID].setTime = new float[count];
                            break;

                        case "maxvol":
                            soundClips[currentID].maxVolume = float.Parse(reader.ReadString());
                            break;
                        case "pitch":
                            soundClips[currentID].pitch = float.Parse(reader.ReadString());
                            break;

                        case "dopplerLevel":
                            soundClips[currentID].dopplerLevel = float.Parse(reader.ReadString());
                            break;

                        case "rolloffMode":
                            soundClips[currentID].rolloffMode = (AudioRolloffMode) Enum.Parse(typeof(AudioRolloffMode), reader.ReadString());
                            break;

                        case "minDistance":
                            soundClips[currentID].minDistance = float.Parse(reader.ReadString());
                            break;
                        case "maxDistance":
                            soundClips[currentID].maxDistance = float.Parse(reader.ReadString());
                            break;
                        case "sparialBlend":
                            soundClips[currentID].spartialBlend = float.Parse(reader.ReadString());
                            break;
                        case "loop":
                            soundClips[currentID].isLoop = true;
                            break;
                        case "clippath":
                            soundClips[currentID].clipPath = reader.ReadString();
                            break;
                        case "clipname":
                            soundClips[currentID].clipName = reader.ReadString();
                            break;
                        case "checktimecount":
                            break;
                        case "checktime":
                            SetLoopTime(true, soundClips[currentID], reader.ReadString());
                            break;
                        case "settime":
                            SetLoopTime(false, soundClips[currentID], reader.ReadString());
                            break;
                        case "type":
                            soundClips[currentID].playType = (SoundPlayType) Enum.Parse(typeof(SoundPlayType), reader.ReadString());
                            break;
                    }
                }
            }
        }
        
        //사전로딩해놓기
        foreach(SoundClip clip in soundClips)
        {
            clip.PreLoad();
        }
        
    }

    void SetLoopTime(bool isCheck, SoundClip clip, string timeString)
    {
        string[] time = timeString.Split('/');

        for (int i = 0; i < time.Length; i++)
        {
            if (time[i] != string.Empty)
            {
                if(isCheck == true)
                {
                    clip.checkTime[i] = float.Parse(time[i]);
                }

                else
                {
                    clip.setTime[i] = float.Parse(time[i]); 
                }
            }
        }
    }

    public override int AddData(string newName)
    {
        if (this.names == null)
        {
            this.names = new string[] { newName };
            this.soundClips = new SoundClip[] { new SoundClip() };
        }
        else
        {
            this.names = ArrayHelper.Add(newName, names);
            this.soundClips = ArrayHelper.Add(new SoundClip(), soundClips);
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

        this.soundClips = ArrayHelper.Remove(index, this.soundClips);
    }


    public SoundClip GetCopy(int index)
    {
        if(index <0 || index >= soundClips.Length)
        {
            return null;
        }

        SoundClip clip = new SoundClip();
        SoundClip original = soundClips[index];
        clip.realId = index;
        clip.clipPath = original.clipPath;
        clip.clipName = original.clipName;
        clip.maxVolume = original.maxVolume;
        clip.pitch = original.pitch;
        clip.dopplerLevel = original.dopplerLevel;
        clip.rolloffMode = original.rolloffMode;
        clip.minDistance = original.minDistance;
        clip.maxDistance = original.maxDistance;
        clip.spartialBlend = original.spartialBlend;
        clip.isLoop = original.isLoop;
        clip.checkTime = new float[original.checkTime.Length];
        clip.setTime = new float[original.setTime.Length];
        clip.playType = original.playType;
        for (int i = 0; i < clip.checkTime.Length; i++) 
        {
            clip.checkTime[i] = original.checkTime[i];
            clip.setTime[i] = original.setTime[i];
        }
        clip.PreLoad();
        return clip;
    }


    public override void Copy(int index)
    {
        this.names = ArrayHelper.Add(this.names[index], this.names);
        this.soundClips = ArrayHelper.Add(GetCopy(index), soundClips);
    }

}
