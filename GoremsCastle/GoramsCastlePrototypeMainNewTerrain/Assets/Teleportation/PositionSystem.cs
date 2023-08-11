using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class PositionSystem : MonoBehaviour
{
    [Header("Player")]

    [SerializeField]
    private GameObject _player;
    
    private Vector3 _scaleOriginal;

    //[SerializeField] private GameObject _playspace;

    [Header("Map")]

    [SerializeField]
    public GameObject _map;
    [SerializeField]
    private Transform _leftHand;
    [SerializeField]
    private Transform _rightHand;
    [SerializeField]
    private Transform _activeHand;

    [Header("Gizmos")]

    [SerializeField]
    public bool _drawAllGizmos = false;

    [Header("Markers")]

    [SerializeField]
    private List<TeleportMarker> _teleportMakers;

    [SerializeField]
    private TeleportMarker _startingTeleportMaker;

    public TeleportMarker _currentTeleportMarker;

    [Header("Playspace")]

    [SerializeField]
    public Vector3 _playspaceSize = new Vector3(2.5f, 0.01f, 2.5f);

    // Start is called before the first frame update
    void Start()
    {
        //save orginal player size
        _scaleOriginal = _player.transform.localScale;

        //_player.transform.SetPositionAndRotation(_startingTeleportMaker.transform.position, _startingTeleportMaker.transform.rotation);
        _player.transform.localPosition = _startingTeleportMaker.transform.position;
        _player.transform.localRotation = _startingTeleportMaker.transform.rotation;
        _player.transform.SetParent(_startingTeleportMaker.transform);

        _startingTeleportMaker.Teleport(_player, _startingTeleportMaker);
        _currentTeleportMarker = _startingTeleportMaker;

        /*_playspace.transform.localPosition = _startingTeleportMaker.transform.position;
        _playspace.transform.localRotation = _startingTeleportMaker.transform.rotation;

        _playspace.transform.SetParent(_startingTeleportMaker.transform);
        _scaleOriginal = _player.transform.localScale;

        _startingTeleportMaker.Teleport(_playspace, _player, _startingTeleportMaker);
        _currentTeleportMarker = _startingTeleportMaker;*/


        foreach (GameObject point in _teleportSelectPoints)
        {
            _teleporterMarkers.Add(point.GetComponent<MapMarker>());
            _teleporterRenderers.Add(point.GetComponent<MeshRenderer>());
        }

        //_activeHand = _rightHand;
        _map.transform.SetParent((_activeHand == _rightHand) ? _leftHand : _rightHand);
    }

    //teleports the position systems player to the specified teleport marker
    public TeleportMarker TeleportToMarker(TeleportMarker teleportMarker)
    {
        if (!teleportMarker.IsEnabled())
        {
            Assert.IsTrue(teleportMarker.IsEnabled(), "[Bug] You've attempted to teleport to a marker that's been disabled.");
            return _currentTeleportMarker;
        }

        if (teleportMarker == _currentTeleportMarker)
        {
            return _currentTeleportMarker;
        }

        teleportMarker.Teleport(_player, _currentTeleportMarker);
        _currentTeleportMarker = teleportMarker;

        return teleportMarker;
    }

    //gets a list of the currently enabled teleport markers
    public List<TeleportMarker> GetEnabledTeleportMarkers()
    {
        List<TeleportMarker> enabledMarkers = new List<TeleportMarker>();

        foreach (TeleportMarker teleportMarker in _teleportMakers)
        {
            if (teleportMarker.IsEnabled())
            {
                enabledMarkers.Add(teleportMarker);
            }
        }

        return enabledMarkers;
    }

    public List<TeleportMarker> GetTeleportMarkers()
    {
        return _teleportMakers;
    }
    internal TeleportMarker GetCurrentMarker()
    {
        return _currentTeleportMarker;
    }

    public Vector3 GetOriginalPlayerScale()
    {
        return _scaleOriginal;
    }

    //map stuff

    [SerializeField] private List<GameObject> _teleportSelectPoints;
    [HideInInspector] public List<MapMarker> _teleporterMarkers;
    [HideInInspector] public List<MeshRenderer> _teleporterRenderers;
    private MeshRenderer rendererToBeEnabled;
    private TeleportMarker _newDestination;

    //[SerializeField]
    //private float _handMinDistance = 0.25f;

    void TeleportHighlight(Transform hand)
    {
        //MeshRenderer rendererToBeEnabled;

        float measuredDistance;
        float currentMinDistance = 10000.0f;

        for (int i = 0; i < _teleportSelectPoints.Count; i++)
        {
            if(!_teleportSelectPoints[i].activeSelf || !_teleporterMarkers[i].teleportDestination.IsEnabled())
                continue;

            _teleporterRenderers[i].enabled = false;
            measuredDistance = Vector3.Distance(_teleportSelectPoints[i].transform.position, hand.position);
            //if (measuredDistance < _handMinDistance && measuredDistance < currentMinDistance)
            if (measuredDistance < currentMinDistance)
            {
                currentMinDistance = measuredDistance;
                _newDestination = _teleporterMarkers[i].teleportDestination;
                rendererToBeEnabled = _teleporterRenderers[i];
            }
        }

        if(rendererToBeEnabled)
            rendererToBeEnabled.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_map.activeSelf)
        {
            TeleportHighlight(_activeHand);
        }
    }

    //returns false if marker doesn't belong to this position system
    public bool SetEnabledTeleportMarker(TeleportMarker teleportMarker, bool enabled)
    {
        if (_teleportMakers.Contains(teleportMarker))
        {
            teleportMarker.SetEnabled(enabled);
            return true;
        }

        return false;
    }

    public void TeleportCallback(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log(context);
            if (_map.activeSelf && _newDestination.IsEnabled())
            {
                TeleportToMarker(_newDestination);
            }
        }
    }

    public void MapLeftCallback(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            //Debug.Log(context);
            //_map.SetActive(!_map.activeSelf);
            if (_map.activeSelf)
            {
                _map.SetActive(false);
            }
            else
            {
                _map.SetActive(true);
                _map.transform.SetParent(_leftHand.transform);
                _map.transform.localPosition = new Vector3(0.1338392f, 0.1328f, 0.1491596f);
                _map.transform.localRotation = Quaternion.Euler(-23.575f, 197.087f, -27.854f);
                _activeHand = _rightHand.transform;
            }
        }
    }

    public void MapRightCallback(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            //Debug.Log(context);
            //_map.SetActive(!_map.activeSelf);
            if (_map.activeSelf)
            {
                _map.SetActive(false);
            }
            else
            {
                _map.SetActive(true);
                _map.transform.SetParent(_rightHand.transform);
                _map.transform.localPosition = new Vector3(-0.1338392f, 0.1328f, 0.1491596f);
                _map.transform.localRotation = Quaternion.Euler(-23.575f, 163.0f, 27.854f);
                _activeHand = _leftHand.transform;
            }
        }

    }

    //Gizmo code

    /*void OnDrawGizmos()
    {
        if(_drawAllGizmos)
            DrawDebug();
    }

    void OnDrawGizmosSelected()
    {
        DrawDebug();
    }

    private void DrawDebug()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        
        Gizmos.color = _enabled ? new Color(0f, 0f, 1f, 0.1f) : new Color(1f, 0f, 0f, 0.1f);
        //Gizmos.DrawCube(transform.position, _size * _scale);
        Gizmos.DrawCube(Vector3.zero, _size * _scale);

        //Gizmos.color = new Color(0, 0, 0, 1f);
        //Gizmos.DrawFrustum(transform.position + (Vector3.up*1.75f*_scale), 90f, 0.5f, 5f, 1f);

        Gizmos.color = new Color(1f, 0f, 0f, 1f);
        //Gizmos.DrawLine(transform.position, transform.position+transform.forward);
        Gizmos.DrawLine(Vector3.zero, Vector3.forward*_scale);
    }*/
}
