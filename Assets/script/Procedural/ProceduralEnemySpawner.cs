using UnityEngine;
using System.Collections.Generic;

public class ProceduralEnemySpawner : MonoBehaviour
{
    public Terrain terrain;          // Le terrain où les ennemis seront générés
    public GameObject[] enemyPrefabs; // Tableau contenant les différents prefabs d'ennemis
    public int maxEnemies = 20;       // Nombre total d'ennemis à générer
    public float spawnRadius = 30f;   // Rayon autour du joueur dans lequel les ennemis peuvent apparaître
    public float minSpawnDistance = 10f; // Distance minimale entre le joueur et un ennemi
    public Vector2 scaleFactor = new Vector2(0.8f, 1.2f); // Facteur d'échelle pour ajuster la taille des ennemis
    public int maxPoolSize = 30; // Nombre maximum d'ennemis dans le pool

    private Transform player;  // Le joueur
    private int currentEnemyCount = 0; // Nombre d'ennemis générés jusqu'à présent
    private List<GameObject> activeEnemies = new List<GameObject>(); // Liste des ennemis actifs
    private Queue<GameObject> enemyPool = new Queue<GameObject>(); // Pool d'ennemis

    private float lastSpawnTime = 0f; // Temps de la dernière génération d'ennemi
    public float spawnInterval = 1f; // Intervalle entre chaque tentative de génération

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Trouver le joueur

        // Pré-instancier les ennemis et les désactiver
        for (int i = 0; i < maxPoolSize; i++)
        {
            int enemyIndex = Random.Range(0, enemyPrefabs.Length);
            GameObject enemyObject = Instantiate(enemyPrefabs[enemyIndex]);
            enemyObject.SetActive(false);
            enemyPool.Enqueue(enemyObject);  // Ajoute l'ennemi au pool
        }
    }

    void Update()
    {
        // Vérifie le temps écoulé avant de tenter de générer un ennemi
        if (Time.time - lastSpawnTime > spawnInterval)
        {
            lastSpawnTime = Time.time; // Met à jour le temps de la dernière génération
            if (currentEnemyCount < maxEnemies)
            {
                TrySpawnEnemiesNearPlayer();
            }
        }

        // Vérifie et désactive les ennemis morts (s'ils sont inactifs ou tués)
        CheckAndDeactivateEnemies();
    }

    void TrySpawnEnemiesNearPlayer()
    {
        // Vérifie si le joueur est suffisamment proche du centre de la carte pour faire apparaître des ennemis
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < spawnRadius)
        {
            // Génère une position aléatoire pour les ennemis autour du joueur
            Vector3 spawnPosition = GenerateRandomSpawnPosition();

            // Si la position est valide (assez loin du joueur et sur le terrain)
            if (spawnPosition != Vector3.zero)
            {
                SpawnEnemyAtPosition(spawnPosition);
            }
        }
    }

    // Génère une position aléatoire autour du joueur
    Vector3 GenerateRandomSpawnPosition()
    {
        Vector3 randomPosition = Vector3.zero;

        // Crée une position aléatoire à l'intérieur d'un rayon autour du joueur
        float angle = Random.Range(0f, 2 * Mathf.PI);
        float distance = Random.Range(minSpawnDistance, spawnRadius); // Distance entre minSpawnDistance et spawnRadius
        float xOffset = Mathf.Cos(angle) * distance;
        float zOffset = Mathf.Sin(angle) * distance;

        float worldX = player.position.x + xOffset;
        float worldZ = player.position.z + zOffset;
        float worldY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));

        randomPosition = new Vector3(worldX, worldY, worldZ);

        // Vérifie si la position est suffisamment éloignée du joueur et sur le terrain
        if (Vector3.Distance(player.position, randomPosition) >= minSpawnDistance)
        {
            return randomPosition; // Retourne la position si elle est valide
        }

        return Vector3.zero; // Retourne zéro si aucune position valide n'a été trouvée
    }

    // Spawns un ennemi à une position donnée
    void SpawnEnemyAtPosition(Vector3 position)
    {
        GameObject enemyObject;

        if (enemyPool.Count > 0)
        {
            // Récupère un ennemi inactif du pool
            enemyObject = enemyPool.Dequeue();
        }
        else
        {
            // Si le pool est vide, instancier un nouvel ennemi
            int enemyIndex = Random.Range(0, enemyPrefabs.Length);
            enemyObject = Instantiate(enemyPrefabs[enemyIndex]);
        }

        // Positionne et active l'ennemi
        enemyObject.transform.position = position;
        enemyObject.SetActive(true);

        // Ajuste l'échelle de l'ennemi
        float scale = Random.Range(scaleFactor.x, scaleFactor.y);
        enemyObject.transform.localScale = new Vector3(scale, scale, scale);

        // Ajoute l'ennemi à la liste active
        activeEnemies.Add(enemyObject);

        // Augmente le compteur d'ennemis générés
        currentEnemyCount++;
    }

    // Vérifie les ennemis qui sont morts ou inactifs et les désactive
    void CheckAndDeactivateEnemies()
    {
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            GameObject enemy = activeEnemies[i];

            if (enemy != null)
            {
                // Si l'ennemi est désactivé (mort ou hors-jeu), on peut le réactiver
                if (!enemy.activeInHierarchy)
                {
                    currentEnemyCount--; // Réduire le compteur d'ennemis actifs
                    activeEnemies.RemoveAt(i); // Supprimer de la liste des ennemis actifs
                    i--; // Réajuster l'index

                    // Ajoute l'ennemi au pool
                    enemyPool.Enqueue(enemy);
                }
            }
        }
    }
}
