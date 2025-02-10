using UnityEngine;
using System.Collections.Generic;

public class ProceduralArbre : MonoBehaviour
{
    public Terrain terrain;
    public GameObject[] treePrefabs; // Plusieurs types d'arbres
    public int treeCount = 100;
    public float exclusionRadius = 5f; // Rayon d'exclusion autour des objets à éviter
    public float sizeFactor = 0.5f;   // Facteur pour ajuster la taille du Collider

    void Start()
    {
        AddTreePrototypes();
        SpawnTrees();
    }

    void AddTreePrototypes()
    {
        TerrainData terrainData = terrain.terrainData;
        List<TreePrototype> treePrototypes = new List<TreePrototype>();

        foreach (GameObject treePrefab in treePrefabs)
        {
            TreePrototype treePrototype = new TreePrototype { prefab = treePrefab };
            treePrototypes.Add(treePrototype);
        }

        terrainData.treePrototypes = treePrototypes.ToArray();
    }

    void SpawnTrees()
    {
        TerrainData terrainData = terrain.terrainData;
        List<TreeInstance> treeInstances = new List<TreeInstance>();

        // Récupération des objets à exclure
        List<Vector3> exclusionZones = GetExclusionZones();

        int attempts = 0;
        int maxAttempts = treeCount * 5; // Évite une boucle infinie

        while (treeInstances.Count < treeCount && attempts < maxAttempts)
        {
            attempts++;

            float x = Random.Range(0f, 1f);
            float z = Random.Range(0f, 1f);
            float worldX = x * terrainData.size.x;
            float worldZ = z * terrainData.size.z;
            float worldY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));

            Vector3 treePosition = new Vector3(worldX, worldY, worldZ);

            if (IsPositionValid(treePosition, exclusionZones))
            {
                TreeInstance tree = new TreeInstance
                {
                    position = new Vector3(x, worldY / terrainData.size.y, z),
                    prototypeIndex = Random.Range(0, treePrefabs.Length), // Arbre aléatoire
                    widthScale = Random.Range(0.8f, 1.2f),
                    heightScale = Random.Range(0.8f, 1.2f),
                    color = Color.white,
                    lightmapColor = Color.white
                };

                // Ajouter le CapsuleCollider dynamique pour l'arbre
                GameObject treeObject = Instantiate(treePrefabs[tree.prototypeIndex], treePosition, Quaternion.identity);
                AddColliderToTree(treeObject);

                treeInstances.Add(tree);
            }
        }

        terrainData.treeInstances = treeInstances.ToArray();
    }

    List<Vector3> GetExclusionZones()
    {
        List<Vector3> exclusionZones = new List<Vector3>();
        string[] tagsToExclude = { "Player", "Enemy", "Potion", "Rocher", "Plant" };

        foreach (string tag in tagsToExclude)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            exclusionZones.AddRange(GetPositions(objects));
        }

        return exclusionZones;
    }

    List<Vector3> GetPositions(GameObject[] objects)
    {
        List<Vector3> positions = new List<Vector3>();
        foreach (GameObject obj in objects)
        {
            positions.Add(obj.transform.position);
        }
        return positions;
    }

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

    // Ajouter un CapsuleCollider aux arbres générés dynamiquement
    void AddColliderToTree(GameObject treeObject)
    {
        // Vérifier si l'arbre a un Collider attaché, sinon on l'ajoute
        CapsuleCollider existingCollider = treeObject.GetComponent<CapsuleCollider>();
        if (existingCollider != null)
        {
            Destroy(existingCollider); // Supprimer les colliders existants si nécessaire
        }

        // Ajouter un CapsuleCollider
        CapsuleCollider capsuleCollider = treeObject.AddComponent<CapsuleCollider>();

        // Ajuster la taille du Collider en fonction de la taille du prefab de l'arbre
        float treeRadius = Mathf.Max(treeObject.transform.localScale.x, treeObject.transform.localScale.z);
        capsuleCollider.radius = treeRadius * sizeFactor;

        // Ajuster la hauteur du CapsuleCollider en fonction de la hauteur du prefab
        float treeHeight = treeObject.transform.localScale.y;
        capsuleCollider.height = treeHeight;

        // Centrer le CapsuleCollider sur l'arbre
        capsuleCollider.center = new Vector3(0, treeHeight / 2, 0); // Centré sur la hauteur de l'arbre

        // Assurer que le collider est en mode "physique" (non trigger)
        capsuleCollider.isTrigger = false;
    }
}
