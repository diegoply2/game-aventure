using UnityEngine;
using System.Collections.Generic;

public class ProceduralHealthPotionSpawner : MonoBehaviour
{
    public Terrain terrain;          // Le terrain où les potions seront générées
    public GameObject healthPotionPrefab; // Le prefab de la potion de santé
    public int potionCount = 50;      // Nombre total de potions à générer
    public float spawnRadius = 50f;   // Rayon autour du joueur dans lequel les potions peuvent apparaître
    public float updateInterval = 1f; // Intervalle entre chaque mise à jour de la génération de potions
    public float minDistance = 10f;   // Distance minimale avant que les objets n'apparaissent
    public float minSpawnDistance = 5f; // Distance minimale avant qu'une potion ne soit générée autour du joueur

    private Transform player;  // Le joueur
    private float lastUpdateTime = 0f;  // Temps de la dernière mise à jour

    void Start()
    {
        // Assurez-vous que le joueur est bien trouvé
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null)
        {
            Debug.LogError("Aucun joueur trouvé avec le tag 'Player'.");
            return;
        }

        // Vous pouvez activer ici la génération continue
        StartCoroutine(GenerateHealthPotionsAtIntervals());
    }

    // Générer des potions de santé autour du joueur pendant qu'il se déplace
    void GenerateHealthPotionsAroundPlayer()
    {
        if (player == null || terrain == null)
        {
            return;
        }

        // Créer des potions autour du joueur dans un rayon défini
        for (float angle = 0f; angle < 2 * Mathf.PI; angle += Mathf.PI / 10f)
        {
            // Position aléatoire autour du joueur
            float distance = Random.Range(minSpawnDistance, spawnRadius); // Distance aléatoire entre minSpawnDistance et spawnRadius
            float xOffset = Mathf.Cos(angle) * distance;
            float zOffset = Mathf.Sin(angle) * distance;

            // Générer une position du monde pour la potion
            float worldX = player.position.x + xOffset;
            float worldZ = player.position.z + zOffset;

            // Vérifier si la potion est à l'intérieur du terrain
            worldX = Mathf.Clamp(worldX, 0f, terrain.terrainData.size.x);
            worldZ = Mathf.Clamp(worldZ, 0f, terrain.terrainData.size.z);

            float worldY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));

            Vector3 potionPosition = new Vector3(worldX, worldY, worldZ);

            // Créer une instance de potion
            GameObject healthPotion = Instantiate(healthPotionPrefab, potionPosition, Quaternion.identity);

            // Vous pouvez ajouter des comportements supplémentaires ici pour les potions
        }
    }

    // Coroutine pour générer les potions à intervalles réguliers
    System.Collections.IEnumerator GenerateHealthPotionsAtIntervals()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateInterval); // Attendre l'intervalle défini
            GenerateHealthPotionsAroundPlayer(); // Générer des potions autour du joueur
        }
    }
}
