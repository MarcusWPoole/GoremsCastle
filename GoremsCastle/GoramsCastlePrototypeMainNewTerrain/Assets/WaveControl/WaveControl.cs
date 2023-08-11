using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaveControl : MonoBehaviour
{
    [Header("Enemie Types")]
    [SerializeField] public GameObject golem;
    private GameObject[] livingGolems;
    [SerializeField] public GameObject carrot;
    private GameObject[] livingCarrots;
    [SerializeField] public GameObject skelly;
    private GameObject[] livingSkellys;
    
    [Header("Enemy Values and Components")]
    [SerializeField] public float numberOfGolems;
    [SerializeField] public float numberOfCarrots;
    [SerializeField] public float numberOfSkellys;

    [SerializeField] public float carrotSpawnTimer;
    private bool spawningCarrot;
    [SerializeField] public float skellySpawnTimer;
    private bool spawningSkelly;
    [SerializeField] public float golemSpawnTimer;
    private bool spawningGolem;

    [SerializeField] public float enemyModifier;

    [SerializeField] public Transform enemySpawnLocation;

    [Header("Defences, Waypoints and Heart")]
    [SerializeField] public List<GameObject> gatesInOrder;
    [SerializeField] public GameObject heart;
    private List<Transform> waypoints;
    private List<GameObject> activeDefencesAndHeart;

    [SerializeField] public GameObject firstGate;
    private bool firstGateDestroyed;

    private int golemCounter;
    private int carrotCounter;
    private int skellyCounter;
    private bool allEnemiesDead;

    private NavMeshAgent enemyBehaviour;

    public bool waveComplete;
    public bool waveInProgress;
    
    // Start is called before the first frame update
    void Start()
    {
        SetCountersAndTrackers();
        
        activeDefencesAndHeart = gatesInOrder;
        activeDefencesAndHeart.Add(heart);

        waveComplete = waveInProgress = false;
        spawningSkelly = spawningCarrot = spawningCarrot = allEnemiesDead = false;

        waypoints = new List<Transform>();
        waypoints.Add(firstGate.transform);
    }

    public IEnumerator NextWave()
    {        
        waveInProgress = true;
        
        while (!allEnemiesDead)
        {
            if (UpdateDefencesAndWaypoints())
            {
                UpdateWaypoints();
            }
            if (!spawningCarrot && carrotCounter > 0)
            {
                StartCoroutine(SpawnCarrot());
            }

            if (!spawningSkelly && skellyCounter > 0)
            {
                StartCoroutine(SpawnSkelly());
            }

            if (!spawningGolem && golemCounter > 0)
            {
                StartCoroutine(SpawnGolem());
            }

            yield return new WaitUntil(() => spawningCarrot == false && spawningSkelly == false && spawningGolem == false);
            allEnemiesDead = CheckIfAllEnemiesDead();
        }

        waveInProgress = false;
        waveComplete = true;

        yield return new WaitForSeconds(2.0f);

        SetCountersAndTrackers();
        StopCoroutine(NextWave());
    }

    IEnumerator SpawnCarrot()
    {
        spawningCarrot = true;
        yield return new WaitForSeconds(carrotSpawnTimer);
        livingCarrots[carrotCounter - 1] = Instantiate(carrot, enemySpawnLocation.position, enemySpawnLocation.rotation);
        livingCarrots[carrotCounter - 1].GetComponent<MoveTo>().goal = activeDefencesAndHeart[0].transform;
        carrotCounter--;
        spawningCarrot = false;
        StopCoroutine(SpawnCarrot());
    }

    IEnumerator SpawnSkelly()
    {
        spawningSkelly = true;
        yield return new WaitForSeconds(skellySpawnTimer);
        livingSkellys[skellyCounter - 1] = Instantiate(skelly, enemySpawnLocation.position, enemySpawnLocation.rotation);
        livingSkellys[skellyCounter - 1].GetComponent<MoveTo>().goal = activeDefencesAndHeart[0].transform;
        skellyCounter--;
        spawningSkelly = false;
        StopCoroutine(SpawnSkelly());
    }
    
    IEnumerator SpawnGolem()
    {
        spawningGolem = true;
        yield return new WaitForSeconds(golemSpawnTimer);
        livingGolems[golemCounter - 1] = Instantiate(golem, enemySpawnLocation.position, enemySpawnLocation.rotation);
        livingGolems[golemCounter - 1].GetComponent<MoveToGolem>().goal = activeDefencesAndHeart[0].transform;
        golemCounter--;
        spawningGolem = false;
        StopCoroutine(SpawnGolem());
    }

    bool  UpdateDefencesAndWaypoints()
    {
        bool change = false;
        
        for (int i = 0; i < activeDefencesAndHeart.Count; i++)
        {
            if (activeDefencesAndHeart[i] == null)
            {
                waypoints.Add(activeDefencesAndHeart[i + 1].transform);
                if (i == 0)
                {
                    firstGateDestroyed = true;
                }
                activeDefencesAndHeart.RemoveAt(i);
                change = true;
            }
        }

        return change;
    }

    bool CheckIfAllEnemiesDead()
    {
        foreach (GameObject gameGolem in livingGolems)
        {
            if (gameGolem != null)
            {
                return false;
            }
        }

        foreach (GameObject gameCarrot in livingCarrots)
        {
            if (gameCarrot != null)
            {
                return false;
            }
        }

        foreach (GameObject gameSkelly in livingSkellys)
        {
            if (gameSkelly != null)
            {
                return false;
            }
        }

        return true;
    }

    void SetCountersAndTrackers()
    {
        golemCounter = (int)(Mathf.Ceil(numberOfGolems * enemyModifier));
        carrotCounter = (int)(Mathf.Ceil(numberOfCarrots * enemyModifier));
        skellyCounter = (int)(Mathf.Ceil(numberOfSkellys * enemyModifier));

        livingGolems = new GameObject[golemCounter];
        livingCarrots = new GameObject[carrotCounter];
        livingSkellys = new GameObject[skellyCounter];
    }    
    
    void AddWaypoints(GameObject enemy)
    { 
        if (enemy.name == golem.name)
        {
            enemy.GetComponent<MoveToGolem>().waypoints = waypoints;
        }
        else
        {
            enemy.GetComponent<MoveTo>().waypoints = waypoints;
        }
    }

    void UpdateWaypoints()
    {
        foreach (GameObject golem in livingGolems)
        {
            if (golem != null)
            {
                golem.GetComponent<MoveToGolem>().waypoints = waypoints;
            }
        }

        foreach(GameObject carrot in livingCarrots)
        {
            if (carrot != null)
            {
                carrot.GetComponent<MoveTo>().waypoints = waypoints;
            } 
        }

        foreach(GameObject skelly in livingSkellys)
        {
            if (skelly != null)
            {
                skelly.GetComponent<MoveTo>().waypoints = waypoints;
            }
        }
    }
}
