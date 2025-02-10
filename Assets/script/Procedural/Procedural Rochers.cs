using UnityEngine;
using System.Collections.Generic;

public class ProceduralRochers : MonoBehaviour
{
    public Terrain terrain;  
    public GameObject[] rockPrefabs; // Plusieurs types de rochers
    public int rockCount = 50;
    public float exclusionRadius = 5f; // Rayon d'exclusion autour des objets à éviter

    void Start()
    {
        
        SpawnRocks();
    }

    void SpawnRocks()
    {
        TerrainData terrainData = terrain.terrainData;
        List<Vector3> exclusionZones = GetExclusionZones();
        List<GameObject> spawnedRocks = new List<GameObject>();

        int attempts = 0;
        int maxAttempts = rockCount * 5; // Limite pour éviter boucle infinie

        while (spawnedRocks.Count < rockCount && attempts < maxAttempts)
        {
            attempts++;

            // Position aléatoire sur le terrain
            float x = Random.Range(0f, 1f);
            float z = Random.Range(0f, 1f);
            float worldX = x * terrainData.size.x;
            float worldZ = z * terrainData.size.z;
            float worldY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));

            Vector3 rockPosition = new Vector3(worldX, worldY, worldZ);

            // Vérifier si la position est valide
            if (IsPositionValid(rockPosition, exclusionZones))
            {
                // Créer un rocher aléatoire
                GameObject rock = Instantiate(rockPrefabs[Random.Range(0, rockPrefabs.Length)], rockPosition, Quaternion.identity);
                spawnedRocks.Add(rock);
            }
        }
    }

    // Récupérer les zones à exclure (joueurs, ennemis, etc.)
    List<Vector3> GetExclusionZones()
    {
        List<Vector3> exclusionZones = new List<Vector3>();
        string[] tagsToExclude = { "Player", "Enemy", "Arbre", "Potion", "Plant" };

        foreach (string tag in tagsToExclude)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            exclusionZones.AddRange(GetPositions(objects));
        }

        return exclusionZones;
    }

    // Récupérer les positions des objets à exclure
    List<Vector3> GetPositions(GameObject[] objects)
    {
        List<Vector3> positions = new List<Vector3>();
        foreach (GameObject obj in objects)
        {
            positions.Add(obj.transform.position);
        }
        return positions;
    }

    // Vérifier si la position du rocher est valide
    bool IsPositionValid(Vector3 position, List<Vector3> exclusionZones)
    {
        foreach (Vector3 exclusionZone in exclusionZones)
        {
            if (Vector3.Distance(position, exclusionZone) < exclusionRadius)
            {
                return false; // Trop proche d'un objet à exclure
            }
        }
        return true;
    }
}

