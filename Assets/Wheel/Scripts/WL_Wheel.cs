using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class WL_Wheel : MonoBehaviour
{

    public List<WL_Segment> AllSegments = new List<WL_Segment>();

    public int Cuts = 4;
    public float Size = 5;

    public bool UseColor = true;

    public bool MultipleMat = false;
    public bool MultipleColor = false;

    public Material SingleMat = null;
    public Color SingleColor = Color.black;
    public Color HighlighColor = Color.red;

    public List<Material> AllMats = new List<Material>();
    public List<UnityEvent> AllEvents = new List<UnityEvent>();
    public List<Color> AllColors = new List<Color>();

    int oldCuts = 0;
    float oldSize = 0;
    List<Material> oldMats = new List<Material>();
    List<Color> oldColors = new List<Color>();
    float lastMousePos = 0;

    private void Start()
    {
        InitWheel();
        oldCuts = Cuts;
        oldSize = Size;
        for (int i = 0; i < AllMats.Count; i++)
        {
            oldMats.Add(AllMats[i]);
        }
        for (int i = 0; i < AllColors.Count; i++)
        {
            oldColors.Add(AllColors[i]);
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Wheel Submit")) ClickSegment();
        if (Cuts != oldCuts || Size != oldSize)
        {
            InitWheel();
        }
        for (int i = 0; i < AllColors.Count; i++)
        {
            if (oldColors.Count > i) if (AllColors[i] != oldColors[i])
                {
                    InitWheel();
                    break;
                }
        }
        for (int i = 0; i < AllMats.Count; i++)
        {
            if (oldMats.Count > i) if (AllMats[i] != oldMats[i])
                {
                    InitWheel();
                    break;
                }
        }
        float _mousePos = Input.mousePosition.magnitude;
        if (_mousePos == lastMousePos && !AllSegments.Any(s => s.MouseSelected)) JoystickAngle();
        lastMousePos = _mousePos;
        oldCuts = Cuts;
        oldSize = Size;
        if (!UseColor)
        {
            oldMats.Clear();
            for (int i = 0; i < AllMats.Count; i++)
            {
                oldMats.Add(AllMats[i]);
            }
        }
        else
        {
            oldColors.Clear();
            for (int i = 0; i < AllColors.Count; i++)
            {
                oldColors.Add(AllColors[i]);
            }
        }
    }

    void ClickSegment()
    {
        WL_Segment _s = AllSegments.Where(s => s.SegmentActive).FirstOrDefault();
        if (!_s) return;
        if (AllEvents.Count > _s.Index) AllEvents[_s.Index]?.Invoke();
    }

    void InitWheel()
    {
        AllSegments.ForEach(g => Destroy(g.gameObject));
        AllSegments.Clear();
        for (int i = 0; i < Cuts; i++)
        {
            GameObject _go = new GameObject($"Segment {i}", typeof(RectTransform));
            _go.transform.SetParent(transform);
            _go.transform.position = transform.position;
            WL_Segment _s = _go.AddComponent<WL_Segment>();
            AllSegments.Add(_s);
            _s.InitSight(i, Size, Cuts, this);
        }
    }

    void JoystickAngle()
    {
        float _x = Input.GetAxis("Wheel Horizontal"), _y = Input.GetAxis("Wheel Vertical");
        if (_x > -.01f && _x < .01f && _y > -.01f && _y < .01f)
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
        if (_x < -.01f || _x > .01f || _y < -.01f || _y > .01f) SetSegmentActive(_angle);
    }

    public void ClearSegments()
    {
        foreach (WL_Segment _s in AllSegments)
        {
            _s.Activate(false);
        }
    }

    void SetSegmentActive(float _angle)
    {
        foreach (WL_Segment _s in AllSegments)
        {
            _s.Activate(false);
            if (_angle >= _s.StartAngle && _angle < _s.EndAngle) _s.Activate(true);
        }
        if (AllSegments.Any(s => s.SegmentActive)) return;
        else AllSegments[0].Activate(true);
    }
}

/*
 * 
 * KESKY FO 2 PUBLIC ?
 * 
 * events, materials, colors.   DONE
 * 
 * 
 */
