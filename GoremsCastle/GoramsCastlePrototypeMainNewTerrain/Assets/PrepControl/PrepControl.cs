using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// This script handles the preparation phase in which the player can add new towers to the environment

public class PrepControl : MonoBehaviour
{
    [SerializeField] public PositionSystem teleportationSystem;
    [SerializeField] public TeleportMarker giantMarker;
    private TeleportMarker lastMarker;
    
    [SerializeField] public GameObject tower;

    [SerializeField] public List<XRSocketInteractor> towerSockets;
    private GameObject[] towersAvailable;

    [SerializeField] public List<Transform> towerSpawnLocations;
    private Quaternion towerRotation;

    [SerializeField] public int startingDefences = 3;
    [SerializeField] public int prepTime = 30;
    [SerializeField] public float defenceModifier = 1;

    private AudioSource prepPhaseSFX;
    [SerializeField] public AudioClip towerPlacedSFX;
    [SerializeField] public AudioClip timeRunningOutSFX;

    private bool towersCreated;
    private bool playerTeleported;

    public bool prepInProgress;
    public bool prepComplete; 
    
    // Start is called before the first frame update
    void Start()
    {
        towersCreated = playerTeleported = prepInProgress = prepComplete = false;
        towerRotation = new Quaternion(0, 0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // This method handles the prepartaion phase in which the player places their defences
    public IEnumerator PlayPreparationPhase()
    {
        prepInProgress = true;
        TeleportPlayer();
        
        if (!towersCreated)
        {
            ShowSocketMarkers();
            
            startingDefences = (int)(Mathf.Ceil((float)startingDefences * defenceModifier));
            towersAvailable = new GameObject[startingDefences];
            
            for (int i = 0; i < towersAvailable.Length; i++)
            {
                towersAvailable[i] = Instantiate(tower, towerSpawnLocations[i].position, towerRotation);
            }

            towersCreated = true;
        }
    
        yield return new WaitForSeconds(prepTime);

        foreach (GameObject spawnedTower in towersAvailable)
        {
            foreach (XRSocketInteractor towerSocket in towerSockets)
            {
                if (towerSocket.attachTransform.position == spawnedTower.transform.position)
                {
                    spawnedTower.GetComponent<XRGrabInteractable>().enabled = false;
                    break;  
                }
                else
                {
                    continue;
                }
            }

            if (spawnedTower.GetComponent<XRGrabInteractable>().enabled)
            {
                Destroy(spawnedTower);
            }
        }
        
        prepInProgress = false;
        prepComplete = true;
        EnableDisableMapAndMarkers(true);
        HideSocketMarkers();
        lastMarker = teleportationSystem.TeleportToMarker(lastMarker);
        StopCoroutine(PlayPreparationPhase());
    }

    void TeleportPlayer()
    {
        if (!playerTeleported)
        {
            lastMarker = teleportationSystem.GetCurrentMarker();
            giantMarker = teleportationSystem.TeleportToMarker(giantMarker);
            EnableDisableMapAndMarkers(false);
        }
    }

    void EnableDisableMapAndMarkers(bool state)
    {
        foreach (TeleportMarker marker in teleportationSystem.GetTeleportMarkers())
        {
            if (marker != giantMarker)
            {
                teleportationSystem.SetEnabledTeleportMarker(marker, state);
            }
        }

        // NEED SCRIPT HERE TO DISABLE THE MAP BEING OPENED?
    }

    void HideSocketMarkers()
    {
        foreach (XRSocketInteractor socket in towerSockets)
        {
            ParticleSystem particleRing = socket.GetComponent<ParticleSystem>();

            particleRing.Stop();
        }
    }

    void ShowSocketMarkers()
    {
        foreach (XRSocketInteractor socket in towerSockets)
        {
            ParticleSystem particleRing = socket.GetComponent<ParticleSystem>();

            particleRing.Play();
        }
    }
}
