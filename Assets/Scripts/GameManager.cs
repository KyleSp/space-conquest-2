using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const float UPDATE_PLANETS_TIME = 2.0f;

    private List<GameObject> planets;
    private bool isGameFinished = false;

    void Start()
    {
        planets = new List<GameObject>(GameObject.FindGameObjectsWithTag("Planet"));
        StartCoroutine(UpdatePlanets());
    }

    void Update()
    {
        if (!isGameFinished) {
            CheckGameFinished();
        }
    }

    private IEnumerator UpdatePlanets() {
        while (!isGameFinished) {
            yield return new WaitForSeconds(UPDATE_PLANETS_TIME);
            foreach (GameObject planetGameObject in planets) {
                planetGameObject.GetComponent<Planet>().UpdatePlanet();
            }
        }
    }

    private void CheckGameFinished() {
        HashSet<Team> teamsAlive = new HashSet<Team>();
        foreach(GameObject planetGameObject in planets) {
            Planet planet = planetGameObject.GetComponent<Planet>();
            teamsAlive.Add(planet.owner);
            List<KeyValuePair<Team, Queue<GameObject>>> nonEmptyTeamShips = planet.GetNonEmptyTeamShips();
            foreach (KeyValuePair<Team, Queue<GameObject>> teamShips in nonEmptyTeamShips) {
                if (teamShips.Value.Count > 0) {
                    teamsAlive.Add(teamShips.Key);
                }
            }
        }
        GameObject[] shipsInSpace = GameObject.FindGameObjectsWithTag(Tag.SHIP);
        foreach(GameObject shipGameObject in shipsInSpace) {
            Ship ship = shipGameObject.GetComponent<Ship>();
            if (ship.isActiveAndEnabled) { // TODO: can we combine this with nonEmptyTeamShips?
                teamsAlive.Add(ship.owner);
            }
        }

        teamsAlive.Remove(Team.NEUTRAL);
        if (teamsAlive.Count == 1) {
            Team winner = teamsAlive.GetEnumerator().Current;
            isGameFinished = true;
            Time.timeScale = 0f;
            Debug.Log("WINNER: " + winner);
        }
    }

}
