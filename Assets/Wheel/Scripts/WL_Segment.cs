using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WL_Segment : MonoBehaviour
{
    [SerializeField] UnityEvent clickEvent = null;

    Color startColor = Color.black;


    MeshCollider collider = null;
    MeshFilter filter = null;
    MeshRenderer rend = null;


    int index = 0;
    float size = 0;
    int cuts = 0;

    private void OnMouseDown()
    {
        clickEvent?.Invoke();
    }

    private void OnMouseEnter()
    {
        rend.material.color = Color.red;
    }

    private void OnMouseExit()
    {
        rend.material.color = startColor;
    }



    public void InitSight(int _index, float _size, int _cuts)
    {
        transform.localScale = Vector3.one;
        gameObject.layer = 5;
        gameObject.AddComponent<CanvasRenderer>();
        collider = gameObject.AddComponent<MeshCollider>();
        filter = gameObject.AddComponent<MeshFilter>();
        rend = gameObject.AddComponent<MeshRenderer>();
        cuts = _cuts;
        index = _index;
        size = _size;
        DrawMesh();
        //DrawMeshOLD();
    }


    void DrawMeshOLD()
    {
        int _start = index * (360 / cuts);
        int _end = _start + (360 / cuts);
        int _angle = _end - _start;
        int _vertices = _angle + 2;
        Vector3[] _verticesPosition = new Vector3[_vertices];
        int[] _triangles = new int[(_vertices - 2) * 3];
        int _index = 0;
        for (int i = _start; i < _end + 1; i++)
        {
            _verticesPosition[_index + 1] = transform.InverseTransformDirection((Quaternion.Euler(0, 0, -i) * Vector3.down).normalized * size);
            if (_index < _vertices - 2)
            {
                _triangles[_index * 3] = 0;
                _triangles[_index * 3 + 1] = _index + 1;
                _triangles[_index * 3 + 2] = _index + 2;
            }
            _index++;
        }
        Mesh _mesh = new Mesh();
        _mesh.vertices = _verticesPosition;
        _mesh.triangles = _triangles;
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
        filter.mesh = _mesh;
        GetComponent<MeshCollider>().sharedMesh = _mesh;
        startColor = rend.material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    void DrawMesh()
    {
        float _start = index * (360f / cuts);
        float _end = _start + (360f / cuts);
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

        //List<Vector3> _arcPoints = new List<Vector3>();
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

        //int _index = 0;

        for (int i = 7; i < _vertices.Count - 2; i++)
        {
            _triangles.Add(2);
            _triangles.Add(i + 1);
            _triangles.Add(i + 2);
            //if (i % 3 == 0) _index++;
        }

        //for (int i = 0; i < _pos.Length - 3; i+=3)
        //{
        //    _vertices[i + 6] = _midPos;
        //    if (i == 0) _vertices[i + 7] = _vertices[1];
        //    else _vertices[i + 7] = _vertices[i + 6 - 1];
        //    _vertices[i + 8] = _pos[i + 2];
        //    _triangles[i + 6] = i + 6;
        //    _triangles[i + 7] = i + 7;
        //    _triangles[i + 8] = i + 8;
        //}

        //

        Mesh _mesh = new Mesh();
        _mesh.name = $"Segment {index}  mesh";
        _mesh.vertices = _vertices.ToArray();
        _mesh.triangles = _triangles.ToArray();
        _mesh.RecalculateTangents();
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
        filter.mesh = _mesh;
        GetComponent<MeshCollider>().sharedMesh = _mesh;

        startColor = rend.material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        // debug
        //rend.enabled = false;
    }

}

/*
 * calculer l'angle
 * faire un triangle simple du start à l'end
 * rajouter le segment manquant
 * done.
 */
 