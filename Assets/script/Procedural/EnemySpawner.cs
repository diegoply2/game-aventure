using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs; // Tableaux des types d'ennemis
    public int minEnemiesPerGroup = 3;
    public int maxEnemiesPerGroup = 9;
    public float spawnRadius = 50f; // Rayon d'apparition des ennemis
    public float minSpawnDistance = 10f; // Distance minimale du joueur
    public float spawnInterval = 5f; // Temps entre les vagues
    public int groupsPerWave = 3; // Nombre de groupes d'ennemis par vague
    public float minGroupDistance = 20f; // Distance minimale entre les groupes d'ennemis

    public Terrain terrain; // Ajout de la référence explicite du terrain

    private Transform player;
    private List<Vector3> groupPositions = new List<Vector3>(); // Liste des positions des groupes

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null)
        {
            Debug.LogError("Aucun joueur trouvé avec le tag 'Player'.");
            return;
        }

        if (terrain == null)
        {
            Debug.LogError("Terrain n'est pas assigné.");
            return;
        }

        StartCoroutine(SpawnEnemyGroups());
    }

    IEnumerator SpawnEnemyGroups()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnEnemyWave();
        }
    }

    void SpawnEnemyWave()
    {
        // Continue normalement sans aucune vérification de limite d'ennemis

        groupPositions.Clear(); // Réinitialise les positions des groupes à chaque nouvelle vague

        for (int i = 0; i < groupsPerWave; i++)
        {
            Vector3 spawnCenter = GetValidGroupPosition();
            groupPositions.Add(spawnCenter); // Ajoute la position du groupe à la liste

            int enemyCount = Random.Range(minEnemiesPerGroup, maxEnemiesPerGroup + 1);

            for (int j = 0; j < enemyCount; j++)
            {
                Vector3 spawnPosition = spawnCenter + Random.insideUnitSphere * 3f;
                spawnPosition.y = terrain.SampleHeight(spawnPosition); // Utilise le terrain assigné
                GameObject enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], spawnPosition, Quaternion.identity);
            }
        }
    }

    Vector3 GetValidGroupPosition()
    {
        Vector3 spawnPosition;
        bool isValidPosition;

        do
        {
            float angle = Random.Range(0f, 2 * Mathf.PI);
            float distance = Random.Range(minSpawnDistance, spawnRadius);
            float xOffset = Mathf.Cos(angle) * distance;
            float zOffset = Mathf.Sin(angle) * distance;

            spawnPosition = new Vector3(player.position.x + xOffset, 0, player.position.z + zOffset);
            spawnPosition.y = terrain.SampleHeight(spawnPosition); // Utilise le terrain assigné

            // Vérifie si le groupe est suffisamment éloigné des autres groupes
            isValidPosition = true;
            foreach (Vector3 groupPos in groupPositions)
            {
                if (Vector3.Distance(spawnPosition, groupPos) < minGroupDistance)
                {
                    isValidPosition = false;
                    break;
                }
            }

        } while (!isValidPosition);

        return spawnPosition;
    }
}
