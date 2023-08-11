using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] public PositionSystem teleportationSystem;
    [SerializeField] public TeleportMarker gameOverMarker;

    [SerializeField] public AudioSource gameOverMusic;

    public bool gameEnding;
    public bool gameEnded;
    
    // Start is called before the first frame update
    void Start()
    {
        gameEnding = gameEnded = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Enemy")
        {
            if (!gameEnding)
            {
                GameOverScene();
                gameEnding = true;
            }
        }
    }

    void GameOverScene()
    {
        if (teleportationSystem.GetCurrentMarker() == gameOverMarker)
        {
            gameOverMarker = teleportationSystem.TeleportToMarker(gameOverMarker);
        }

        StartCoroutine(WaitForMusic());
    }

    IEnumerator WaitForMusic()
    {
        gameOverMusic.Play();

        yield return new WaitWhile(() => gameOverMusic.isPlaying == true);

        gameEnded = true;
        StopCoroutine(WaitForMusic());
    }
}
