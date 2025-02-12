using UnityEngine;
using System.Collections.Generic;

public class ProceduralRockSpawner : MonoBehaviour
{
    public Terrain terrain;          // Le terrain où les rochers seront générés
    public GameObject[] rockPrefabs; // Tableau contenant les différents prefabs de rochers
    public int rockCount = 100;      // Nombre total de rochers à générer
    public float spawnRadius = 50f;  // Rayon autour du joueur dans lequel les rochers peuvent apparaître
    public Vector2 scaleFactor = new Vector2(0.8f, 1.2f); // Facteur d'échelle pour ajuster la taille des rochers
    public float updateInterval = 0.5f; // Intervalle entre chaque mise à jour de la génération de rochers
    public int initialRockCount = 20;  // Nombre de rochers à générer immédiatement au lancement du jeu

    private Transform player;  // Le joueur

    public float minDistance = 30f;  

    void Start()
    {
        // Assurez-vous que le joueur est bien trouvé
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null)
        {
            Debug.LogError("Aucun joueur trouvé avec le tag 'Player'.");
            return;
        }

        // Générer un premier groupe de rochers proches du joueur immédiatement au début
        GenerateInitialRocks();

        // Démarrer la génération continue de rochers
        StartCoroutine(GenerateRocksAtIntervals());
    }

    // Générer un premier groupe de rochers immédiatement au lancement du jeu
    void GenerateInitialRocks()
    {
        if (player == null || terrain == null)
        {
            return;
        }

        // Créer des rochers proches du joueur dès le début
        for (int i = 0; i < initialRockCount; i++)
        {
            // Position aléatoire autour du joueur
            float angle = Random.Range(0f, 2 * Mathf.PI); // Angle aléatoire autour du joueur
            float distance = Random.Range(5f, spawnRadius); // Distance aléatoire proche du joueur
            float xOffset = Mathf.Cos(angle) * distance;
            float zOffset = Mathf.Sin(angle) * distance;

            // Générer une position du monde pour le rocher
            float worldX = player.position.x + xOffset;
            float worldZ = player.position.z + zOffset;

            // Vérifier si le rocher est à l'intérieur du terrain
            worldX = Mathf.Clamp(worldX, 0f, terrain.terrainData.size.x);
            worldZ = Mathf.Clamp(worldZ, 0f, terrain.terrainData.size.z);

            float worldY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));

            Vector3 rockPosition = new Vector3(worldX, worldY, worldZ);

            // Choisir un rocher aléatoire
            int randomRockIndex = Random.Range(0, rockPrefabs.Length);
            GameObject rockPrefab = rockPrefabs[randomRockIndex];

            // Appliquer une échelle aléatoire
            float scaleX = Random.Range(scaleFactor.x, scaleFactor.y);
            float scaleY = Random.Range(scaleFactor.x, scaleFactor.y);
            float scaleZ = Random.Range(scaleFactor.x, scaleFactor.y);

            // Créer un rocher dans la scène
            GameObject rock = Instantiate(rockPrefab, rockPosition, Quaternion.identity);
            rock.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);

            // Ajouter un BoxCollider au rocher en fonction de son échelle
            BoxCollider boxCollider = rock.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(scaleX, scaleY, scaleZ);
            boxCollider.center = new Vector3(0f, scaleY / 2f, 0f);  // Centrer le collider sur le rocher
        }
    }

    // Générer des rochers autour du joueur pendant qu'il se déplace
    void GenerateRocksAroundPlayer()
    {
        if (player == null || terrain == null)
        {
            return;
        }

        TerrainData terrainData = terrain.terrainData;

        // Créer des rochers autour du joueur dans un rayon défini, mais à une distance minimale
        for (float angle = 0f; angle < 2 * Mathf.PI; angle += Mathf.PI / 10f)
        {
            // Position aléatoire autour du joueur
            float distance = Random.Range(minDistance, spawnRadius); // Distance aléatoire entre minDistance et spawnRadius
            float xOffset = Mathf.Cos(angle) * distance;
            float zOffset = Mathf.Sin(angle) * distance;

            // Générer une position du monde pour le rocher
            float worldX = player.position.x + xOffset;
            float worldZ = player.position.z + zOffset;

            // Vérifier si le rocher est à l'intérieur du terrain
            worldX = Mathf.Clamp(worldX, 0f, terrain.terrainData.size.x);
            worldZ = Mathf.Clamp(worldZ, 0f, terrain.terrainData.size.z);

            float worldY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));

            Vector3 rockPosition = new Vector3(worldX, worldY, worldZ);

            // Choisir un rocher aléatoire
            int randomRockIndex = Random.Range(0, rockPrefabs.Length);
            GameObject rockPrefab = rockPrefabs[randomRockIndex];

            // Appliquer une échelle aléatoire
            float scaleX = Random.Range(scaleFactor.x, scaleFactor.y);
            float scaleY = Random.Range(scaleFactor.x, scaleFactor.y);
            float scaleZ = Random.Range(scaleFactor.x, scaleFactor.y);

            // Créer un rocher dans la scène
            GameObject rock = Instantiate(rockPrefab, rockPosition, Quaternion.identity);
            rock.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);

            // Ajouter un BoxCollider au rocher en fonction de son échelle
            BoxCollider boxCollider = rock.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(scaleX, scaleY, scaleZ);
            boxCollider.center = new Vector3(0f, scaleY / 2f, 0f);  // Centrer le collider sur le rocher
        }
    }

    // Coroutine pour générer les rochers à intervalles réguliers
    System.Collections.IEnumerator GenerateRocksAtIntervals()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateInterval); // Attendre l'intervalle défini
            GenerateRocksAroundPlayer(); // Générer des rochers autour du joueur
        }
    }
}
