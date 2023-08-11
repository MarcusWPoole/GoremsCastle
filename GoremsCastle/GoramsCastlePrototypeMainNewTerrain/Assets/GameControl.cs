using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    [SerializeField] public bool tutorialToggle; // This toggle is for debugging, if not on, then the first wave should occur
    [SerializeField] public bool cutsceneToggle;

    [Header("SubControllers")]
    [SerializeField] public TutorialControl tutorialController;
    [SerializeField] public WaveControl waveController;
    [SerializeField] public PrepControl prepController;
    [SerializeField] public GameOver gameOverScript;

    [SerializeField] public int numberOfRounds = 2;
    public int roundCounter;

    private bool roundComplete;
    private bool roundInProgress;
    private bool postTutorialRoundComplete;
    
    // Start is called before the first frame update
    void Start()
    {
        roundComplete = roundInProgress = postTutorialRoundComplete = false;
        roundCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOverScript.gameEnded)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            if (tutorialToggle && !tutorialController.tutorialComplete && !tutorialController.tutorialRunning)
            {
                StartCoroutine(tutorialController.PlayTutorial());
            }
            else if (tutorialToggle && tutorialController.tutorialComplete && !roundInProgress && !postTutorialRoundComplete)
            {
                StartCoroutine(waveController.NextWave());
                postTutorialRoundComplete = roundInProgress = true;
                roundCounter++;
            }
            else if ((!tutorialToggle || (tutorialController.tutorialComplete && postTutorialRoundComplete)) && !roundInProgress && roundCounter != numberOfRounds)
            {
                ResetForNextRound();
                StartCoroutine(GameplayLoop());
            }
            else if (roundCounter == numberOfRounds)
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }

    }

    void ResetForNextRound()
    {
        waveController.waveComplete = prepController.prepComplete = roundComplete = false;
    }
    
    
    IEnumerator GameplayLoop()
    {
        roundInProgress = true;
        
        if (!prepController.prepInProgress && !prepController.prepComplete)
        {
            StartCoroutine(prepController.PlayPreparationPhase());
        }

        yield return new WaitUntil(() => prepController.prepComplete == true);

        if (!waveController.waveInProgress && !waveController.waveComplete)
        {
            StartCoroutine(waveController.NextWave());
        }

        yield return new WaitUntil(() => waveController.waveComplete == true);

        roundCounter++;
        roundInProgress = false;
        StopCoroutine(GameplayLoop());
    }
}
