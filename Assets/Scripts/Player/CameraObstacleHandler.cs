using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraObstacleHandler : MonoBehaviour
{
    public Camera cam;
    public GameObject player;
    public float rayCastDistance = 3f;
    public LayerMask mask;

    [Header("Obstructed Settings")]
    public Vector3 offsetWhenObstructed = new Vector3(0f, -10f, -10f);
    public Vector3 rotationWhenObstructed = new Vector3(20f, 0f, 0f);
    public float transitionSpeed = 5f;

    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;

    [Header("Raycast Origin")]
    public Transform rayOrigin; // Punto desde donde lanzar el raycast (estático)

    void Start()
    {
        if (cam == null)
            cam = Camera.main;

        if (rayOrigin == null)
            rayOrigin = transform; // Usa este mismo objeto si no se asigna otro

        originalLocalPosition = cam.transform.localPosition;
        originalLocalRotation = cam.transform.localRotation;
    }

    void Update()
    {
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
        bool hit = Physics.Raycast(ray, rayCastDistance, mask);

        Debug.DrawRay(ray.origin, ray.direction * rayCastDistance, Color.red);

        Vector3 targetPosition = originalLocalPosition;
        Quaternion targetRotation = originalLocalRotation;

        if (hit)
        {
            targetPosition = originalLocalPosition + offsetWhenObstructed;
            targetRotation = originalLocalRotation * Quaternion.Euler(rotationWhenObstructed);
        }

        cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, targetPosition, Time.deltaTime * transitionSpeed);
        cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, targetRotation, Time.deltaTime * transitionSpeed);

        //player.transform.localRotation = Quaternion.Lerp(player.transform.localRotation, targetRotation, Time.deltaTime * transitionSpeed);
    }
}