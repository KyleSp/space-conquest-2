using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
- Left click on planet to get stats about planet
- Right click and drag on planet to another planet to move ships
*/
public class Player : MonoBehaviour
{
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
            shipMovementSourcePlanet = getPlanetAtMouse();
            shipMovementSourcePlanet.GetComponent<Planet>().InterruptShipMovement();
        } else if (Input.GetMouseButtonUp(RIGHT_CLICK_BUTTON)) {
            if (draggingMouse) {
                GameObject shipMovementDestinationPlanet = getPlanetAtMouse();
                if (shipMovementSourcePlanet != null && shipMovementDestinationPlanet != null && shipMovementSourcePlanet != shipMovementDestinationPlanet) {
                    Planet sourcePlanet = shipMovementSourcePlanet.GetComponent<Planet>();
                    sourcePlanet.MoveShips(shipMovementDestinationPlanet);
                    shipMovementSourcePlanet = null;
                    draggingMouse = false;
                }
            }
        }
    }

    private GameObject getPlanetAtMouse() {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D raycastHit = Physics2D.Raycast(mousePosition, Vector2.zero);
        Collider2D collider = raycastHit.collider;
        if (collider != null && collider.tag == Tag.PLANET) {
            return collider.gameObject;
        }
        return null;
    }

}