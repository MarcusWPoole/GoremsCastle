using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideShowMap : MonoBehaviour
{
    [SerializeField] private GameObject map;
    private MeshRenderer mapRenderer;

    void Start()
    {
        mapRenderer = map.GetComponent<MeshRenderer>();
    }

    public void HideShow()
    {
        map.SetActive(!map.activeSelf);
    }
}
