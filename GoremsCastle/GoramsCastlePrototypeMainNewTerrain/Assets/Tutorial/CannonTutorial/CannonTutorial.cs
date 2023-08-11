using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

// This script handles the tutorial for firing the cannon

public class CannonTutorial : MonoBehaviour
{  
    [SerializeField] public PositionSystem teleportationSystem;
    [SerializeField] public TeleportMarker previousTeleportMarker;
    
    [SerializeField] public List<AudioClip> tutorialAudio;

    private Dictionary<int, AudioClip> audioStageDic;
    private bool audioClipPlayed;
    private AudioSource cannonTutorialAS;

    [SerializeField] public List<Sprite> tutorialImages;

    private Dictionary<int, Sprite> imageStageDic;
    private bool imageChanged;

    [Header("Tutorial Objects")]

    [SerializeField] public XRGrabInteractable rotateCannon;
    [SerializeField] public XRGrabInteractable adjustCannonDistance;
    [SerializeField] public FireCannon cannonControl;

    // Image
    [SerializeField] public Canvas cannonTutorialImageObj;
    private Image cannonTutorialImage;

    // Tutorial Variables
    public bool tutorialComplete;
    public bool tutorialRunning;
    private bool playerTeleported;
    private int stageCounter;
    
    // Start is called before the first frame update
    void Start()
    {    
        cannonTutorialImage = cannonTutorialImageObj.GetComponent<Image>();
        EnableDisableImage(false);
        
        cannonTutorialAS = GetComponent<AudioSource>();
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
    
    void RenableMarkersPostTutorial()
    {
        foreach (TeleportMarker marker in teleportationSystem.GetTeleportMarkers())
        {
            teleportationSystem.SetEnabledTeleportMarker(marker, true);
        }
    }

    public IEnumerator PlayTutorial()
    {
        tutorialRunning = true;

        // STAGES
        // Stage 0: Player grips the back of the cannon to aim
        // Stage 1: (No audio) Player uses lever to increase distance of shot
        // Stage 2: (No audio) Player fires the cannon

        switch (stageCounter)
        {
            case 0:
                //TeleportPlayer(); The Player should already be here!
                SwitchAndPlayAudio(audioStageDic[0]);
                SwitchImage(imageStageDic[0]);
                yield return new WaitUntil(() => CheckCannonAim() == true);
                NextStage();
                break;
            case 1:
                SwitchImage(imageStageDic[1]);
                yield return new WaitUntil(() => CheckCannonDistance() == true);
                NextStage();
                break;
            case 2:
                SwitchImage(imageStageDic[2]);
                yield return new WaitUntil(() => CheckCannonFire() == true);
                NextStage();
                break;
            case 3:
                tutorialComplete = true;
                tutorialRunning = false;
                RenableMarkersPostTutorial();
                EnableDisableImage(false);
                StopCoroutine(PlayTutorial());
                break;
        }

        StartCoroutine(PlayTutorial());
    }

    bool CheckCannonAim()
    {
        return rotateCannon.isSelected;
    }

    bool CheckCannonDistance()
    {
        return adjustCannonDistance.isSelected;
    }

    bool CheckCannonFire()
    {
        return !cannonControl.ready;
    }   
    
    void SwitchAndPlayAudio(AudioClip audio)
    {
        if (!audioClipPlayed)
        {
            cannonTutorialAS.clip = audio;
            cannonTutorialAS.Play();
            audioClipPlayed = true;
        }
    }

    void SwitchImage(Sprite newImage)
    {
        if (!imageChanged)
        {
            if (!cannonTutorialImageObj.gameObject.activeSelf)
            {
                EnableDisableImage(true);
            }
            cannonTutorialImage.overrideSprite = newImage;
            imageChanged = true;
        }
    }
    
    internal void EnableDisableImage(bool state)
    {
        cannonTutorialImageObj.gameObject.SetActive(state);
    }

    void NextStage()
    {
        imageChanged = false;
        audioClipPlayed = false;
        stageCounter++;
    }
    
}
