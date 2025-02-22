using UnityEngine;

public class ProceduralTerrainGenerator : MonoBehaviour
{
    public Terrain terrain;               // Le terrain à modifier
    public float scale = 50f;             // Échelle de variation du terrain (plus grand = plus espacé)
    public float heightFactor = 0.02f;    // Facteur d'intensité pour les variations de hauteur (faible pour collines petites)
    public float hillFactor = 0.1f;      // Facteur pour les collines (encore plus faible pour des collines petites)
    public float smoothFactor = 0.4f;     // Facteur de lissage (plus haut = transitions plus douces)

    void Start()
    {
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain non assigné.");
            return;
        }

        TerrainData terrainData = terrain.terrainData;
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;

        // Créer un tableau pour stocker les hauteurs de terrain
        float[,] heights = new float[width, height];

        // Générer un terrain procédural avec des bruit à différentes échelles
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // Coordonnées normalisées
                float xCoord = (float)x / width * scale;
                float zCoord = (float)z / height * scale;

                // Bruit de Perlin pour le terrain général (faible variation pour les zones plates)
                float baseHeight = Mathf.PerlinNoise(xCoord, zCoord) * heightFactor;

                // Bruit de Perlin pour les collines (variations plus douces et espacées)
                float hillHeight = Mathf.PerlinNoise(xCoord * 0.2f, zCoord * 0.2f) * hillFactor;

                // Calculer la hauteur finale en combinant les bruits de base et collines
                float finalHeight = baseHeight + hillHeight;

                // Limiter les valeurs pour éviter des hauteurs trop extrêmes
                finalHeight = Mathf.Clamp01(finalHeight);
                
                heights[x, z] = finalHeight;
            }
        }

        // Appliquer un léger lissage des hauteurs pour adoucir les transitions
        heights = SmoothHeights(heights, smoothFactor);

        // Appliquer les nouvelles hauteurs au terrain
        terrainData.SetHeights(0, 0, heights);
    }

    // Lissage des hauteurs pour adoucir les transitions
    float[,] SmoothHeights(float[,] heights, float smoothFactor)
    {
        int width = heights.GetLength(0);
        int height = heights.GetLength(1);
        float[,] smoothedHeights = new float[width, height];

        for (int x = 1; x < width - 1; x++)
        {
            for (int z = 1; z < height - 1; z++)
            {
                // Moyenne des voisins (lissage simple)
                float averageHeight = 0f;
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dz = -1; dz <= 1; dz++)
                    {
                        averageHeight += heights[x + dx, z + dz];
                    }
                }

                // Appliquer le lissage avec un facteur
                smoothedHeights[x, z] = averageHeight / 9f;
            }
        }

        return smoothedHeights;
    }
}
