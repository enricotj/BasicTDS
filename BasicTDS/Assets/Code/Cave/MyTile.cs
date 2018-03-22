using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTile : MonoBehaviour
{
    public Sprite _filled;
    public Sprite _slantNE;
    public Sprite _slantNW;
    public Sprite _slantSW;
    public Sprite _slantSE;
    public Sprite _edgeE;
    public Sprite _edgeN;
    public Sprite _edgeW;
    public Sprite _edgeS;

    private SpriteRenderer _spr;
    private PolygonCollider2D _collider;

    public void Init(CellType cellType)
    {
        _collider = GetComponent<PolygonCollider2D>();
        List<Vector2> path = new List<Vector2>();
        path.AddRange(_collider.GetPath(0));

        _spr = GetComponent<SpriteRenderer>();
        Sprite sprite = _filled;

        switch (cellType)
        {
            case CellType.Filled:
                sprite = _filled;
                Destroy(_collider);
                break;
            case CellType.SlantNE:
                sprite = _slantNE;
                path.RemoveAt(0);
                break;
            case CellType.SlantNW:
                sprite = _slantNW;
                path.RemoveAt(1);
                break;
            case CellType.SlantSW:
                sprite = _slantSW;
                path.RemoveAt(2);
                break;
            case CellType.SlantSE:
                sprite = _slantSE;
                path.RemoveAt(3);
                break;
            case CellType.EdgeE:
                sprite = _edgeE;
                Destroy(_collider);
                break;
            case CellType.EdgeN:
                sprite = _edgeN;
                Destroy(_collider);
                break;
            case CellType.EdgeW:
                sprite = _edgeW;
                Destroy(_collider);
                break;
            case CellType.EdgeS:
                sprite = _edgeS;
                Destroy(_collider);
                break;
            default:
                break;
        }

        _collider.SetPath(0, path.ToArray());
        _spr.sprite = sprite;

        //Destroy(_collider);
    }

    //public void Init(CellType cellType)
    //{
    //    _collider = GetComponent<PolygonCollider2D>();
    //    List<Vector2> path = new List<Vector2>();
    //    path.AddRange(_collider.GetPath(0));

    //    _spr = GetComponent<SpriteRenderer>();
    //    Sprite sprite = _filled;

    //    switch (cellType)
    //    {
    //        case CellType.Filled:
    //            sprite = _filled;
    //            Destroy(_collider);
    //            break;
    //        case CellType.SlantNE:
    //            sprite = _slantNE;
    //            path.RemoveAt(0);
    //            break;
    //        case CellType.SlantNW:
    //            sprite = _slantNW;
    //            path.RemoveAt(1);
    //            break;
    //        case CellType.SlantSW:
    //            sprite = _slantSW;
    //            path.RemoveAt(2);
    //            break;
    //        case CellType.SlantSE:
    //            sprite = _slantSE;
    //            path.RemoveAt(3);
    //            break;
    //        case CellType.EdgeE:
    //            sprite = _edgeE;
    //            Destroy(_collider);
    //            break;
    //        case CellType.EdgeN:
    //            sprite = _edgeN;
    //            Destroy(_collider);
    //            break;
    //        case CellType.EdgeW:
    //            sprite = _edgeW;
    //            Destroy(_collider);
    //            break;
    //        case CellType.EdgeS:
    //            sprite = _edgeS;
    //            Destroy(_collider);
    //            break;
    //        default:
    //            break;
    //    }

    //    _collider.SetPath(0, path.ToArray());
    //    _spr.sprite = sprite;

    //    //Destroy(_collider);
    //}
}
