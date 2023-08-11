using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// This script controls the starting tutorial for the player,
// and calls several other scripts, one for the opening cutscene,
// and one for each tutorial.

// The current version is empty, and is being used to as an excersise in identifying
// which audio clips and strings will be needed for the tutorial, plus some basic 
// functions for updating text.

public class TutorialControl : MonoBehaviour
{
    [SerializeField] public PositionSystem teleportationSystem;
    
    [SerializeField] public AudioClip roundStartAudio;

    [Header("Tutorials and Cutscenes")]

    [SerializeField] public DefencePlacementTutorial defenceTutorial;
    [SerializeField] public BowAndTeleportTutorial bowAndTeleportTutorial;
    [SerializeField] public CannonTutorial cannonTutorial;

    public bool tutorialRunning;
    public bool tutorialComplete;

    void Start()
    {
        tutorialRunning = tutorialComplete = false;
        //DisableMarkersForTutorial();
    }

    void DisableMarkersForTutorial()
    {
        foreach (TeleportMarker marker in teleportationSystem.GetTeleportMarkers())
        {
            if (marker != bowAndTeleportTutorial.firstTeleportMarker || marker != bowAndTeleportTutorial.mapTeleportMarker || marker != defenceTutorial.giantTeleportMarker)
            {
                teleportationSystem.SetEnabledTeleportMarker(marker, false);
            }
        }
    }
    
    public IEnumerator PlayTutorial()
    {
        tutorialRunning = true;
        
        if (!defenceTutorial.tutorialComplete && !defenceTutorial.tutorialRunning)
        {
            StartCoroutine(defenceTutorial.PlayTutorial());
        }

        yield return new WaitUntil(() => defenceTutorial.tutorialComplete == true);

        if (!bowAndTeleportTutorial.tutorialRunning && !bowAndTeleportTutorial.tutorialComplete)
        {
            StartCoroutine(bowAndTeleportTutorial.PlayTutorial());
        }

        yield return new WaitUntil(() => bowAndTeleportTutorial.tutorialComplete == true);
        
        if (!cannonTutorial.tutorialRunning && !cannonTutorial.tutorialComplete)
        {
            StartCoroutine(cannonTutorial.PlayTutorial());
        }

        yield return new WaitUntil(() => cannonTutorial.tutorialComplete == true);

        tutorialComplete = true;
        tutorialRunning = false;
        StopCoroutine(PlayTutorial());
    }

}
