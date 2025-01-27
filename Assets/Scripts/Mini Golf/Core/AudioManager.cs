using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager am;
    
    [SerializeField] List<AudioClip> musicList = new List<AudioClip>();  
    [SerializeField] List<AudioClip> waterList = new List<AudioClip>();
    [SerializeField] List<AudioClip> strokeList = new List<AudioClip>();
    [SerializeField] List<AudioClip> dialogueList = new List<AudioClip>();
    [SerializeField] List<AudioClip> rollSoundList = new List<AudioClip>();


    AudioSource musicSC;
    AudioSource sfxSC;
    AudioSource dialogueSC;

    AudioSource playerRollSC;
    AudioSource playerStrokeSC;

    private void Awake()
    {
        if (am == null)
            am = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        musicSC = transform.Find("Music").GetComponent<AudioSource>();
        sfxSC = transform.Find("SFX").GetComponent<AudioSource>();
        dialogueSC = transform.Find("Dialogue").GetComponent<AudioSource>();

        playerRollSC = GameObject.FindWithTag("Player").transform.Find("Roll Source").GetComponent<AudioSource>();
        playerStrokeSC = GameObject.FindWithTag("Player").transform.Find("Stroke Source").GetComponent<AudioSource>();
    }

    public void WaterHazardSound(Vector3 pos)
    {
        AudioSource.PlayClipAtPoint(waterList[Random.Range(0, waterList.Count)], pos);
    }

    public void PlayRollSound(int val)
    {
        if (val > rollSoundList.Count)
            playerRollSC.clip = rollSoundList[0];
        else
            playerRollSC.clip = rollSoundList[val];

        playerRollSC.Play();
    }

    public void PlayStrikeSound()
    {
        playerStrokeSC.clip = strokeList[Random.Range(0, strokeList.Count)];
        playerStrokeSC.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSC.clip = clip;
        sfxSC.Play();
    }
}
