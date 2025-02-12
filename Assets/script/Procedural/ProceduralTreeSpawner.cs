using UnityEngine;
using System.Collections.Generic;

public class ProceduralTreeSpawner : MonoBehaviour
{
    public Terrain terrain;          // Le terrain où les arbres seront générés
    public GameObject[] treePrefabs; // Tableau contenant les différents prefabs d'arbres
    public int treeCount = 100;      // Nombre d'arbres à générer

    // Facteur d'échelle pour ajuster la taille des arbres générés
    public Vector2 scaleFactor = new Vector2(0.8f, 1.2f); 

    void Start()
    {
        SpawnTrees(); // Appel à la méthode de génération des arbres
    }

    void SpawnTrees()
    {
        TerrainData terrainData = terrain.terrainData;
        List<TreePrototype> treePrototypes = new List<TreePrototype>();

        // Ajoute les prototypes d'arbres dans le tableau du terrain
        foreach (GameObject treePrefab in treePrefabs)
        {
            if (treePrefab != null)
            {
                TreePrototype treePrototype = new TreePrototype { prefab = treePrefab };
                treePrototypes.Add(treePrototype);
            }
        }

        // Assure-toi qu'il y a des prototypes valides
        if (treePrototypes.Count == 0)
        {
            Debug.LogError("Aucun arbre valide trouvé.");
            return;
        }

        terrainData.treePrototypes = treePrototypes.ToArray();

        List<TreeInstance> treeInstances = new List<TreeInstance>();
        int attempts = 0;
        int maxAttempts = treeCount * 5; // Limite pour éviter une boucle infinie

        while (treeInstances.Count < treeCount && attempts < maxAttempts)
        {
            attempts++;

            // Génère une position aléatoire sur le terrain
            float x = Random.Range(0f, 1f);
            float z = Random.Range(0f, 1f);
            float worldX = x * terrainData.size.x;
            float worldZ = z * terrainData.size.z;
            float worldY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));

            Vector3 treePosition = new Vector3(worldX, worldY, worldZ);

            // Crée une instance d'arbre à une position valide
            TreeInstance treeInstance = new TreeInstance
            {
                position = new Vector3(x, worldY / terrainData.size.y, z), // Position normalisée
                prototypeIndex = Random.Range(0, treePrefabs.Length), // Choisit un arbre aléatoire
                widthScale = Random.Range(scaleFactor.x, scaleFactor.y), // Échelle de largeur aléatoire
                heightScale = Random.Range(scaleFactor.x, scaleFactor.y), // Échelle de hauteur aléatoire
                color = Color.white, // Couleur (ici par défaut, à ajuster)
                lightmapColor = Color.white // Coloration pour l'éclairage
            };

            treeInstances.Add(treeInstance);

            // Ajoute un collider capsule à l'arbre généré
            GameObject treeObject = Instantiate(treePrefabs[treeInstance.prototypeIndex], treePosition, Quaternion.identity);
            CapsuleCollider capsuleCollider = treeObject.AddComponent<CapsuleCollider>();

            // Ajuste la taille du capsule collider en fonction de l'échelle de l'arbre
            capsuleCollider.height = treeInstance.heightScale * 5f; // Ajuste la hauteur du collider (taille de l'arbre)
            capsuleCollider.radius = treeInstance.widthScale * 0.5f; // Ajuste le rayon du collider (épaisseur de l'arbre)

            // Place correctement le collider à la position de l'arbre
            capsuleCollider.center = new Vector3(0, capsuleCollider.height / 2, 0);
        }

        // Applique les instances d'arbres au terrain
        terrainData.treeInstances = treeInstances.ToArray();
    }
}
