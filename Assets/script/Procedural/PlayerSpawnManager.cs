using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    public Terrain terrain;  // Référence au terrain
    public GameObject playerPrefab;  // Référence au prefab du joueur

    private GameObject player;  // Référence au joueur instancié

    void Start()
    {
        // Assurer que le joueur est bien instancié avant de commencer
        SpawnPlayerAtRandomPosition();
    }

    void SpawnPlayerAtRandomPosition()
    {
        if (terrain == null || playerPrefab == null)
        {
            Debug.LogError("Terrain ou PlayerPrefab non assigné.");
            return;
        }

        // Générer une position aléatoire sur le terrain
        TerrainData terrainData = terrain.terrainData;
        float x = Random.Range(0f, 1f) * terrainData.size.x;
        float z = Random.Range(0f, 1f) * terrainData.size.z;
        float y = terrain.SampleHeight(new Vector3(x, 0, z)) + 1f; // Ajuste légèrement la hauteur

        Vector3 spawnPosition = new Vector3(x, y, z);

        // Instancier le joueur à la position aléatoire
        player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

        // Activer la caméra du joueur
        Camera playerCamera = player.GetComponentInChildren<Camera>();
        if (playerCamera != null)
        {
            playerCamera.enabled = true;
        }
        else
        {
            Debug.LogError("Aucune caméra trouvée sur le Player.");
        }

        // Initialiser ou configurer d'autres composants du joueur si nécessaire
        player.transform.rotation = Quaternion.identity;
    }
}
