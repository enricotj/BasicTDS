using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Cave
{
    private static CellType[,] grid;
    private static int caveWidth = 20;
    private static int caveHeight = 20;

    // cave generation variables
    private static float fillRatio = 0.435f;
    private const int NUM_STEPS = 25;
    private const int DEATH_LIM = 3;
    private const int BIRTH_LIM = 4;
    private const int REVIVE_NUM = 3;
    private static bool pointy = false;

    // de-cheesing variables
    private static CellType[,] pgrid;
    private static ArrayList fill = new ArrayList();
    private static int cheeseThresh;

    private static List<GridPoint> empty = new List<GridPoint>();

    private static System.Random rng;

    public static bool IsFilled(float x, float y)
    {
        int cell = (int)grid[(int)x, (int)y];
        return (cell >= 6 || cell == 1);
    }

    public static CellType GetEffectiveCell(float x, float y)
    {
        float col = Mathf.Floor(x);
        float row = Mathf.Floor(y);
        bool isInEffectiveCell = false;
        const float offset = 0.5f;
        switch (grid[(int)col, (int)row])
        {
            case CellType.SlantNE:
                isInEffectiveCell = (y < row + 1 + offset);
                break;
            case CellType.SlantNW:
                isInEffectiveCell = (x > col - offset);
                break;
            case CellType.SlantSE:
                isInEffectiveCell = (y > row - offset);
                break;
            case CellType.SlantSW:
                isInEffectiveCell = (x < col + offset);
                break;
            default:
                isInEffectiveCell = true;
                break;
        }
        
        return isInEffectiveCell ? grid[(int)col, (int)row] : CellType.Empty;
    }

    public static Vector2 GetNormal(float x, float y)
    {
        switch (GetEffectiveCell(x, y))
        {
            case CellType.SlantNE:
                return (new Vector2(1, 1)).normalized;
            case CellType.SlantNW:
                return (new Vector2(-1, 1)).normalized;
            case CellType.SlantSE:
                return (new Vector2(1, -1)).normalized;
            case CellType.SlantSW:
                return (new Vector2(-1, -1)).normalized;
            case CellType.EdgeE:
                return Vector2.right;
            case CellType.EdgeN:
                return Vector2.up;
            case CellType.EdgeW:
                return Vector2.left;
            case CellType.EdgeS:
                return Vector2.down;
            default:
                return Vector2.zero;
        }
    }

    public static CellType[,] Generate(int width, int height, bool angular, bool empty)
    {
        pointy = angular;
        caveWidth = width;
        caveHeight = height;
        rng = new System.Random(23948);
        //fillRatio = (float)(rng.NextDouble() * 0.04 + 0.44);
        cheeseThresh = (int)Math.Floor(width * height / 6.0f);
        grid = new CellType[width, height];

        if (!empty)
        {
            // randomize grid
            for (int c = 0; c < caveWidth; c++)
            {
                for (int r = 0; r < caveHeight; r++)
                {
                    if (rng.NextDouble() < fillRatio)
                    {
                        grid[c, r] = CellType.Filled;
                    }
                    else
                    {
                        grid[c, r] = CellType.Empty;
                    }
                }
            }

            // generate cave level
            for (int i = 0; i < NUM_STEPS; i++)
            {
                GenerateCave();
            }
        }

        //fill borders
        for (var r = 0; r < caveHeight; r++)
        {
            grid[0, r] = CellType.Filled;
            grid[caveWidth - 1, r] = CellType.Filled;
        }
        for (var c = 0; c < caveWidth; c++)
        {
            grid[c, 0] = CellType.Filled;
            grid[c, caveHeight - 1] = CellType.Filled;
        }

        if (!empty)
        {
            // de-cheese the cave (i.e. fill smaller, unreachable caverns)
            pgrid = new CellType[caveWidth, caveHeight];
            pgrid = (CellType[,])grid.Clone();
            DeCheeseCave();
        }

        // Find/Set Slants/Edges
        FindSlants();
        FindEdges();

        return grid;
    }

    public static GridPoint GetSpawnPoint()
    {
        int r = rng.Next(empty.Count);
        GridPoint gp = empty[r];
        empty.RemoveAt(r);
        return gp;
    }

    #region Generator
    private static void GenerateCave()
    {
        CellType[,] newGrid = new CellType[caveWidth, caveHeight];
        for (int r = 0; r < caveHeight; r++)
        {
            for (int c = 0; c < caveWidth; c++)
            {
                float nbs = CountAliveNeighbors(r, c);
                if (grid[c, r] == CellType.Filled)
                {
                    if (nbs < DEATH_LIM)
                    {
                        newGrid[c, r] = CellType.Empty;
                    }
                    else
                    {
                        newGrid[c, r] = CellType.Filled;
                    }
                }
                else
                {
                    if (nbs > BIRTH_LIM)
                    {
                        newGrid[c, r] = CellType.Filled;
                    }
                    else
                    {
                        newGrid[c, r] = CellType.Empty;
                    }
                }
            }
        }
        for (int r = 0; r < caveHeight; r++)
        {
            for (int c = 0; c < caveWidth; c++)
            {
                grid[c, r] = newGrid[c, r];
            }
        }
    }

    private static void DeCheeseCave()
    {
        for (int r = 1; r < caveHeight - 1; r++)
        {
            for (int c = 1; c < caveWidth - 1; c++)
            {
                if (pgrid[c, r] == CellType.Empty)
                {
                    if (GetCavernSize(r, c) > cheeseThresh)
                    {
                        for (var i = 0; i < fill.Count; i++)
                        {
                            GridPoint p = (GridPoint)fill[i];
                            grid[p.column, p.row] = CellType.Empty;
                        }
                    }
                }
                fill.Clear();
            }
        }
    }

    private static int GetCavernSize(int r, int c)
    {
        int i = 0;
        Queue q = new Queue();
        q.Enqueue(new GridPoint(r, c));
        while (q.Count > 0)
        {
            GridPoint p = (GridPoint)q.Dequeue();
            int row = p.row;
            int col = p.column;
            if (pgrid[col, row] == CellType.Empty)
            {
                i++;
                fill.Add(new GridPoint(row, col));
                grid[col, row] = CellType.Filled;
                pgrid[col, row] = CellType.Filled;
                q.Enqueue(new GridPoint(row, col + 1));
                q.Enqueue(new GridPoint(row - 1, col));
                q.Enqueue(new GridPoint(row, col - 1));
                q.Enqueue(new GridPoint(row + 1, col));
            }
        }
        return i;
    }

    private static void FindSlants()
    {
        for (int r = 0; r < caveHeight; r++)
        {
            for (int c = 0; c < caveWidth; c++)
            {
                CellType check = pointy ? CellType.Filled : CellType.Empty;
                if (grid[c, r] == check)
                {
                    SetSlant(r, c);
                }
            }
        }
    }

    private static void FindEdges()
    {
        for (int r = 0; r < caveHeight; r++)
        {
            for (int c = 0; c < caveWidth; c++)
            {
                CellType cell = grid[c, r];
                if (cell == CellType.Filled)
                {
                    SetEdge(r, c);
                }
                else if (cell == CellType.Empty)
                {
                    // add to empty list
                    empty.Add(new GridPoint(r, c));
                }
            }
        }
    }

    private static bool SetSlant(int r, int c)
    {
        if (CountAliveNeighborsInt(r, c) >= 2)
        {
            if (c > 0 && r > 0 && c < caveWidth - 1 && r < caveHeight - 1 &&
                !CellIsFilled(c, r, Neighbor.East) && 
                !CellIsFilled(c, r, Neighbor.North) &&
                CellIsFilled(c, r, Neighbor.South) &&
                CellIsFilled(c, r, Neighbor.West))
            {
                grid[c, r] = CellType.SlantNE;
                return true;
            }
            else if (c > 0 && r > 0 && c < caveWidth - 1 && r < caveHeight - 1 &&
                !CellIsFilled(c, r, Neighbor.East) &&
                !CellIsFilled(c, r, Neighbor.South) &&
                CellIsFilled(c, r, Neighbor.North) &&
                CellIsFilled(c, r, Neighbor.West))
            {
                grid[c, r] = CellType.SlantSE;
                return true;
            }
            else if (c > 0 && r > 0 && c < caveWidth - 1 && r < caveHeight - 1 &&
                !CellIsFilled(c, r, Neighbor.West) &&
                !CellIsFilled(c, r, Neighbor.South) &&
                CellIsFilled(c, r, Neighbor.North) &&
                CellIsFilled(c, r, Neighbor.East))
            {
                grid[c, r] = CellType.SlantSW;
                return true;
            }
            else if (c > 0 && r > 0 && c < caveWidth - 1 && r < caveHeight - 1 &&
                !CellIsFilled(c, r, Neighbor.West) &&
                !CellIsFilled(c, r, Neighbor.North) &&
                CellIsFilled(c, r, Neighbor.South) &&
                CellIsFilled(c, r, Neighbor.East))
            {
                grid[c, r] = CellType.SlantNW;
                return true;
            }
        }
        return false;
    }

    private static bool CellIsFilled(int c, int r, Neighbor neighbor)
    {
        CellType cell = CellType.Empty;
        switch (neighbor)
        {
            case Neighbor.North:
                cell = grid[c, r + 1];
                break;
            case Neighbor.West:
                cell = grid[c - 1, r];
                break;
            case Neighbor.South:
                cell = grid[c, r - 1];
                break;
            case Neighbor.East:
                cell = grid[c + 1, r];
                break;
        }

        bool treatEmpty = false;
        switch (cell)
        {
            case CellType.SlantNE:
                treatEmpty = (neighbor == Neighbor.South || neighbor == Neighbor.West);
                break;
            case CellType.SlantNW:
                treatEmpty = (neighbor == Neighbor.South || neighbor == Neighbor.East);
                break;
            case CellType.SlantSW:
                treatEmpty = (neighbor == Neighbor.North || neighbor == Neighbor.East);
                break;
            case CellType.SlantSE:
                treatEmpty = (neighbor == Neighbor.North || neighbor == Neighbor.West);
                break;
            default:
                break;
        }

        if (pointy)
        {
            return ((int)cell >= 1 && (int)cell <= 5) && !treatEmpty;
        }
        else
        {
            return !(treatEmpty || ((int)cell == 0));
        }
    }

    private static bool SetEdge(int r, int c)
    {
        if (c < caveWidth - 1 && grid[c + 1, r] == CellType.Empty)
        {
            grid[c, r] = CellType.EdgeE;
            return true;
        }
        if (r > 0 && grid[c, r - 1] == CellType.Empty)
        {
            grid[c, r] = CellType.EdgeS;
            return true;
        }
        if (c > 0 && grid[c - 1, r] == CellType.Empty)
        {
            grid[c, r] = CellType.EdgeW;
            return true;
        }
        if (r < caveHeight - 1 && grid[c, r + 1] == CellType.Empty)
        {
            grid[c, r] = CellType.EdgeN;
            return true;
        }
        return false;
    }

    private static float CountAliveNeighbors(int r, int c)
    {
        float count = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int k = -1; k <= 1; k++)
            {
                int nr = r + i;
                int nc = c + k;
                float val = 1.0f;
                if (Math.Abs(i) == 1 && Math.Abs(k) == 1)
                {
                    val = 0.7f;
                }
                if (!(i == 0 && k == 0))
                {
                    if (nr < 0 || nc < 0 || nr >= caveHeight || nc >= caveWidth)
                    {
                        count += val;
                    }
                    else if (grid[nc, nr] == CellType.Filled)
                    {
                        count += val;
                    }
                }
            }
        }
        return count;
    }

    private static int CountAliveNeighborsInt(int r, int c)
    {
        int count = 0;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                int nr = r + i;
                int nc = c + j;
                if (!(i == 0 && j == 0))
                {
                    if (nr < 0 || nc < 0 || nr >= caveHeight || nc >= caveWidth)
                    {
                        count++;
                    }
                    else if (grid[nc, nr] == CellType.Filled)
                    {
                        count++;
                    }
                }
            }
        }

        return count;
    }
    #endregion

}

public enum CellType
{
    Empty = 0,
    Filled = 1,
    SlantNE = 2,
    SlantNW = 3,
    SlantSW = 4,
    SlantSE = 5,
    EdgeE = 6,
    EdgeN = 7,
    EdgeW = 8,
    EdgeS = 9
}

public enum Neighbor
{
    North,
    South,
    East,
    West
}

public class GridPoint
{
    public int row;
    public int column;

    public GridPoint(int row, int col)
    {
        this.row = row;
        this.column = col;
    }
}