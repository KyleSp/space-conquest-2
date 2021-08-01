using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    private const float AI_TIME_WAIT = 1f; // TODO: make this a random range of wait time

    public Team team;
    
    private bool isAlive = true;
    private bool isGameFinished = false;
    private GameObject[] planets;

    void Start()
    {
        planets = GameObject.FindGameObjectsWithTag("Planet");
        StartCoroutine(RunAI());
    }

    void Update()
    {

    }

    private IEnumerator RunAI() {
        while (isAlive && !isGameFinished) {
            Dictionary<Team, List<GameObject>> teamPlanets = SplitPlanetsByTeamOwner();
            // TODO: AI could have ships on planets it doesn't own
            if (!teamPlanets.ContainsKey(team) || teamPlanets[team].Count == 0) {
                isAlive = false;
                break;
            }

            List<Team> teams = new List<Team>();
            foreach (Team playerTeam in teams) {
                teams.Add(playerTeam);
            }

            Team targetTeam;
            int randInt = Random.Range(0, 100);
            if (randInt < 65 && teamPlanets.ContainsKey(Team.NEUTRAL) && teamPlanets[Team.NEUTRAL].Count > 0) {
                targetTeam = Team.NEUTRAL;
            } else {
                List<Team> otherPlayerTeams = GetOtherPlayerTeams();
                if (otherPlayerTeams.Count == 0) {
                    isGameFinished = true;
                    break;
                }
                targetTeam = ChooseRandomFromList(otherPlayerTeams);
            }

            MoveShipsToPlanet(teamPlanets, targetTeam);

            yield return new WaitForSeconds(AI_TIME_WAIT);
        }
    }

    private Dictionary<Team, List<GameObject>> SplitPlanetsByTeamOwner() {
        Dictionary<Team, List<GameObject>> teamPlanets = new Dictionary<Team, List<GameObject>>();
        foreach (GameObject planetGameObject in planets) {
            Planet planet = planetGameObject.GetComponent<Planet>();
            if (teamPlanets.ContainsKey(planet.owner)) {
                teamPlanets[planet.owner].Add(planetGameObject);
            } else {
                List<GameObject> planetsOwnedByTeam = new List<GameObject>();
                planetsOwnedByTeam.Add(planetGameObject);
                teamPlanets.Add(planet.owner, planetsOwnedByTeam);
            }
        }
        return teamPlanets;
    }

    private void MoveShipsToPlanet(Dictionary<Team, List<GameObject>> teamPlanets, Team targetTeam) {
        if (!teamPlanets.ContainsKey(team)) {
            isAlive = false;
            return;
        }
        GameObject sourcePlanetGameObject = ChooseRandomFromList(teamPlanets[team]);
        Planet sourcePlanet = sourcePlanetGameObject.GetComponent<Planet>();
        int sourcePlanetShipCount = sourcePlanet.GetPlanetOwnerShipCount();
        if (!teamPlanets.ContainsKey(targetTeam)) {
            return;
        }
        GameObject destinationPlanetGameObject = ChooseRandomFromList(teamPlanets[targetTeam]);
        StartCoroutine(MoveShipsWithCount(sourcePlanet, destinationPlanetGameObject, sourcePlanetShipCount));
    }

    private IEnumerator MoveShipsWithCount(Planet sourcePlanet, GameObject destinationPlanetGameObject, int count) {
        if (count > 0) {
            sourcePlanet.MoveShips(team, destinationPlanetGameObject);
            yield return new WaitForSeconds(count * Planet.SHIP_SPAWN_SPEED);
            sourcePlanet.InterruptShipMovement();
        }
    }

    private List<Team> GetOtherPlayerTeams() {
        List<Team> otherPlayerTeams = new List<Team>();
        foreach (Team playerTeam in Team.GetValues(typeof(Team))) {
            if (playerTeam != team && playerTeam != Team.NEUTRAL) {
                otherPlayerTeams.Add(playerTeam);
            }
        }
        return otherPlayerTeams;
    }

    private T ChooseRandomFromList<T>(List<T> list) {
        if (list.Count == 0) {
            throw new System.Exception("Can't choose random index from empty list.");
        }
        int randomInt = Random.Range(0, list.Count);
        return list[randomInt];
    }

}
