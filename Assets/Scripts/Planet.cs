using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    private const float SHIP_SPAWN_SPEED = 0.3f;
    private const float COLONIZE_TIME = 5.0f;

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
    private SpriteRenderer spriteRenderer;
    private Coroutine moveShipsCoroutine;
    private TextMesh shipCountText;

    private Team currentUndisputedOccupant;
    private float currentUndisputedOccupantStartTime;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentUndisputedOccupant = owner;
        currentUndisputedOccupantStartTime = Time.fixedTime;
        ships = new Dictionary<Team, Queue<GameObject>>();
        shipTemplates = new Dictionary<Team, GameObject>();
        foreach (Team team in Team.GetValues(typeof(Team))) {
            ships.Add(team, new Queue<GameObject>());
            shipTemplates.Add(team, Resources.Load<GameObject>("Prefabs/Ship" + team.ToString()));
        }
        shipCountText = this.transform.Find("ShipCountText").GetComponent<TextMesh>();
        StartCoroutine(Combat());
        
    }

    void FixedUpdate()
    {
        CheckColonizeOrConquer();

        shipCountText.text = ships[owner].Count.ToString();
    }

    public void UpdatePlanet() {
        if (owner != Team.NEUTRAL && owner == currentUndisputedOccupant) {
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
                ship.StopMovingShip(this.gameObject);
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

    private IEnumerator Combat() {
        while (true) {
            // TODO: implement better combat mechanics
            List<KeyValuePair<Team, Queue<GameObject>>> combatantTeamShips = GetListOfNonEmptyTeamShips();
            if (combatantTeamShips.Count >= 2) {
                foreach(KeyValuePair<Team, Queue<GameObject>> teamShips in combatantTeamShips) {
                    teamShips.Value.Dequeue();
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    private void CheckColonizeOrConquer() {
        List<KeyValuePair<Team, Queue<GameObject>>> nonEmptyTeamShips = GetListOfNonEmptyTeamShips();
        if (nonEmptyTeamShips.Count == 1) {
            KeyValuePair<Team, Queue<GameObject>> teamShip = nonEmptyTeamShips[0];
            if (teamShip.Key != currentUndisputedOccupant) {
                currentUndisputedOccupant = teamShip.Key;
                currentUndisputedOccupantStartTime = Time.fixedTime;
            } else if (teamShip.Key != owner && Time.fixedTime > currentUndisputedOccupantStartTime + COLONIZE_TIME) {
                owner = teamShip.Key;
                Sprite newSprite = Resources.Load<Sprite>("Sprites/Planet" + owner.ToString());
                spriteRenderer.sprite = newSprite;
            }
        } else if (nonEmptyTeamShips.Count > 1) {
            currentUndisputedOccupant = Team.NEUTRAL;
            currentUndisputedOccupantStartTime = Time.fixedTime;
        }
    }

    private List<KeyValuePair<Team, Queue<GameObject>>> GetListOfNonEmptyTeamShips() {
        List<KeyValuePair<Team, Queue<GameObject>>> nonEmptyTeamShips = new List<KeyValuePair<Team, Queue<GameObject>>>();
        foreach(KeyValuePair<Team, Queue<GameObject>> teamShipPair in ships) {
            if (teamShipPair.Value.Count > 0) {
                nonEmptyTeamShips.Add(teamShipPair);
            }
        }
        return nonEmptyTeamShips;
    }

}
