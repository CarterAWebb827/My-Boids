using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Boid2D {
    public Vector2 position;
    public Vector2 velocity;
    public float angle;
}

public class Boid : MonoBehaviour {
    private int widthOfArea;
    private int heightOfArea;

    private float maxSpeed;
    private float desiredSeparation;

    public Boid2D boid;

    public Vector2 rule1Vector, rule2Vector, rule3Vector;
    private GameObject[] boidsPerceived;

    [Header("Rule 1")]
    private Vector2 centerOfMassPerceived;
    private int boidCountPerceived;
    private int percentageMovedToCenter;

    [Header("Rule 2")]
    private Vector2 separationVector;

    [Header("Rule 3")]
    private Vector2 averageVelocityPerceived;
    private int velocityMatched;

    private void Start() {
        widthOfArea = transform.parent.GetComponent<BoidManager>().width;
        heightOfArea = transform.parent.GetComponent<BoidManager>().height;

        maxSpeed = transform.parent.GetComponent<BoidManager>().maxSpeed;
        desiredSeparation = transform.parent.GetComponent<BoidManager>().desiredSeparation;

        boid.position = transform.position;
        boid.velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        boid.angle = Random.Range(0, 360);

        rule1Vector = Vector2.zero;
        rule2Vector = Vector2.zero;
        rule3Vector = Vector2.zero;

        boidsPerceived = transform.parent.GetComponent<BoidManager>().boids;

        centerOfMassPerceived = Vector2.zero;
        boidCountPerceived = 0;
        percentageMovedToCenter = transform.parent.GetComponent<BoidManager>().percentageMovedToCenter;

        separationVector = Vector2.zero;

        averageVelocityPerceived = Vector2.zero;
        velocityMatched = transform.parent.GetComponent<BoidManager>().velocityMatched;

        boid.velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

        transform.position = boid.position;
        transform.rotation = Quaternion.Euler(0, 0, boid.angle);
    }

    private void Update() {
        boidsPerceived = transform.parent.GetComponent<BoidManager>().boids;

        rule1Vector = Rule1();
        rule2Vector = Rule2();
        rule3Vector = Rule3();

        boid.velocity += rule1Vector + rule2Vector + rule3Vector;
        boid.velocity = Vector2.ClampMagnitude(boid.velocity, maxSpeed);
        boid.position += boid.velocity;
        //boid.position += boid.velocity * Time.deltaTime;
        
        // if the boid is out of the area, make it appear on the other side
        if (boid.position.x > widthOfArea) {
            boid.position.x = -widthOfArea;
        } else if (boid.position.x < -widthOfArea) {
            boid.position.x = widthOfArea;
        }

        if (boid.position.y > heightOfArea) {
            boid.position.y = -heightOfArea;
        } else if (boid.position.y < -heightOfArea) {
            boid.position.y = heightOfArea;
        }

        transform.position = boid.position;
        boid.angle = Mathf.Atan2(boid.velocity.y, boid.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, boid.angle);
    }

    /// <summary>
    /// Rule 1: Boids try to fly towards the center of mass of neighboring boids.
    /// </summary>
    /// <returns></returns>
    private Vector2 Rule1() {
        centerOfMassPerceived = Vector2.zero;
        boidCountPerceived = 0;

        foreach (GameObject boidPerceived in boidsPerceived) {
            if (boidPerceived != gameObject) {
                centerOfMassPerceived += (Vector2)boidPerceived.transform.position;
                boidCountPerceived++;
            }
        }

        if (boidCountPerceived > 0) {
            centerOfMassPerceived /= boidCountPerceived;
            return (centerOfMassPerceived - boid.position) / (float)percentageMovedToCenter;
        } else {
            return Vector2.zero;
        }
    }

    /// <summary>
    /// Rule 2: Boids try to keep a small distance away from other boids.
    /// </summary>
    /// <returns></returns>
    private Vector2 Rule2() {
        separationVector = Vector2.zero;

        foreach (GameObject boidPerceived in boidsPerceived) {
            if (boidPerceived != gameObject) {
                if (Vector2.Distance(boidPerceived.transform.position, transform.position) < desiredSeparation) {
                    separationVector -= (Vector2)boidPerceived.transform.position - boid.position;
                }
            }
        }

        return separationVector;
    }

    /// <summary>
    /// Rule 3: Boids try to match velocity with near boids.
    /// </summary>
    /// <returns></returns>
    private Vector2 Rule3() {
        averageVelocityPerceived = Vector2.zero;

        foreach (GameObject boidPerceived in boidsPerceived) {
            if (boidPerceived != gameObject) {
                averageVelocityPerceived += boidPerceived.GetComponent<Boid>().boid.velocity;
            }
        }

        averageVelocityPerceived /= boidCountPerceived;

        return (averageVelocityPerceived - boid.velocity) / (float)velocityMatched;
    }
}