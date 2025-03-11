using System.Collections.Generic;
using UnityEngine;

namespace RunnerGame {
public class TileSpawner : MonoBehaviour
{
    [SerializeField]
    private int tileStartCount = 10; // how many straight tiles will spawn at the start
    [SerializeField]
    private int minimumStraightTiles = 3; // min number of straight tiles before a turn tile
    [SerializeField]
    private int maximumStraightTiles = 8; // max number of straight tiles before a turn tile
    [SerializeField]
    private GameObject startingTile;
    [SerializeField]
    private List<GameObject> turnTiles;
    [SerializeField]
    private List<GameObject> obstacles;
    [SerializeField]
    private GameObject door;
    [SerializeField]
    private int doorAppearsAfter = 1; // door appears after 5 turns

    private Vector3 currentTileLocation = Vector3.zero;
    private Vector3 currentTileDirection = Vector3.forward; // original moving direction
    private GameObject prevTile; // keep track for where to place next tile

    private List<GameObject> currentTiles;
    private List<GameObject> currentObstacles;
    private float obstacleSpawnChance = 0.4f; // 20% chance of spawning an obstacle

    private void Start()
    {
        // Initialize the lists
        currentTiles = new List<GameObject>();
        currentObstacles = new List<GameObject>();

        Random.InitState(System.DateTime.Now.Millisecond); // Set the seed so numbers are random

        // Spawn the starting straight tiles
        for (int i=0 ; i<tileStartCount ; i++)
        {
            SpawnTile(startingTile.GetComponent<Tile>());
        }

        // Spawn a turn tile
        SpawnTile(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>());
    }

    private void SpawnTile(Tile tile, bool spawnObstacle = false, bool spawnDoor = false)
    {
        // Quaternions represent rotations in Unity
        Quaternion newTileRotation = tile.gameObject.transform.rotation * Quaternion.LookRotation
            (currentTileDirection, Vector3.up);

        prevTile = GameObject.Instantiate(tile.gameObject, currentTileLocation, newTileRotation);
        currentTiles.Add(prevTile);

        // Adding door spawning functionality
        if (spawnDoor) SpawnDoor();

        if (spawnObstacle) SpawnObstacle();

        if(tile.type == TileType.STRAIGHT) // Adds offset for next tile location to be spawned
        {
            // Vector3.Scale multiplies two vectors element-wise
            currentTileLocation += Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size, currentTileDirection);
            // Example: (3,4,5) * (0,0,1) => (0,0,5)
        }
    }

    private void DeletePreviousTiles()
    {
        // Remove all tiles but one
        while(currentTiles.Count != 1){ // need to keep the last tile as player is on it while turning
            GameObject tile = currentTiles[0];
            currentTiles.RemoveAt(0);
            Destroy(tile);
        }
        
        // Remove all obstacles
        while(currentObstacles.Count != 0){
            GameObject obstacle = currentObstacles[0];
            currentObstacles.RemoveAt(0);
            Destroy(obstacle);
        }
    }

    public void AddNewDirection(Vector3 direction){
        currentTileDirection = direction;
        DeletePreviousTiles(); 
        // to improve this ^, use object tooling so you don't need to create and delete tiles lots of times.
        
        // Determine placement of next tile using calculations
        Vector3 tilePlacementScale;
        if (prevTile.GetComponent<Tile>().type == TileType.SIDEWAYS)
        {
            tilePlacementScale = Vector3.Scale((prevTile.GetComponent<Renderer>().bounds.size / 2) +
            (Vector3.one * startingTile.GetComponent<BoxCollider>().size.z / 2), currentTileDirection);
            // z is the defined direction which is forward on the straight tile
        }
        else
        {
            // left or right tiles
            tilePlacementScale = Vector3.Scale((prevTile.GetComponent<Renderer>().bounds.size - (Vector3.one * 2)) +
            (Vector3.one * startingTile.GetComponent<BoxCollider>().size.z / 2), currentTileDirection);
            // z is the defined direction which is forward on the straight tile
        }

        currentTileLocation += tilePlacementScale; // Change tile location to new location

        // Now spawn a random amount of straight tiles after this turn tile if door is not spawning
        if (doorAppearsAfter != 0)
        {
            int currentPathLength = Random.Range(minimumStraightTiles, maximumStraightTiles);
            for (int i=0 ; i<currentPathLength ; i++)
            {
                SpawnTile(startingTile.GetComponent<Tile>(), (i == 0) ? false : true,false);
                // do not spawn obstacle on first tile
            }
        }
        else // spawn door if doorAppearsAfter is == 0
        {
            SpawnTile(startingTile.GetComponent<Tile>(), false, true);
        }

        // Spawn a random turn tile
        SpawnTile(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>(), false);
        doorAppearsAfter--;
    }

    private void SpawnObstacle()
    {
        if (Random.value > obstacleSpawnChance) return;

        GameObject obstaclePrefab = SelectRandomGameObjectFromList(obstacles);
        Quaternion newObjectRotation = obstaclePrefab.gameObject.transform.rotation * Quaternion.LookRotation
            (currentTileDirection, Vector3.up);

        GameObject obstacle = Instantiate(obstaclePrefab, currentTileLocation, newObjectRotation);
        currentObstacles.Add(obstacle);

    }

    private void SpawnDoor()
    {
        Quaternion newObjectRotation = door.gameObject.transform.rotation * Quaternion.LookRotation
            (currentTileDirection, Vector3.up);

        GameObject doorToSpawn = Instantiate(door, currentTileLocation, newObjectRotation);
        // Adjust the position to make the door touch the tile floor
        Vector3 doorPosition = doorToSpawn.transform.position;
        doorPosition.y -= 2f;
        // doorPosition.y += doorToSpawn.GetComponent<Renderer>().bounds.extents.y; // Adjust based on the door's height
        doorToSpawn.transform.position = doorPosition;
    }

    private GameObject SelectRandomGameObjectFromList(List<GameObject> list)
    {
        if(list.Count == 0) return null;

        return list[Random.Range(0, list.Count)];
    }
}

}