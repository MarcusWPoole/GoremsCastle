using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportManagement : MonoBehaviour
{
    [SerializeField] private List<GameObject> teleportSelectPoints;
    [HideInInspector] public List<Teleport> teleporterScripts;
    [HideInInspector] public List<MeshRenderer> renderers;
    [SerializeField] private List<Transform> hands;
    [SerializeField] private Transform xrOrigin;
    private MeshRenderer rendererToBeEnabled;

    private Transform newDestination;

    private int handCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject point in teleportSelectPoints)
        {
            teleporterScripts.Add(point.GetComponent<Teleport>());
            renderers.Add(point.GetComponent<MeshRenderer>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(handCount.ToString());
        if (handCount > 0)
        {
            TeleportHighlight();
        }
        else
        {
            for (int i = 0; i < teleportSelectPoints.Count; i++)
            {
                renderers[i].enabled = false;
            }
        }

        if(Input.GetKeyDown("space"))
        {
            PerformTeleport();
        }
        
    }

    void TeleportHighlight()
    {
        float currentMinDistance = 10000.0f;
        float measuredDistance;
        foreach (Transform hand in hands)
        {
            for (int i = 0; i < teleportSelectPoints.Count; i++)
            {
                renderers[i].enabled = false;
                measuredDistance = Vector3.Distance(teleportSelectPoints[i].transform.position, hand.position);
                if (measuredDistance < currentMinDistance)
                {
                    currentMinDistance = measuredDistance;
                    newDestination = teleporterScripts[i].teleportDestination;
                    rendererToBeEnabled = renderers[i];
                }
            }
        }

        rendererToBeEnabled.enabled = true;
    }

    public void PerformTeleport()
    {
        if (handCount > 0)
        {
            xrOrigin.position = newDestination.position;
        }
    }

    // Code below used from Theremin in the Interaction and Audio Coursework:
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Hand")
        {
            handCount++;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Hand")
        {
            handCount--;
        }
    }
}
