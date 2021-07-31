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

    private Dictionary<Team, Queue<GameObject>> ships;
    private Dictionary<Team, GameObject> shipTemplates;
    private Sprite sprite;
    private Coroutine moveShipsCoroutine;
    private TextMesh shipCountText;

    void Start()
    {
        sprite = GetComponent<Sprite>();
        ships = new Dictionary<Team, Queue<GameObject>>();
        shipTemplates = new Dictionary<Team, GameObject>();
        foreach (Team team in Team.GetValues(typeof(Team))) {
            ships.Add(team, new Queue<GameObject>());
            shipTemplates.Add(team, Resources.Load<GameObject>("Prefabs/Ship" + team.ToString()));
        }
        shipCountText = this.transform.Find("ShipCountText").GetComponent<TextMesh>();
    }

    void Update()
    {
        // update UI
        shipCountText.text = ships[owner].Count.ToString();
    }

    public void UpdatePlanet() {
        if (owner != Team.NEUTRAL) {
            troopCount += troopGenerationRate;
            CreateShips();
        }
    }

    private void CreateShips() {
        for (int i = 0; i < shipGenerationRate; ++i) {
            GameObject shipGameObject = Ship.Instantiate(shipTemplates[owner], transform.position, Quaternion.identity);
            Ship ship = shipGameObject.GetComponent<Ship>();
            ship.InitializeShip(this.gameObject);
            ships[owner].Enqueue(shipGameObject);
        }
    }

    public void MoveShips(Team team, GameObject destinationPlanet) {
        InterruptShipMovement();
        moveShipsCoroutine = StartCoroutine(MoveShipsCoroutine(team, destinationPlanet));
    }

    private IEnumerator MoveShipsCoroutine(Team team, GameObject destinationPlanet) {
        while (ships[team].Count > 0) {
            GameObject shipGameObject = ships[team].Dequeue();
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
                ships[ship.owner].Enqueue(ship.gameObject);
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
