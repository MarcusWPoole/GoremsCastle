using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

// This script handles the tutorial for using the Bow and Teleport functions

// ATM this script only deals with the Bow Tutorial

public class BowAndTeleportTutorial : MonoBehaviour
{
    [SerializeField] public PositionSystem teleportationSystem;
    [SerializeField] public TeleportMarker firstTeleportMarker;
    [SerializeField] public TeleportMarker mapTeleportMarker;
    [SerializeField] public TeleportMarker giantTeleportMarker;

    [SerializeField] public List<AudioClip> tutorialAudio;

    private Dictionary<int, AudioClip> audioStageDic;
    private bool audioClipPlayed;
    private AudioSource bowAndTeleportAS;

    [SerializeField] public List<Sprite> tutorialImages;

    private Dictionary<int, Sprite> imageStageDic;
    private bool imageChanged;

    [Header("Tutorial Objects")]
    
    [SerializeField] public XRGrabInteractable magicBowString;
    [SerializeField] public XRGrabInteractable magicBowGrip;

    // Images 
    [SerializeField] public Canvas bowTutorialImageObj;
    [SerializeField] public Canvas teleportTutorialImageObj;
    private Image bowTutorialImage;
    private Image teleportTutorialImage;

    // Tutorial Variables
    public bool tutorialComplete;
    public bool tutorialRunning;
    private bool playerTeleported;
    private int stageCounter;

    
    // Start is called before the first frame update
    void Start()
    {
        bowTutorialImage = bowTutorialImageObj.GetComponent<Image>();
        teleportTutorialImage = teleportTutorialImageObj.GetComponent<Image>();

        EnableDisableBowImage(false);
        EnableDisableTeleportImage(false);
        
        bowAndTeleportAS = GetComponent<AudioSource>();

        tutorialComplete = tutorialRunning = audioClipPlayed = imageChanged = false;
        stageCounter = 0;

        audioStageDic = new Dictionary<int, AudioClip>();
        imageStageDic = new Dictionary<int, Sprite>();

        for (int i = 0; i < tutorialAudio.Count; i++)
        {
            audioStageDic.Add(i, tutorialAudio[i]);
        }

        for (int j = 0; j < tutorialImages.Count; j++)
        {
            imageStageDic.Add(j, tutorialImages[j]);
        }
    }

    public IEnumerator PlayTutorial()
    {

        tutorialRunning = true;

        // STAGES
        // Stage 0: Player grabs bow handle
        // Stage 1: (No Audio) Player pulls back bow string
        // Stage 2: (No Audio) Player releases bow string
        // Stage 3: Player opens map
        // Stage 4: (No audio?) Player picks a new tower to teleport to?

        switch(stageCounter)
        {
            case 0:
                TeleportPlayer();
                SwitchAndPlayAudio(audioStageDic[0]);
                SwitchImage(imageStageDic[0]);
                yield return new WaitUntil(() => CheckBowHandle() == true);
                NextStage();
                break;
            case 1:
                SwitchImage(imageStageDic[1]);
                SwitchAndPlayAudio(audioStageDic[1]);
                yield return new WaitUntil(() => CheckBowString() == true);
                NextStage();
                break;
            case 2:
                SwitchImage(imageStageDic[2]);
                yield return new WaitUntil(() => CheckBowFired() == true);
                EnableDisableBowImage(false);
                NextStage();
                break;
            case 3:
                yield return new WaitForSeconds(2.0f);
                SwitchAndPlayAudio(audioStageDic[2]);
                EnableDisableTeleportImage(true);
                SwitchImage(imageStageDic[3]);
                yield return new WaitUntil(() => MapOpened() == true);
                NextStage();
                break;
            case 4:
                SwitchImage(imageStageDic[4]);
                yield return new WaitUntil(() => PlayerSelfTeleported() == true);
                EnableDisableTeleportImage(false);
                NextStage();
                break;
            case 5:
                tutorialComplete = true;
                tutorialRunning = false;
                DisableLastMarker();
                yield return new WaitForSeconds(2.0f);
                StopCoroutine(PlayTutorial());
                break;
        }

        StartCoroutine(PlayTutorial());
    }

    void DisableLastMarker()
    {
        teleportationSystem.SetEnabledTeleportMarker(firstTeleportMarker, false);
    }

    bool CheckBowHandle()
    {
        return magicBowGrip.isSelected;
    }

    bool CheckBowString()
    {
        return magicBowString.isSelected;
    }

    bool CheckBowFired()
    {
        return !magicBowString.isSelected;
    }

    bool MapOpened()
    {
        return teleportationSystem._map.activeSelf;
    }

    bool PlayerSelfTeleported()
    {
        return (teleportationSystem.GetCurrentMarker() == mapTeleportMarker);
    }

    void TeleportPlayer()
    {
        if (!playerTeleported)
        {
            firstTeleportMarker = teleportationSystem.TeleportToMarker(firstTeleportMarker);
            teleportationSystem.SetEnabledTeleportMarker(giantTeleportMarker, false);
            EnableDisableBowImage(true);
            playerTeleported = true;
        }
    }
    
    void SwitchAndPlayAudio(AudioClip audio)
    {
        if (!audioClipPlayed)
        {
            bowAndTeleportAS.clip = audio;
            bowAndTeleportAS.Play();
            audioClipPlayed = true;
        }
    }

    void SwitchImage(Sprite newImage)
    {
        if (!imageChanged)
        {
            if (stageCounter < 3)
            {
                bowTutorialImage.overrideSprite = newImage;
            }
            else if (stageCounter >= 3)
            {
                teleportTutorialImage.overrideSprite = newImage;
            }

            imageChanged = true;
        }
    }

    internal void EnableDisableBowImage(bool state)
    {
        bowTutorialImageObj.gameObject.SetActive(state);
    }

    internal void EnableDisableTeleportImage(bool state)
    {
        teleportTutorialImageObj.gameObject.SetActive(state);
    }

    void NextStage()
    {
        imageChanged = false;
        audioClipPlayed = false;
        stageCounter++;
    }
}
