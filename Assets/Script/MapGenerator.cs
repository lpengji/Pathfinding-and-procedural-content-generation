using UnityEngine;
using UnityEngine.Tilemaps;

public class PerlinNoiseTilemapCreator : MonoBehaviour
{
    public float magnification = 7.0f;  // Magnitud del ruido
    public Tile[] tileset;
    Tilemap tilemap;
    public int halfTilemapSize = 10;    // Tamaño del tilemap
    public float paintProbability = 0.3f; // Probabilidad de pintar en una posición

    void Start()
    {
        tilemap = GetComponent<Tilemap>();

        for (int x = -halfTilemapSize; x < halfTilemapSize; x++)
        {
            for (int y = -halfTilemapSize; y < halfTilemapSize; y++)
            {
                if (ShouldPaintAtPosition())
                {
                    int tileId = GetIdUsingPerlin(x, y);
                    tilemap.SetTile(new Vector3Int(x, y, 0), tileset[tileId]);
                }
            }
        }
    }

    bool ShouldPaintAtPosition()    // decidir si pintar o no en el tilemap
    {
        float randomValue = Random.value; // Valor aleatorio entre 0 y 1

        return randomValue < paintProbability;
    }

    int GetIdUsingPerlin(int x, int y)
    {
        float rawPerlin = Mathf.PerlinNoise(x / magnification, y / magnification);
        int categorizedPerlin = Mathf.RoundToInt(rawPerlin * tileset.Length) % tileset.Length;
        return categorizedPerlin;
    }
}
