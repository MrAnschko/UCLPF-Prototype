using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StandardObject", menuName = "UCLPF/StandardSonificationObject", order = 1)]
public class SonificationHandler:ScriptableObject
{
    [SerializeField] private List<SonificationClip> audioClips = new List<SonificationClip>();
    

    public List<SonificationClip> AudioClips
    {
        get
        {
            List<SonificationClip> ret = new(audioClips);
            return ret;
        }
    }

    public virtual string DisplayName(int id)
    {
        return AudioClips[id].clipName;
    }

    public virtual SonificationClip GetClipFromID(int id)
    {
        return AudioClips[id];
    }

    
}
