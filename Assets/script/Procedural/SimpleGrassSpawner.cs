using UnityEngine;

public class SimpleGrassSpawner : MonoBehaviour
{
    public Terrain terrain;  // Terrain Unity
    public Texture2D grassTexture;  // Texture d'herbe (type Grass Texture)
    public int grassCount = 1000;  // Nombre d'herbes à générer
    public float brushRadius = 100f;  // Rayon du brush
    public bool useBillboard = false;  // Pour choisir le mode de rendu

    void Start()
    {
        // Vérification des éléments nécessaires
        if (terrain == null || grassTexture == null)
        {
            Debug.LogError("Terrain ou texture d'herbe non assignés !");
            return;
        }

        // Créer un prototype d'herbe avec la texture
        DetailPrototype[] detailPrototypes = new DetailPrototype[1];
        detailPrototypes[0] = new DetailPrototype();
        detailPrototypes[0].prototypeTexture = grassTexture;  // Assigner la texture de type Grass
        detailPrototypes[0].renderMode = useBillboard ? DetailRenderMode.GrassBillboard : DetailRenderMode.Grass;
        detailPrototypes[0].usePrototypeMesh = false;  // Pas de mesh personnalisé, on utilise la texture comme détail

        // Réinitialisation de la couche de détails
        terrain.terrainData.detailPrototypes = detailPrototypes;

        // Augmenter la résolution des détails pour une plus grande densité
        terrain.terrainData.SetDetailResolution(1024, 16);  // On augmente la résolution des détails

        // Peindre l'herbe sur le terrain
        PaintGrass();
    }

void PaintGrass()
{
    // Obtenir la largeur et la hauteur du terrain
    int width = terrain.terrainData.detailWidth;
    int height = terrain.terrainData.detailHeight;

    // Créer un tableau pour stocker les détails (herbe)
    int[,] detailLayer = new int[width, height];

    // Log pour vérifier que l'herbe est bien placée
    Debug.Log("Démarrage de la peinture d'herbe");

    // Placer l'herbe dans des positions aléatoires
    for (int i = 0; i < grassCount; i++)
    {
        int x = Random.Range(0, width);
        int z = Random.Range(0, height);
        
        // Calculer la position en monde de x et z
        float worldX = terrain.transform.position.x + (x * terrain.terrainData.size.x / width);
        float worldZ = terrain.transform.position.z + (z * terrain.terrainData.size.z / height);

        // Obtenir la hauteur du terrain à cette position
        float y = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));

        // Déplacer l'herbe en fonction de la hauteur du terrain
        Vector3 position = new Vector3(worldX, y, worldZ);

        // Ajouter l'herbe si elle est dans le rayon de la brosse
        if (Vector3.Distance(position, transform.position) <= brushRadius)
        {
            detailLayer[x, z] = 1;  // Marquer un emplacement d'herbe

            // Log pour les 100 premiers placements
            if (i < 100)
                Debug.Log($"Placer l'herbe à x={x}, z={z}, y={y}, distance={Vector3.Distance(position, transform.position)}");
        }
    }

    // Appliquer les détails au terrain
    terrain.terrainData.SetDetailLayer(0, 0, 0, detailLayer);

    Debug.Log("Peinture terminée");
}

}
