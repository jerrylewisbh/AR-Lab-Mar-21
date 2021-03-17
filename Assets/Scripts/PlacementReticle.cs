using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlacementReticle : MonoBehaviour
{

    #region Variables
    [Header("Scene References")]
    [SerializeField]
    [Tooltip("Reference to the plane manager in the scene")]
    private ARPlaneManager planeManager;

    [SerializeField]
    [Tooltip("Reference to the ray cast manager in the scene")]
    private ARRaycastManager rayCastManager;

    [SerializeField]
    [Tooltip("Prefab with the visual reticle")]
    private GameObject reticlePrefab;

    [SerializeField]
    [Tooltip("Reference to the AR Camera Transform")]
    private Transform cameraTransform;

    //Reference to the instantiated reticle
    private GameObject reticle;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    #endregion

    #region Unity Functions
    public void Awake()
    {

        reticle = Instantiate(reticlePrefab);
        planeManager.planesChanged += PlanesChanged;
    }


    // Update is called once per frame
    void Update()
    {
        Vector2 screenCenter = ScreenUtils.GetScreenCenter();

        if (rayCastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
        {
            //Repositions the reticle
            RepositionReticle();
        }
    }

    #endregion

    public FurnitureConfig selectedFurniture;
    private void PlanesChanged(ARPlanesChangedEventArgs arg)
    {

    }

    private void RepositionReticle()
    {
        Pose pose = hits[0].pose;

        Vector3  normal = pose.rotation * Vector3.up;

        Vector3 userVector = cameraTransform.position -  pose.position;

        if(Vector3.Dot(userVector, normal) >= 0)
        {
            reticle.transform.SetPositionAndRotation(pose.position, pose.rotation);
        }
    }


    public void PlaceFurnitureOnPlane(ARPlane plane)
    {
        if (selectedFurniture.canBePlaced(plane) && selectedFurniture.fitsOnPlane(plane)){

            Instantiate(selectedFurniture.furniturePrefab, reticle.transform);

        }
        else
        {
            Debug.Log("This piece of furniture doesn't fit the given plane");
        }
    }
}
