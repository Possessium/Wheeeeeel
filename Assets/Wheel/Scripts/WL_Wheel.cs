using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class WL_Wheel : MonoBehaviour
{
    [SerializeField, Range(1, 180)] int cuts = 4;
    [SerializeField, Range(.1f, 10)] float size = 5;

    [SerializeField] List<Material> allMats = new List<Material>();
        public List<Material> AllMats { get { return allMats; } }
    [SerializeField] List<UnityEvent> allEvents = new List<UnityEvent>();
        public List<UnityEvent> AllEvents { get { return allEvents; } }

    List<WL_Segment> allSegments = new List<WL_Segment>();

    int oldCuts = 0;
    float oldSize = 0;
    float lastMousePos = 0;

    private void Start()
    {
        InitWheel();
        oldCuts = cuts;
        oldSize = size;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Wheel Submit")) ClickSegment();
        if (cuts != oldCuts || size != oldSize)
        {
            InitWheel();
        }
        float _mousePos = Input.mousePosition.magnitude;
        if (_mousePos == lastMousePos && !allSegments.Any(s => s.MouseSelected)) JoystickAngle();
        lastMousePos = _mousePos;
        oldCuts = cuts;
        oldSize = size;
    }

    void ClickSegment()
    {
        WL_Segment _s = allSegments.Where(s => s.SegmentActive).FirstOrDefault();
        if (!_s) return;
        if(allEvents.Count > _s.Index) allEvents[_s.Index]?.Invoke();
    }

    void InitWheel()
    {
        allSegments.ForEach(g => Destroy(g.gameObject));
        allSegments.Clear();
        for (int i = 0; i < cuts; i++)
        {
            GameObject _go = new GameObject($"Segment {i}", typeof(RectTransform));
            _go.transform.SetParent(transform);
            _go.transform.position = transform.position;
            WL_Segment _s = _go.AddComponent<WL_Segment>();
            allSegments.Add(_s);
            _s.InitSight(i, size, cuts, this);
        }
    }

    void JoystickAngle()
    {
        float _x = Input.GetAxis("Wheel Horizontal"), _y = Input.GetAxis("Wheel Vertical");
        if(_x > -.3f && _x < .3f && _y > -.3f && _y < .3f)
        {
            ClearSegments();
            return;
        }
        float _angle = Mathf.Asin(_y) * Mathf.Rad2Deg + 90;
        if (_x < 0)
        {
            _angle += 180;
            float _diff = 360 - _angle;
            _angle = 180 + _diff;
        }
        if (_x < -.3f || _x > .3f || _y < -.3f || _y > .3f) SetSegmentActive(_angle);
    }

    public void ClearSegments()
    {
        foreach (WL_Segment _s in allSegments)
        {
            _s.Activate(false);
        }
    }

    void SetSegmentActive(float _angle)
    {
        foreach(WL_Segment _s in allSegments)
        {
            _s.Activate(false);
            if (_angle >= _s.StartAngle && _angle < _s.EndAngle) _s.Activate(true);
        }
        if (allSegments.Any(s => s.SegmentActive)) return;
        else allSegments[0].Activate(true);
    }
}
