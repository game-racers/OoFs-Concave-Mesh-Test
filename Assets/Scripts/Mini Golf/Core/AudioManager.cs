using gameracers.Dialogue;
using gameracers.MiniGolf.Core;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioMixer masterMixer;


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

    GolfGameManager gm;
    GameObject optionsTemp;
    [SerializeField] GameObject menuBG;

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

        gm = GameObject.FindWithTag("GameController").GetComponent<GolfGameManager>();
    }

    private void Update()
    {
        if (!dialogueSC.isPlaying)
        {
            if (dialogueSC.clip != dialogueList[0])
            {
                DialogueManager.dm.CloseTextBauble();
            }
        }
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

    public void PlayDialogue(int index)
    {
        Debug.Log(dialogueList[index].name);
        dialogueSC.clip = dialogueList[index];
        dialogueSC.Play();
        DialogueManager.dm.NextHoleText(index - 1);
    }

    #region UI Elements
    public void ChangeMaster(float val)
    {
        masterMixer.SetFloat("Master", Mathf.Log10(val) * 20);
    }

    public void ChangeMusic(float val)
    {
        masterMixer.SetFloat("Music", Mathf.Log10(val) * 20);
    }

    public void ChangeSFX(float val)
    {
        masterMixer.SetFloat("SFX", Mathf.Log10(val) * 20);
    }

    public void ChangeDialogue(float val)
    {
        masterMixer.SetFloat("Dialogue", Mathf.Log10(val) * 20);
    }

    public void BackButton()
    {
        if (gm.GetGameState() == MiniGolfState.GameStart)
        {
            menuBG.SetActive(true);
            menuBG.transform.GetChild(0).gameObject.SetActive(true);
            DialogueManager.dm.gameObject.SetActive(true);
        }
        else
        {
            gm.ChangeGameState(MiniGolfState.MiniGolf);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false; 
        }
    }
    #endregion
}
