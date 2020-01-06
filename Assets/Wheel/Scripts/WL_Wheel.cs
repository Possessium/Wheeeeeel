using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WL_Wheel : MonoBehaviour
{
    [SerializeField] int cuts = 4;
    [SerializeField] int size = 5;



    private void Start()
    {
        InitWheel();
    }


    void InitWheel()
    {
        for (int i = 0; i < cuts; i++)
        {
            GameObject _go = new GameObject($"Segment {i}");
            _go.transform.parent = transform;
            _go.transform.position = transform.position;
            WL_Segment _s = _go.AddComponent<WL_Segment>();
            _s.InitSight(i, size, cuts);
        }
    }









}
