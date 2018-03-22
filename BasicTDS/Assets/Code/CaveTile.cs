using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CaveTile : Tile
{
    [SerializeField]
    public Sprite[] _sprites;

    [SerializeField]
    public Sprite _preview;

    public bool visited = false;

    public override void RefreshTile(Vector3Int pos, ITilemap tilemap)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3Int nPos = new Vector3Int(pos.x + x, pos.y + y, pos.z);
                if (IsCave(tilemap, nPos))
                {
                    // no infinite loop going back and forth here, different RefreshTile method
                    tilemap.RefreshTile(nPos);
                }
            }
        }
    }

    public override void GetTileData(Vector3Int pos, ITilemap tilemap, ref TileData tileData)
    {

        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (x != 0 || y != 0)
                {
                    Vector3Int nPos = new Vector3Int(pos.x + x, pos.y + y, pos.z);
                    if (IsCave(tilemap, nPos))
                    {

                    }
                    else
                    {

                    }
                }
            }
        }
    }

    private bool IsCave(ITilemap tilemap, Vector3Int pos)
    {
        return (tilemap.GetTile(pos) == this);
    }

    public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
    {
        return base.GetTileAnimationData(position, tilemap, ref tileAnimationData);
    }
}
