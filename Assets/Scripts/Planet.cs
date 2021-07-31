using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    private const float SHIP_SPAWN_SPEED = 0.3f;

    public Team owner;
    public float troopCapacity;
    public float troopCount;
    public float troopGenerationRate;
    // public float populationCapacity;
    // public float population;
    // public float populationGenerationRate;
    public float shipGenerationRate;

    public GameObject shipTemplate;

    private Queue<GameObject> ships;
    private Sprite sprite;
    private Coroutine moveShipsCoroutine;
    private TextMesh shipCountText;

    void Start()
    {
        sprite = GetComponent<Sprite>();
        ships = new Queue<GameObject>();
        shipCountText = this.transform.Find("ShipCountText").GetComponent<TextMesh>();
    }

    void Update()
    {
        // update UI
        shipCountText.text = ships.Count.ToString();
    }

    public void UpdatePlanet() {
        if (owner != Team.NONE) {
            troopCount += troopGenerationRate;
            CreateShips();
        }
    }

    private void CreateShips() {
        for (int i = 0; i < shipGenerationRate; ++i) {
            GameObject shipGameObject = Ship.Instantiate(shipTemplate, transform.position, Quaternion.identity);
            Ship ship = shipGameObject.GetComponent<Ship>();
            ship.InitializeShip(this.gameObject);
            ships.Enqueue(shipGameObject);
        }
    }

    public void MoveShips(GameObject destinationPlanet) {
        InterruptShipMovement();
        moveShipsCoroutine = StartCoroutine(MoveShipsCoroutine(destinationPlanet));
    }

    private IEnumerator MoveShipsCoroutine(GameObject destinationPlanet) {
        while (ships.Count > 0) {
            GameObject shipGameObject = ships.Dequeue();
            Ship ship = shipGameObject.GetComponent<Ship>();
            ship.MoveShipBetweenPlanets(this.gameObject, destinationPlanet);
            yield return new WaitForSeconds(SHIP_SPAWN_SPEED);
        }
        moveShipsCoroutine = null;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == Tag.SHIP) {
            Ship ship = other.GetComponent<Ship>();
            if (ship.GetLastPlanetVisited() != this.gameObject) {
                ship.stopMovingShip(this.gameObject);
                ships.Enqueue(ship.gameObject);
            }
        }
    }

    public void InterruptShipMovement() {
        if (moveShipsCoroutine != null) {
            StopCoroutine(moveShipsCoroutine);
            moveShipsCoroutine = null;
        }
    }

}
