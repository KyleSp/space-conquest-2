using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
- Left click on planet to get stats about planet
- Right click and drag on planet to another planet to move ships
*/
public class Player : MonoBehaviour
{
    public Team team;
    private const int RIGHT_CLICK_BUTTON = 1;

    private bool draggingMouse = false;
    private GameObject shipMovementSourcePlanet;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(RIGHT_CLICK_BUTTON)) {
            draggingMouse = true;
            shipMovementSourcePlanet = GetPlanetAtMouse();
            if (shipMovementSourcePlanet != null) {
                shipMovementSourcePlanet.GetComponent<Planet>().InterruptShipMovement();
            }
        } else if (Input.GetMouseButtonUp(RIGHT_CLICK_BUTTON)) {
            if (draggingMouse) {
                GameObject shipMovementDestinationPlanet = GetPlanetAtMouse();
                if (shipMovementSourcePlanet != null && shipMovementDestinationPlanet != null && shipMovementSourcePlanet != shipMovementDestinationPlanet) {
                    Planet sourcePlanet = shipMovementSourcePlanet.GetComponent<Planet>();
                    sourcePlanet.MoveShips(team, shipMovementDestinationPlanet);
                    shipMovementSourcePlanet = null;
                    draggingMouse = false;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            team = Team.BLUE;
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            team = Team.RED;
        } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            team = Team.GREEN;
        }
    }

    private GameObject GetPlanetAtMouse() {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D raycastHit = Physics2D.Raycast(mousePosition, Vector2.zero);
        Collider2D collider = raycastHit.collider;
        if (collider != null && collider.tag == Tag.PLANET) {
            return collider.gameObject;
        }
        return null;
    }

}
