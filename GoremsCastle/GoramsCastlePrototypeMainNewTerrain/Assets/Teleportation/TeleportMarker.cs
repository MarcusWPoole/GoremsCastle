using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportMarker : MonoBehaviour
{
    [SerializeField]
    private PositionSystem _positionSystem;

    [SerializeField]
    private bool _enabled = true;

    [SerializeField]
    private bool _stationaryPosition = false;
    //private Vector3 _preStationaryLocalOffset;

    [SerializeField]
    public float _scale = 1.0f;

    //[SerializeField]
    //private Vector3 _size = new Vector3(1f, 0.01f, 1f);

    public bool IsEnabled()
    {
        return _enabled;
    }

    public void SetEnabled(bool enabled)
    {
        _enabled = enabled;
    }

    public float Scale()
    {
        return _scale;
    }

    public bool IsStationaryPosition()
    {
        return _stationaryPosition;
    }

    /*public Vector3 GetPreStationLocalOffset()
    {
        return _preStationaryLocalOffset;
    }*/

    private void SetPositionAndRotation(GameObject player, TeleportMarker currentTeleportMaker, Vector3 localPosition, Quaternion localRotation)
    {
        player.transform.SetParent(transform);
        player.transform.localPosition = localPosition * (_scale / currentTeleportMaker.Scale());
        player.transform.localRotation = localRotation;
    }

    public void Teleport(GameObject player, TeleportMarker currentTeleportMaker)
    {
        /*Vector3 newPosition = transform.position;// + Vector3.zero;
        player.transform.position = newPosition;*/

        /*Vector3 localPosition;
        Quaternion localRotation;
        player.transform.GetLocalPositionAndRotation(out localPosition, out localRotation);
        player.transform.SetParent(transform);
        player.transform.SetLocalPositionAndRotation(localPosition, localRotation);*/

        /*Vector3 oldPosition = currentTeleportMaker.transform.InverseTransformVector(player.transform.position);
        Vector3 newPosition = transform.TransformVector(oldPosition);

        Vector3 oldDirection = currentTeleportMaker.transform.InverseTransformVector(player.transform.forward);
        Vector3 newDirection = transform.TransformVector(oldDirection);

        player.transform.SetPositionAndRotation(newPosition, newDirection);*/

        /*if(IsStationaryPosition())
        {
            _preStationaryLocalOffset = player.transform.localPosition;
            SetPositionAndRotation(player, currentTeleportMaker, Vector3.zero, player.transform.localRotation);
        }
        else if(currentTeleportMaker.IsStationaryPosition())
        {
            Vector3 newLocalPosition = player.transform.localPosition - currentTeleportMaker.GetPreStationLocalOffset();
            SetPositionAndRotation(player, currentTeleportMaker, newLocalPosition, player.transform.localRotation);
        }
        else
        {*/
        ///SetPositionAndRotation(player, currentTeleportMaker, player.transform.localPosition, player.transform.localRotation);
        //}

        //playspace.transform.SetParent(transform);
        //playspace.transform.localPosition = Vector3.zero;
        //playspace.transform.rotation = transform.rotation;

        if (IsStationaryPosition())
        {
            //                     XR Origin        Camera Offset         Main Camera
            Vector3 cameraOffset = player.transform.GetChild(0).transform.GetChild(0).transform.localPosition;
            cameraOffset.y = 0;
            SetPositionAndRotation(player, currentTeleportMaker, -cameraOffset, player.transform.localRotation);
        }
        else
        {
            SetPositionAndRotation(player, currentTeleportMaker, Vector3.zero, player.transform.localRotation);
        }

        player.transform.localScale = _positionSystem.GetOriginalPlayerScale() * _scale;
    }

    public Vector3 GetLocalOffset(GameObject player)
    {
        return Vector3.zero;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDrawGizmos()
    {
        if (_positionSystem._drawAllGizmos)
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
        Gizmos.DrawCube(Vector3.zero, _positionSystem._playspaceSize * _scale);

        /*Gizmos.color = new Color(0, 0, 0, 1f);
        Gizmos.DrawFrustum(transform.position + (Vector3.up*1.75f*_scale), 90f, 0.5f, 5f, 1f);*/

        Gizmos.color = new Color(1f, 0f, 0f, 1f);
        //Gizmos.DrawLine(transform.position, transform.position+transform.forward);
        Gizmos.DrawLine(Vector3.zero, Vector3.forward * _scale);
    }
}
