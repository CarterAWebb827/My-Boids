using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BoidManager : MonoBehaviour {
    [Header("Boid Settings")]
    [SerializeField] public int boidCount;
    [SerializeField] public GameObject boidPrefab;

    [Header("Area Settings")]
    [SerializeField] public int width;
    [SerializeField] public int height;

    [Header("Camera Settings")]
    [SerializeField] public GameObject camera;

    [Header("Boid Characteristics")]
    [SerializeField] public float boidPerceptionRadius;
    [SerializeField] public float maxSpeed;
    [SerializeField] public float desiredSeparation;
    [SerializeField] public int percentageMovedToCenter;
    [SerializeField] public int velocityMatched;
    [SerializeField] public Material boidBaseMaterial;
    [SerializeField] public Material boidSearchingMaterial;
    [SerializeField] public Material boidFoundMaterial;
    public Vector2 windDirection;
    public GameObject[] boids;

    private int frameCounter = 0;
    [SerializeField] public int maxFrameCount = 100;

    private void Awake() {
        // Make the camera fit the screen
        camera.transform.position = new Vector3(0, 0, -10);
        camera.GetComponent<Camera>().orthographicSize = height;

        boids = new GameObject[boidCount];

        for (int i = 0; i < boidCount; i++) {
            GameObject boid = Instantiate(boidPrefab, new Vector3(Random.Range(-width, width), Random.Range(-height, height), 0), Quaternion.identity);
            boid.transform.parent = transform;

            boids[i] = boid;
        }

        windDirection = Random.insideUnitCircle.normalized;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(new Vector3(-width, -height), new Vector3(width, -height));
        Gizmos.DrawLine(new Vector3(width, -height), new Vector3(width, height));
        Gizmos.DrawLine(new Vector3(width, height), new Vector3(-width, height));
        Gizmos.DrawLine(new Vector3(-width, height), new Vector3(-width, -height));
    }

    private void FixedUpdate() {
        frameCounter++;

        if (frameCounter >= maxFrameCount) {
            windDirection = Random.insideUnitCircle.normalized;
            frameCounter = 0;
        }
    }
}
