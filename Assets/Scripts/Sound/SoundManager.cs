using UnityEngine;

namespace Sound
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioClip[] sounds;
        
        [SerializeField] private AudioSource bgmPlayer;
        [SerializeField] private AudioClip inGameBGM;
[SerializeField] private AudioClip battleBGM;
[SerializeField] private AudioSource sfxPlayer;
        
        private const string BattleBgmName = "战斗音乐-冷静赛博空间Y";
private const string InGameBgmName = "局内音乐-数字悲歌（Y";
public static SoundManager Instance { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        
            Instance = this;
        }
private void Start()
{
    EnsureClipsLoaded();
    if (bgmPlayer == null) return;
    if (inGameBGM != null)
    {
        bgmPlayer.clip = inGameBGM;
        bgmPlayer.loop = true;
        bgmPlayer.Play();
    }
    else if (!bgmPlayer.isPlaying && bgmPlayer.clip != null)
    {
        bgmPlayer.loop = true;
        bgmPlayer.Play();
    }
}

        public void PlaySFX(int index)
        {
            if (index < 0 || index >= sounds.Length) return;
            sfxPlayer.PlayOneShot(sounds[index]);
        }
    

public void PlayBattleBGM()
{
    if (bgmPlayer == null) return;
    if (battleBGM == null)
    {
        Debug.LogWarning("[SoundManager] Battle BGM clip is not assigned");
        return;
    }
    if (bgmPlayer.clip == battleBGM && bgmPlayer.isPlaying) return;
    bgmPlayer.clip = battleBGM;
    bgmPlayer.loop = true;
    bgmPlayer.Play();
}


public void PlayInGameBGM()
{
    if (bgmPlayer == null) return;
    if (inGameBGM == null)
    {
        Debug.LogWarning("[SoundManager] In-game BGM clip is not assigned");
        return;
    }
    if (bgmPlayer.clip == inGameBGM && bgmPlayer.isPlaying) return;
    bgmPlayer.clip = inGameBGM;
    bgmPlayer.loop = true;
    bgmPlayer.Play();
}


private void EnsureClipsLoaded()
{
    if (inGameBGM == null)
        inGameBGM = FindClipByName(InGameBgmName);
    if (battleBGM == null)
        battleBGM = FindClipByName(BattleBgmName);
}

private AudioClip FindClipByName(string clipName)
{
    var clips = Resources.FindObjectsOfTypeAll<AudioClip>();
    foreach (var c in clips)
    {
        if (c != null && c.name == clipName)
            return c;
    }
#if UNITY_EDITOR
    var guids = UnityEditor.AssetDatabase.FindAssets($"{clipName} t:AudioClip");
    if (guids != null && guids.Length > 0)
    {
        var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
        var clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(path);
        if (clip != null) return clip;
    }
#endif
    return null;
}
}
}
