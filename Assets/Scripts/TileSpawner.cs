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
        SpawnTile(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>());
    }

    private void SpawnTile(Tile tile, bool spawnObstacles = false)
    {
        prevTile = GameObject.Instantiate(tile.gameObject, currentTileLocation, Quaternion.identity);
        currentTiles.Add(prevTile);
        // Vector3.Scale multiplies two vectors element-wise
        currentTileLocation += Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size, currentTileDirection);
        // Example: (3,4,5) * (0,0,1) => (0,0,5)
    }

    private GameObject SelectRandomGameObjectFromList(List<GameObject> list)
    {
        if(list.Count == 0) return null;

        return list[Random.Range(0, list.Count)];
    }
}

}