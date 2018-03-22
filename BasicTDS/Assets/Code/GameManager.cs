using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public GameObject _playerPrefab;
    public GameObject _enemyPrefab;

    public Tilemap _tileMap;
    public TilemapCollider2D _tileMapCollider;
    public TileBase _tile;

    public int _cols = 100;
    public int _rows = 100;
    public bool _on = true;

    private CellType[,] _grid;

    private List<GameObject> _enemyPool = new List<GameObject>();

    private bool print = true;
    private System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

    private void Awake()
    {
        watch.Start();
        Debug.Log("Awake");
    }

    void Start ()
    {
        if (_on)
        {
            Debug.Log("Start");
            _tileMapCollider.enabled = false;
            _grid = Cave.Generate(_cols, _rows, false /* isAngular */, false /* isEmpty */);
            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _cols; col++)
                {
                    CellType cell = _grid[col, row];
                    if (cell != CellType.Empty)
                    {
                        _tileMap.SetTile(new Vector3Int(col, row, 0), _tile);
                    }
                }
            }

            Debug.Log(watch.ElapsedMilliseconds);
            _tileMapCollider.enabled = true;

            GridPoint gp = Cave.GetSpawnPoint();
            Instantiate(_playerPrefab, new Vector3(gp.column + 0.5f, (gp.row + 0.5f), 10), Quaternion.identity);

            for (int i = 0; i < 10; i++)
            {
                gp = Cave.GetSpawnPoint();
                Instantiate(_enemyPrefab, new Vector3(gp.column + 0.5f, (gp.row + 0.5f), 10), Quaternion.identity);
            }
        }
	}

    public void Update()
    {
        if (print)
        {
            watch.Stop();
            Debug.Log(watch.ElapsedMilliseconds);
            watch.Reset();
            
            print = false;
        }

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    watch.Start();
        //    print = true;
        //    Debug.Log(watch.ElapsedMilliseconds);
        //}
    }

}
