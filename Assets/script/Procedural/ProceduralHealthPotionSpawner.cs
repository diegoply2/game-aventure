using UnityEngine;
using System.Collections.Generic;

public class ProceduralHealthPotionSpawner : MonoBehaviour
{
    public Terrain terrain;          // Le terrain où les potions seront générées
    public GameObject[] potionPrefabs; // Tableau contenant les différents prefabs de potions
    public int potionCount = 100;      // Nombre de potions à générer

    public Vector2 scaleFactor = new Vector2(0.8f, 1.2f); // Facteur d'échelle pour ajuster la taille des potions

    void Start()
    {
        SpawnPotions(); // Appel à la méthode de génération des potions
    }

    void SpawnPotions()
    {
        TerrainData terrainData = terrain.terrainData;
        List<TreeInstance> potionInstances = new List<TreeInstance>();
        int attempts = 0;
        int maxAttempts = potionCount * 5; // Limite pour éviter une boucle infinie

        while (potionInstances.Count < potionCount && attempts < maxAttempts)
        {
            attempts++;

            // Génère une position aléatoire sur le terrain
            float x = Random.Range(0f, 1f);
            float z = Random.Range(0f, 1f);
            float worldX = x * terrainData.size.x;
            float worldZ = z * terrainData.size.z;
            float worldY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));

            Vector3 potionPosition = new Vector3(worldX, worldY, worldZ);

            // Sélectionne une potion aléatoire
            int potionIndex = Random.Range(0, potionPrefabs.Length);
            GameObject potionObject = Instantiate(potionPrefabs[potionIndex], potionPosition, Quaternion.identity);

            // Ajuste l'échelle de la potion
            float scale = Random.Range(scaleFactor.x, scaleFactor.y);
            potionObject.transform.localScale *= scale;

            // Ajoute un BoxCollider
            BoxCollider boxCollider = potionObject.AddComponent<BoxCollider>();
            
            // Ajuste la taille et la position du BoxCollider en fonction du modèle
            Renderer renderer = potionObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                boxCollider.size = renderer.bounds.size;
                boxCollider.center = renderer.bounds.center - potionObject.transform.position;
            }
        }
    }
}
