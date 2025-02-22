using UnityEngine;
using System.Collections;

public class ProceduralEnemySpawner : MonoBehaviour
{
    public Terrain terrain;                // Le terrain où les ennemis seront générés
    public GameObject[] enemyPrefabs;      // Tableau contenant les différents types d'ennemis
    public int enemyCount = 50;            // Nombre total d'ennemis à générer
    public float spawnRadius = 50f;        // Rayon autour du joueur dans lequel les ennemis peuvent apparaître
    public float updateInterval = 1f;      // Intervalle entre chaque mise à jour de la génération des ennemis
    public int minEnemiesPerGroup = 2;     // Nombre minimum d'ennemis par groupe
    public int maxEnemiesPerGroup = 7;     // Nombre maximum d'ennemis par groupe
    [SerializeField] public int maxConcurrentEnemies = 20;  // Nombre maximum d'ennemis pouvant être présents en même temps

    private Transform player;              // Le joueur
    private int currentEnemyCount = 0;     // Compteur pour suivre le nombre d'ennemis générés

    void Start()
    {
        // Assurez-vous que le joueur est bien trouvé
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null)
        {
            Debug.LogError("Aucun joueur trouvé avec le tag 'Player'.");
            return;
        }

        // Démarrer la génération continue des ennemis
        StartCoroutine(GenerateEnemiesAtIntervals());
    }

    // Générer des ennemis autour du joueur pendant qu'il se déplace
    void GenerateEnemiesAroundPlayer()
    {
        if (player == null || terrain == null)
        {
            return;
        }

        // Vérifier s'il y a déjà trop d'ennemis autour du joueur
        if (currentEnemyCount >= maxConcurrentEnemies)
        {
            return; // Ne pas générer plus d'ennemis si la limite est atteinte
        }

        // Définir une position aléatoire autour du joueur
        int groupSize = Random.Range(minEnemiesPerGroup, maxEnemiesPerGroup + 1); // Nombre d'ennemis dans le groupe
        float angle = Random.Range(0f, 2 * Mathf.PI); // Angle aléatoire autour du joueur
        float distance = Random.Range(5f, spawnRadius); // Distance aléatoire du joueur
        float xOffset = Mathf.Cos(angle) * distance;
        float zOffset = Mathf.Sin(angle) * distance;

        // Générer une position du monde pour le groupe d'ennemis
        float worldX = player.position.x + xOffset;
        float worldZ = player.position.z + zOffset;

        // Vérifier si la position est dans les limites du terrain
        worldX = Mathf.Clamp(worldX, 0f, terrain.terrainData.size.x);
        worldZ = Mathf.Clamp(worldZ, 0f, terrain.terrainData.size.z);

        float worldY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));

        Vector3 groupPosition = new Vector3(worldX, worldY, worldZ);

        // Générer des ennemis pour le groupe à la position calculée
        for (int i = 0; i < groupSize; i++)
        {
            // Position aléatoire pour chaque ennemi dans le groupe
            Vector3 enemyPosition = groupPosition + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));

            // Choisir un ennemi aléatoire
            int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
            GameObject enemyPrefab = enemyPrefabs[randomEnemyIndex];

            // Créer un ennemi dans la scène
            Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);

            // Augmenter le nombre d'ennemis présents
            currentEnemyCount++;
        }
    }

    // Coroutine pour générer les ennemis à intervalles réguliers
    System.Collections.IEnumerator GenerateEnemiesAtIntervals()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateInterval); // Attendre l'intervalle défini
            GenerateEnemiesAroundPlayer(); // Générer des ennemis autour du joueur
        }
    }

    // Méthode pour réduire le nombre d'ennemis lorsque des ennemis sont détruits
    public void EnemyDestroyed()
    {
        currentEnemyCount--; // Réduire le nombre d'ennemis quand un ennemi est détruit
    }
}
