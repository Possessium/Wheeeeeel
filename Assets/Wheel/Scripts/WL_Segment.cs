using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WL_Segment : MonoBehaviour
{
    [SerializeField] UnityEvent clickEvent = null;


    MeshFilter filter = null;
    MeshRenderer rend = null;


    int index = 0;
    int size = 0;
    int cuts = 0;

    private void OnMouseDown()
    {
        clickEvent?.Invoke();
    }






    public void InitSight(int _index, int _size, int _cuts)
    {
        gameObject.AddComponent<MeshCollider>();
        filter = gameObject.AddComponent<MeshFilter>();
        rend = gameObject.AddComponent<MeshRenderer>();
        cuts = _cuts;
        index = _index;
        size = _size;
        DrawMesh();
    }


    void DrawMesh()
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
        rend.material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }



}
