using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform
{
    private List<Vector3Int> tilePositions;

    public Platform()
    {
        tilePositions = new List<Vector3Int>();
    }

    public List<Vector3Int> GetPlatform()
    {
        return tilePositions;
    }

    public void AddTileToPlatform(Vector3Int tilePosition)
    {
        tilePositions.Add(tilePosition);
    }

    public Vector3Int GetLeftBlock()
    {
        Vector3Int min = tilePositions[0];
        foreach (Vector3Int pos in tilePositions)
        {
            if (pos.x < min.x)
            {
                min = pos;
            }
        }
        return min;
    }

    public Vector3Int GetRightBlock()
    {
        Vector3Int max = tilePositions[0];
        foreach (Vector3Int pos in tilePositions)
        {
            if (pos.x > max.x)
            {
                max = pos;
            }
        }
        return max;
    }

    public Vector3Int GetRandomBlock()
    {
        return tilePositions[Random.Range(0, tilePositions.Count)];
    }

    public bool PositionIsConnected(Vector3Int tilePosition)
    {
        foreach (Vector3Int pos in tilePositions)
        {
            if (tilePosition.y != pos.y)
            {
                return false;
            }
            if (tilePosition.x + 1 == pos.x || tilePosition.x - 1 == pos.x)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsEmpty()
    {
        return tilePositions.Count == 0;
    }

    public bool Contains(Vector3Int tilePosition)
    {
        foreach (Vector3Int pos in tilePositions)
        {
            if (pos.x == tilePosition.x && pos.y == tilePosition.y)
            {
                return true;
            }
        }

        return false;
    }
}
