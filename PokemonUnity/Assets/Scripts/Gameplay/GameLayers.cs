using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] public LayerMask solidObjectsLayer;
    [SerializeField] public LayerMask interactableLayer;
    [SerializeField] public LayerMask grassLayer;
    [SerializeField] public LayerMask playerLayer;
    [SerializeField] public LayerMask fovLayer;
    [SerializeField] public LayerMask portalLayer;
    [SerializeField] public LayerMask triggersLayer;
    [SerializeField] public LayerMask ledgeLayer;
    [SerializeField] public LayerMask waterLayer;

    public static GameLayers i { get; set; }
    private void Awake()
    {
        i = this;
    }

    public LayerMask SolidLayer
    {
        get => solidObjectsLayer;
    }

    public LayerMask InteractableLayer
    {
        get => interactableLayer;
    }

    public LayerMask GrassLayer
    {
        get => grassLayer;
    }

    public LayerMask PlayerLayer
    {
        get => playerLayer;
    }
    public LayerMask FovLayer
    {
        get => fovLayer;
    }

    public LayerMask PortalLayer
    {
        get => portalLayer;
    }

    public LayerMask LedgeLayer => ledgeLayer;

    public LayerMask WaterLayer => waterLayer;

    public LayerMask TriggerableLayers
    {
        get => grassLayer | fovLayer | portalLayer | triggersLayer | waterLayer;
    }
}
