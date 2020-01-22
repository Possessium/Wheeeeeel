using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WL_Segment : MonoBehaviour
{
    [SerializeField] UnityEvent clickEvent = null;

    Color startColor = Color.black;

    public bool SegmentActive { get; private set; } = false;

    MeshFilter filter = null;
    MeshRenderer rend = null;
    WL_Wheel wheel = null;

    public float StartAngle { get; private set; } = 0;
    public float EndAngle { get; private set; } = 0;
    public int Index { get; private set; } = 0;
    float size = 0;
    int cuts = 0;
    public bool MouseSelected { get; private set; } = false;

    private void OnMouseDown()
    {
        clickEvent?.Invoke();
    }

    private void OnMouseEnter()
    {
        MouseSelected = true;
        wheel.ClearSegments();
        Activate(true);
    }

    private void OnMouseExit()
    {
        MouseSelected = false;
        rend.material.color = startColor;
        Activate(false);
    }



    public void InitSight(int _index, float _size, int _cuts, WL_Wheel _wheel)
    {
        wheel = _wheel;
        transform.localScale = Vector3.one;
        gameObject.layer = 5;
        gameObject.AddComponent<CanvasRenderer>();
        gameObject.AddComponent<MeshCollider>();
        filter = gameObject.AddComponent<MeshFilter>();
        rend = gameObject.AddComponent<MeshRenderer>();
        cuts = _cuts;
        Index = _index;
        size = _size;
        DrawMesh();
    }

    public void Activate(bool _state)
    {
        SegmentActive = _state;
        if(_state)
        {
            rend.material.color = wheel.HighlighColor;
        }
        else
        {
            rend.material.color = startColor;
        }
    }

    public void DrawMesh()
    {
        float _start = Index * (360f / cuts);
        float _end = _start + (360f / cuts);
        StartAngle = _start;
        EndAngle = _end;
        List<Vector3> _vertices = new List<Vector3>();
        List<int> _triangles = new List<int>();

        // Triangles de base

        Vector3 _startPos, _midPos, _endPos;

        _vertices.Add(Vector3.zero);
        float _angleStart = (90 - _start);
        _startPos = new Vector3(Mathf.Cos(_angleStart * Mathf.Deg2Rad), Mathf.Sin(_angleStart * Mathf.Deg2Rad)) * size;
        float _angleEnd = (90 - _end);
        _endPos = new Vector3(Mathf.Cos(_angleEnd * Mathf.Deg2Rad), Mathf.Sin(_angleEnd * Mathf.Deg2Rad)) * size;

        _midPos = (_startPos + _endPos) / 2;

        _vertices.Add(_startPos);
        _vertices.Add(_midPos);
        _vertices.Add(Vector3.zero);
        _vertices.Add(_midPos);
        _vertices.Add(_endPos);

        _triangles.Add(0);
        _triangles.Add(1);
        _triangles.Add(2);
        _triangles.Add(3);
        _triangles.Add(4);
        _triangles.Add(5);

        //

        // Segment

        float _angle = _start;
        float _arcLength = _end - _start;
        for (int i = 0; i <= 360/cuts; i++)
        {
            float _x = Mathf.Sin(Mathf.Deg2Rad * _angle) * size;
            float _y = Mathf.Cos(Mathf.Deg2Rad * _angle) * size;

            _vertices.Add(new Vector3(_x, _y));

            _angle += (_arcLength / (360 / cuts));
        }

        _triangles.Add(2);
        _triangles.Add(1);
        _triangles.Add(8);

        for (int i = 7; i < _vertices.Count - 2; i++)
        {
            _triangles.Add(2);
            _triangles.Add(i + 1);
            _triangles.Add(i + 2);
        }
        //

        Mesh _mesh = new Mesh();
        _mesh.name = $"Segment {Index}  mesh";
        _mesh.vertices = _vertices.ToArray();
        _mesh.triangles = _triangles.ToArray();
        _mesh.RecalculateTangents();
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
        filter.mesh = _mesh;
        GetComponent<MeshCollider>().sharedMesh = _mesh;


        if (!wheel.UseColor)
        {
            rend.material = wheel.MultipleMat ? (wheel.AllMats.Count > Index ? wheel.AllMats[Index] : rend.material) : wheel.SingleMat ? wheel.SingleMat : rend.material;
            startColor = rend.material.color;
        }
        else
        {
            rend.material = default;
            startColor = wheel.MultipleColor ? (wheel.AllColors.Count > Index ? wheel.AllColors[Index] : rend.material.color) : wheel.SingleColor;
        }
    }

}
