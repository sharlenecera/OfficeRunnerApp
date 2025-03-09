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
    private int maximumStraightTiles = 15; // max number of straight tiles before a turn tile
    [SerializeField]
    private GameObject startingTile;
    [SerializeField]
    private List<GameObject> turnTiles;
    [SerializeField]
    private List<GameObject> obstacles;

    private Vector3 currentTileLocation = Vector3.zero;
    private Vector3 currentTileDirection = Vector3.forward; // original moving direction
    private GameObject prevTile; // keep track for where to place next tile

    private List<GameObject> currentTiles;
    private List<GameObject> currentObstacles;

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
        // SpawnTile(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>());
        SpawnTile(turnTiles[0].GetComponent<Tile>());
        AddNewDirection(Vector3.left);
    }

    private void SpawnTile(Tile tile, bool spawnObstacles = false)
    {
        // Quaternions represent rotations in Unity
        Quaternion newTileRotation = tile.gameObject.transform.rotation * Quaternion.LookRotation
            (currentTileDirection, Vector3.up);

        prevTile = GameObject.Instantiate(tile.gameObject, currentTileLocation, newTileRotation);
        currentTiles.Add(prevTile);
        if(tile.type == TileType.STRAIGHT)
        {
            // Vector3.Scale multiplies two vectors element-wise
            currentTileLocation += Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size, currentTileDirection);
            // Example: (3,4,5) * (0,0,1) => (0,0,5)
        }
    }

    private void DeletePreviousTiles()
    {
        
    }

    public void AddNewDirection(Vector3 direction){
        currentTileDirection = direction;
        DeletePreviousTiles(); 
        // to improve this ^, use object tooling so you don't need to create and delete tiles lots of times.
        
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

        currentTileLocation += tilePlacementScale;

        // Now spawn the rest of the tiles after this turn tile

        int currentPathLength = Random.Range(minimumStraightTiles, maximumStraightTiles);
        for (int i=0 ; i<currentPathLength ; i++)
        {
            SpawnTile(startingTile.GetComponent<Tile>(), (i == 0) ? false : true); // do not spawn obstacle on first tile
        }

        SpawnTile(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>(), false);
    }

    private GameObject SelectRandomGameObjectFromList(List<GameObject> list)
    {
        if(list.Count == 0) return null;

        return list[Random.Range(0, list.Count)];
    }
}

}