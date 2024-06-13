using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : NetworkManager
{
    // You can define a list of spawn positions or just use a single one for simplicity.
    // You can define a list of spawn positions or just use a single one for simplicity.
    public Transform[] spawnPositions;

    private int nextSpawnIndex = 0;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        // Determine the spawn position. Here we're just picking the next one in the array for simplicity.
        Transform startPos = spawnPositions[nextSpawnIndex];

        // Instantiate the player object at the chosen spawn position
        GameObject player = Instantiate(playerPrefab, startPos.position, startPos.rotation);

        // Add the player object to the network
        NetworkServer.AddPlayerForConnection(conn, player);

        // Update the next spawn index to cycle through the array
        nextSpawnIndex = (nextSpawnIndex + 1) % spawnPositions.Length;
    }
}