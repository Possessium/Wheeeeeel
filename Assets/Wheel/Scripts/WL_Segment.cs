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

    private void OnDrawGizmos()
    {
        
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
        Vector3[] _vertices = new Vector3[360 / cuts + 9];
        int[] _triangles = new int[360 / cuts * 3 + 9];

        // Triangles de base

        Vector3 _startPos, _midPos, _endPos;

        _vertices[0] = Vector3.zero;
        _vertices[3] = Vector3.zero;
        float _angleStart = (90 - _start);
        _startPos = new Vector3(Mathf.Cos(_angleStart * Mathf.Deg2Rad), Mathf.Sin(_angleStart * Mathf.Deg2Rad)) * size;
        float _angleEnd = (90 - _end);
        _endPos = new Vector3(Mathf.Cos(_angleEnd * Mathf.Deg2Rad), Mathf.Sin(_angleEnd * Mathf.Deg2Rad)) * size;

        _midPos = (_startPos + _endPos) / 2;

        _vertices[1] = _startPos;
        _vertices[2] = _midPos;
        _vertices[4] = _midPos;
        _vertices[5] = _endPos;

        _triangles[0] = 0;
        _triangles[1] = 1;
        _triangles[2] = 2;
        _triangles[3] = 3;
        _triangles[4] = 4;
        _triangles[5] = 5;

        //

        // Segment

        Vector3[] _pos = new Vector3[360 / cuts + 1];

        Vector3 p0 = _vertices[1], p1 = _vertices[1] + _vertices[5], p2 = _vertices[5];

        float _pouet = 360f / cuts;

        for (int i = 0; i < _pouet + 1; i++)
        {
            _pos[i] = ((1 - (i / _pouet)) * (1 - (i / _pouet))) * p0 + 2 * (1 - (i / _pouet)) * (i / _pouet) * p1 + ((i / _pouet) * (i / _pouet)) * p2;
        }

        for (int i = 0; i < _pos.Length - 3; i+=3)
        {
            _vertices[i + 6] = _midPos;
            if (i == 0) _vertices[i + 7] = _vertices[1];
            else _vertices[i + 7] = _vertices[i + 6 - 1];
            _vertices[i + 8] = _pos[i + 2];
            _triangles[i + 6] = i + 6;
            _triangles[i + 7] = i + 7;
            _triangles[i + 8] = i + 8;
        }

        _vertices[_vertices.Length - 3] = _midPos;
        _vertices[_vertices.Length - 2] = _vertices[_vertices.Length - 4];
        _vertices[_vertices.Length - 1] = _endPos;

        //

        Mesh _mesh = new Mesh();
        _mesh.name = $"Segment {index}  mesh";
        _mesh.vertices = _vertices;
        _mesh.triangles = _triangles;
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
 