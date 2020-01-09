using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WL_Wheel : MonoBehaviour
{
    [SerializeField, Range(1, 360)] int cuts = 4;
    [SerializeField, Range(.1f, 10)] float size = 5;

    List<GameObject> allSegments = new List<GameObject>();

    int oldCuts = 0;
    float oldSize = 0;

    private void Start()
    {
        InitWheel();
        oldCuts = cuts;
        oldSize = size;
        //StartCoroutine(CheckCutsModulo());
    }

    private void Update()
    {
        if (cuts != oldCuts || size != oldSize)
        {
            //StartCoroutine(CheckCutsModulo());
            InitWheel();
        }
        oldCuts = cuts;
        oldSize = size;
    }

    IEnumerator CheckCutsModulo()
    {
        while (360 % cuts != 0)
        {
            if (cuts < 1)
            {
                cuts = 0;
                yield break;
            }
            cuts--;
            //yield return new WaitForSeconds(.01f);
        }
    }

    void InitWheel()
    {
        allSegments.ForEach(g => Destroy(g));
        allSegments.Clear();
        for (int i = 0; i < cuts; i++)
        {
            GameObject _go = new GameObject($"Segment {i}", typeof(RectTransform));
            allSegments.Add(_go);
            _go.transform.SetParent(transform);
            _go.transform.position = transform.position;
            WL_Segment _s = _go.AddComponent<WL_Segment>();
            _s.InitSight(i, size, cuts);
        }
    }

}
