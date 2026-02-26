using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StandardObject", menuName = "UCLPF/StandardSonificationObject", order = 1)]
public class SonificationHandler:ScriptableObject
{
    public List<AudioClip> audioClips = new List<AudioClip>();
    public List<string> names;

    public virtual string DisplayName(int id)
    {
        return names[id];
    }

    public virtual AudioClip GetClipFromID(int id)
    {
        return audioClips[id];
    }

}
