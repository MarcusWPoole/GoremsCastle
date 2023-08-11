using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

// This script handles the tutorial for placing defenses as a giant

public class DefencePlacementTutorial : MonoBehaviour
{
    [SerializeField] public PositionSystem teleportationSystem;
    [SerializeField] public TeleportMarker giantTeleportMarker;
    private bool playerTeleported;

    [SerializeField] public List<AudioClip> tutorialAudio;

    private Dictionary<int, AudioClip> audioStageDic;
    private bool audioClipPlayed;
    private AudioSource defencePlacementAS;

    [SerializeField] public List<Sprite> tutorialImages;

    private Dictionary<int, Sprite> imageStageDic;
    private bool imageChanged;

    [Header("Tutorial Objects")]

    [SerializeField] public GameObject towerPrefab;
    private GameObject[] tutorialTowers; // These need to be instantiated
    private bool towersSpawned;
    [SerializeField] public XRSocketInteractor firstTowerLocation;
    [SerializeField] public List<XRSocketInteractor> allAvailableSockets;

    [SerializeField] public List<Transform> towerSpawnLocations;
    private Quaternion towerRotation;

    // Images
    [SerializeField] public Canvas towerTutorialImageObj;
    private Image towerTutorialImage;
    
    // Tutorial Variables
    public bool tutorialComplete;
    public bool tutorialRunning;
    private int stageCounter;

    // Debug Messages
    private string towerPlacedDM = "Tower Placed";

    // Start is called before the first frame update
    void Start()
    {
        towerTutorialImage = towerTutorialImageObj.GetComponent<Image>();
        EnableDisableImage(false);
        
        defencePlacementAS = GetComponent<AudioSource>();
        tutorialComplete = tutorialRunning = audioClipPlayed = imageChanged = towersSpawned = false;
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

        tutorialTowers = new GameObject[3];
        towerRotation = new Quaternion(0, 0, 0, 0);
    }

    public IEnumerator PlayTutorial()
    {

        tutorialRunning = true;
        
        // STAGES
        // Stage 1: Now Giant, Pickup Tower
        // Stage 2: Move tower over spot
        // Stage 3: (No Audio) Release Tower
        // Stage 4: (No Image) Tower placed, place the rest (Make an image?)

        switch(stageCounter)
        {
            case 0:
                SpawnTowers();
                TeleportPlayer();
                SwitchAndPlayAudio(audioStageDic[0]);
                SwitchImage(imageStageDic[0]);
                yield return new WaitUntil(() => CheckTowerPickup() == true);
                NextStage();
                break;
            case 1:
                SwitchAndPlayAudio(audioStageDic[1]);
                SwitchImage(imageStageDic[1]);
                yield return new WaitUntil(() => CheckTowerHover() == true);
                NextStage();
                break;
            case 2:
                SwitchImage(imageStageDic[2]);
                yield return new WaitUntil(() => CheckTowerPlaced() == true);
                NextStage();
                break;
            case 3:
                SwitchAndPlayAudio(audioStageDic[2]);
                //SwitchImage(imageStageDic[3]) Here in case I make an image
                yield return new WaitUntil(() => CheckAllTowersPlaced() == true);
                NextStage();
                break;
            case 4:
                tutorialComplete = true;
                tutorialRunning = false;
                EnableDisableImage(false);
                yield return new WaitForSeconds(2.0f);
                HideSocketMarkers();
                StopCoroutine(PlayTutorial());
                break;
        }

        StartCoroutine(PlayTutorial());
    }

    bool CheckTowerPickup()
    {
        foreach (GameObject tower in tutorialTowers)
        {
            if (tower.GetComponent<XRGrabInteractable>().isSelected)
            {
                return true;
            }
        }
        return false;
    }

    bool CheckTowerHover()
    {
        foreach (GameObject tower in tutorialTowers)
        {
            if (firstTowerLocation.interactablesHovered.Contains(tower.GetComponent<XRGrabInteractable>()))
            {
                return true;
            }
        }
        return false;
    }

    bool CheckTowerPlaced()
    {
        foreach (GameObject tower in tutorialTowers)
        {
            if (firstTowerLocation.interactablesSelected.Contains(tower.GetComponent<XRGrabInteractable>()))
            {
                return true;
            }
        }
        return false;   
    }

    bool CheckAllTowersPlaced()
    {
        foreach (GameObject tower in tutorialTowers)
        {
            bool towerPlaced = false;

            foreach (XRSocketInteractor towerSocket in allAvailableSockets)
            {
                if (towerSocket.interactablesSelected.Contains(tower.GetComponent<XRGrabInteractable>()))
                {
                    towerPlaced = true;
                    break;  
                }
                else
                {
                    continue;
                }
            }

            if (towerPlaced)
            {
                continue;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    void SpawnTowers()
    {
        if (!towersSpawned)
        {
            tutorialTowers = new GameObject[3];

            for (int j = 0; j < tutorialTowers.Length; j++)
            {
                tutorialTowers[j] = Instantiate(towerPrefab, towerSpawnLocations[j].position, towerRotation);
            }
            towersSpawned = true;
        }
    }
    
    void TeleportPlayer()
    {
        if (!playerTeleported)
        {
            giantTeleportMarker = teleportationSystem.TeleportToMarker(giantTeleportMarker);
            playerTeleported = true;
            EnableDisableImage(true);
        }
    }
    
    void SwitchAndPlayAudio(AudioClip audio)
    {
        if (!audioClipPlayed)
        {
            defencePlacementAS.clip = audio;
            defencePlacementAS.Play();
            audioClipPlayed = true;
        }
    }

    void SwitchImage(Sprite newImage)
    {
        if (!imageChanged)
        {
            towerTutorialImage.overrideSprite = newImage;
            imageChanged = true;
        }
    }

    internal void EnableDisableImage(bool state)
    {
        towerTutorialImageObj.gameObject.SetActive(state);
    }

    void NextStage()
    {
        imageChanged = false;
        audioClipPlayed = false;
        Debug.Log(stageCounter);
        stageCounter++;
    }

    void HideSocketMarkers()
    {
        foreach (XRSocketInteractor socket in allAvailableSockets)
        {
            ParticleSystem particleRing = socket.GetComponent<ParticleSystem>();

            particleRing.Stop();
        }
    }

}
