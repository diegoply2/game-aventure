using UnityEngine;
using System.Collections.Generic;

public class ProceduralRockSpawner : MonoBehaviour
{
    public Terrain terrain;          // Le terrain où les rochers seront générés
    public GameObject[] rockPrefabs; // Tableau contenant les différents prefabs de rochers
    public int rockCount = 100;      // Nombre de rochers à générer

    public Vector2 scaleFactor = new Vector2(0.8f, 1.2f); // Facteur d'échelle pour ajuster la taille des rochers

    void Start()
    {
        SpawnRocks(); // Appel à la méthode de génération des rochers
    }

    void SpawnRocks()
    {
        TerrainData terrainData = terrain.terrainData;
        List<TreeInstance> rockInstances = new List<TreeInstance>();
        int attempts = 0;
        int maxAttempts = rockCount * 5; // Limite pour éviter une boucle infinie

        while (rockInstances.Count < rockCount && attempts < maxAttempts)
        {
            attempts++;

            // Génère une position aléatoire sur le terrain
            float x = Random.Range(0f, 1f);
            float z = Random.Range(0f, 1f);
            float worldX = x * terrainData.size.x;
            float worldZ = z * terrainData.size.z;
            float worldY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));

            Vector3 rockPosition = new Vector3(worldX, worldY, worldZ);

            // Sélectionne un rocher aléatoire
            int rockIndex = Random.Range(0, rockPrefabs.Length);
            GameObject rockObject = Instantiate(rockPrefabs[rockIndex], rockPosition, Quaternion.identity);

            // Ajuste l'échelle du rocher
            float scale = Random.Range(scaleFactor.x, scaleFactor.y);
            rockObject.transform.localScale *= scale;

            // Ajoute un BoxCollider
            BoxCollider boxCollider = rockObject.AddComponent<BoxCollider>();
            
            // Ajuste la taille et la position du BoxCollider en fonction du modèle
            Renderer renderer = rockObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                boxCollider.size = renderer.bounds.size;
                boxCollider.center = renderer.bounds.center - rockObject.transform.position;
            }
        }
    }
}
