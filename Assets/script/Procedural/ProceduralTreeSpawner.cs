using UnityEngine;
using System.Collections.Generic;

public class ProceduralTreeSpawner : MonoBehaviour
{
    public Terrain terrain;          // Le terrain où les arbres seront générés
    public GameObject[] treePrefabs; // Tableau contenant les différents prefabs d'arbres
    public int treeCount = 100;      // Nombre total d'arbres à générer
    public float spawnRadius = 50f;  // Rayon autour du joueur dans lequel les arbres peuvent apparaître
    public Vector2 scaleFactor = new Vector2(0.8f, 1.2f); // Facteur d'échelle pour ajuster la taille des arbres
    public float updateInterval = 0.5f; // Intervalle entre chaque mise à jour de la génération d'arbres
    public int initialTreeCount = 20;  // Nombre d'arbres à générer immédiatement au lancement du jeu

    private Transform player;  // Le joueur
    private float lastUpdateTime = 0f;  // Temps de la dernière mise à jour

    public float minDistance = 30f;  // Distance minimale avant que les objets n'apparaissent

    void Start()
    {
        // Assurez-vous que le joueur est bien trouvé
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null)
        {
            Debug.LogError("Aucun joueur trouvé avec le tag 'Player'.");
            return;
        }

        // Générer un premier groupe d'arbres proches du joueur immédiatement au début
        GenerateInitialTrees();

        // Vous pouvez activer ici la génération continue
        StartCoroutine(GenerateTreesAtIntervals());
    }

    // Générer un premier groupe d'arbres immédiatement au lancement du jeu
    void GenerateInitialTrees()
    {
        if (player == null || terrain == null)
        {
            return;
        }

        TerrainData terrainData = terrain.terrainData;
        List<TreePrototype> treePrototypes = new List<TreePrototype>();

        // Ajouter les prototypes d'arbres dans le tableau du terrain
        foreach (GameObject treePrefab in treePrefabs)
        {
            if (treePrefab != null)
            {
                TreePrototype treePrototype = new TreePrototype { prefab = treePrefab };
                treePrototypes.Add(treePrototype);
            }
        }

        if (treePrototypes.Count == 0)
        {
            Debug.LogError("Aucun arbre valide trouvé.");
            return;
        }

        terrainData.treePrototypes = treePrototypes.ToArray();

        // Créer des arbres proches du joueur dès le début, mais à une distance minimale
        for (int i = 0; i < initialTreeCount; i++)
        {
            // Position aléatoire autour du joueur
            float angle = Random.Range(0f, 2 * Mathf.PI); // Angle aléatoire autour du joueur
            float distance = Random.Range(minDistance, spawnRadius); // Distance aléatoire proche du joueur mais pas trop près
            float xOffset = Mathf.Cos(angle) * distance;
            float zOffset = Mathf.Sin(angle) * distance;

            // Générer une position du monde pour l'arbre
            float worldX = player.position.x + xOffset;
            float worldZ = player.position.z + zOffset;

            // Vérifier si l'arbre est à l'intérieur du terrain
            worldX = Mathf.Clamp(worldX, 0f, terrain.terrainData.size.x);
            worldZ = Mathf.Clamp(worldZ, 0f, terrain.terrainData.size.z);

            float worldY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));

            Vector3 treePosition = new Vector3(worldX, worldY, worldZ);

            // Créer une instance d'arbre
            TreeInstance treeInstance = new TreeInstance
            {
                position = new Vector3(worldX / terrain.terrainData.size.x, worldY / terrain.terrainData.size.y, worldZ / terrain.terrainData.size.z),
                prototypeIndex = Random.Range(0, treePrefabs.Length),
                widthScale = Random.Range(scaleFactor.x, scaleFactor.y),
                heightScale = Random.Range(scaleFactor.x, scaleFactor.y),
                color = Color.white,
                lightmapColor = Color.white
            };

            // Appliquer l'instance d'arbre
            terrain.terrainData.treeInstances = new TreeInstance[] { treeInstance };

            // Créer un objet d'arbre dans la scène
            GameObject treeObject = Instantiate(treePrefabs[treeInstance.prototypeIndex], treePosition, Quaternion.identity);

            // Ajouter un CapsuleCollider à l'arbre
            AddCapsuleCollider(treeObject, treeInstance.heightScale);
        }
    }

    // Ajouter un CapsuleCollider à l'arbre en fonction de son échelle
    void AddCapsuleCollider(GameObject treeObject, float heightScale)
    {
        CapsuleCollider capsuleCollider = treeObject.AddComponent<CapsuleCollider>();

        // Réglez le rayon du collider proportionnellement à l'échelle de l'arbre
        capsuleCollider.radius = 0.5f * heightScale;  // Ajustez cette valeur selon la taille de votre arbre
        capsuleCollider.height = 3f * heightScale;     // Ajustez cette valeur pour changer la hauteur du collider
        capsuleCollider.center = new Vector3(0, heightScale / 2f, 0); // Positionner le collider au centre de l'arbre
    }

    // Générer des arbres autour du joueur pendant qu'il se déplace
    void GenerateTreesAroundPlayer()
    {
        if (player == null || terrain == null)
        {
            return;
        }

        TerrainData terrainData = terrain.terrainData;
        List<TreePrototype> treePrototypes = new List<TreePrototype>();

        // Ajouter les prototypes d'arbres dans le tableau du terrain
        foreach (GameObject treePrefab in treePrefabs)
        {
            if (treePrefab != null)
            {
                TreePrototype treePrototype = new TreePrototype { prefab = treePrefab };
                treePrototypes.Add(treePrototype);
            }
        }

        if (treePrototypes.Count == 0)
        {
            Debug.LogError("Aucun arbre valide trouvé.");
            return;
        }

        terrainData.treePrototypes = treePrototypes.ToArray();

        // Créer des arbres autour du joueur dans un rayon défini
        for (float angle = 0f; angle < 2 * Mathf.PI; angle += Mathf.PI / 10f)
        {
            // Position aléatoire autour du joueur
            float distance = Random.Range(10f, spawnRadius); // Distance aléatoire entre 10 et spawnRadius
            float xOffset = Mathf.Cos(angle) * distance;
            float zOffset = Mathf.Sin(angle) * distance;

            // Générer une position du monde pour l'arbre
            float worldX = player.position.x + xOffset;
            float worldZ = player.position.z + zOffset;

            // Vérifier si l'arbre est à l'intérieur du terrain
            worldX = Mathf.Clamp(worldX, 0f, terrain.terrainData.size.x);
            worldZ = Mathf.Clamp(worldZ, 0f, terrain.terrainData.size.z);

            float worldY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));

            Vector3 treePosition = new Vector3(worldX, worldY, worldZ);

            // Créer une instance d'arbre
            TreeInstance treeInstance = new TreeInstance
            {
                position = new Vector3(worldX / terrain.terrainData.size.x, worldY / terrain.terrainData.size.y, worldZ / terrain.terrainData.size.z),
                prototypeIndex = Random.Range(0, treePrefabs.Length),
                widthScale = Random.Range(scaleFactor.x, scaleFactor.y),
                heightScale = Random.Range(scaleFactor.x, scaleFactor.y),
                color = Color.white,
                lightmapColor = Color.white
            };

            // Appliquer l'instance d'arbre
            terrain.terrainData.treeInstances = new TreeInstance[] { treeInstance };

            // Créer un objet d'arbre dans la scène
            GameObject treeObject = Instantiate(treePrefabs[treeInstance.prototypeIndex], treePosition, Quaternion.identity);

            // Ajouter un CapsuleCollider à l'arbre
            AddCapsuleCollider(treeObject, treeInstance.heightScale);
        }
    }

    // Coroutine pour générer les arbres à intervalles réguliers
    System.Collections.IEnumerator GenerateTreesAtIntervals()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateInterval); // Attendre l'intervalle défini
            GenerateTreesAroundPlayer(); // Générer des arbres autour du joueur
        }
    }
}
