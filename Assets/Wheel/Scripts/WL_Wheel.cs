using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class WL_Wheel : MonoBehaviour
{

    List<WL_Segment> allSegments = new List<WL_Segment>();

    #region USER
    public int Cuts = 4;
    public float Size = 5;

    public bool UseColor = true;

    public bool MultipleMat = false;
    public bool MultipleColor = false;

    public Material SingleMat = null;
    public Color SingleColor = Color.black;

    public List<Material> AllMats = new List<Material>();
    public List<UnityEvent> AllEvents = new List<UnityEvent>();
    public List<Color> AllColors = new List<Color>();
    #endregion

    int oldCuts = 0;
    float oldSize = 0;
    float lastMousePos = 0;

    private void Start()
    {
        InitWheel();
        oldCuts = Cuts;
        oldSize = Size;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Wheel Submit")) ClickSegment();
        if (Cuts != oldCuts || Size != oldSize)
        {
            InitWheel();
        }
        float _mousePos = Input.mousePosition.magnitude;
        if (_mousePos == lastMousePos && !allSegments.Any(s => s.MouseSelected)) JoystickAngle();
        lastMousePos = _mousePos;
        oldCuts = Cuts;
        oldSize = Size;
    }

    void ClickSegment()
    {
        WL_Segment _s = allSegments.Where(s => s.SegmentActive).FirstOrDefault();
        if (!_s) return;
        if(AllEvents.Count > _s.Index) AllEvents[_s.Index]?.Invoke();
    }

    void InitWheel()
    {
        allSegments.ForEach(g => Destroy(g.gameObject));
        allSegments.Clear();
        for (int i = 0; i < Cuts; i++)
        {
            GameObject _go = new GameObject($"Segment {i}", typeof(RectTransform));
            _go.transform.SetParent(transform);
            _go.transform.position = transform.position;
            WL_Segment _s = _go.AddComponent<WL_Segment>();
            allSegments.Add(_s);
            _s.InitSight(i, Size, Cuts, this);
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
