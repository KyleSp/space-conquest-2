using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public Team owner;
    public float attack;
    public float defense;
    public float health;
    public float maxHealth;
    public float shipSpeed;

    public Rigidbody2D rb;
    public GameObject currentPlanet;
    private GameObject lastPlanetVisited;

    private bool moving;
    private GameObject destinationPlanet;

    public void InitializeShip(GameObject sourcePlanet) {
        currentPlanet = sourcePlanet;
        lastPlanetVisited = sourcePlanet;
        gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        if (moving) {
            Vector2 vectorToTarget = destinationPlanet.transform.position - transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.position = Vector2.MoveTowards(rb.position, destinationPlanet.transform.position, shipSpeed);
        }
    }

    public void MoveShipBetweenPlanets(GameObject sourcePlanet, GameObject destinationPlanet) {
        rb.position = sourcePlanet.transform.position;
        gameObject.SetActive(true);
        this.destinationPlanet = destinationPlanet;
        moving = true;
        lastPlanetVisited = currentPlanet;
        currentPlanet = null;
    }

    public void stopMovingShip(GameObject destinationPlanet) {
        moving = false;
        currentPlanet = destinationPlanet;
        gameObject.SetActive(false);
    }

    public GameObject GetLastPlanetVisited() {
        return lastPlanetVisited;
    }

}
