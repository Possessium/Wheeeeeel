using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class WL_Wheel : MonoBehaviour
{
    public List<WL_Segment> AllSegments = new List<WL_Segment>();

    public int Cuts = 4;
    public float Size = 5;

    public string JoystickAxisHorizontal = "Horizontal";
    public string JoystickAxisVertical = "Vertical";
    public string JoystickSubmit = "Submit";


    public Color HighlighColor = Color.red;

    public List<UnityEvent> AllEvents = new List<UnityEvent>();
    public List<Sprite> AllImages = new List<Sprite>();

    int oldCuts = 0;
    float oldSize = 0;

    float startZAngle = 0;

    Quaternion oldRot = Quaternion.identity;

    private void Start()
    {
        transform.localScale = Vector3.one * Size;
        startZAngle = transform.eulerAngles.z;
        InitWheel();
        oldCuts = Cuts;
        oldSize = Size;
    }

    private void Update()
    {
        if (oldRot != transform.rotation) ResetRotation();
        oldRot = transform.rotation;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.eulerAngles += transform.forward;
            ResetRotation();
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.eulerAngles -= transform.forward;
            ResetRotation();
        }

        if (Input.GetButtonDown(JoystickSubmit)) ClickSegment();
        if (Cuts != oldCuts || Size != oldSize)
        {
            InitWheel();
        }
        if(!JoystickAngle()) MouseAngle();
        oldCuts = Cuts;
        oldSize = Size;
    }



    void ClickSegment()
    {
        WL_Segment _s = AllSegments.Where(s => s.SegmentActive).FirstOrDefault();
        if (!_s) return;
        if (AllEvents.Count > _s.Index) AllEvents[_s.Index]?.Invoke();
    }

    void ResetRotation()
    {
        AllSegments.ForEach(g => g.UpdateSegment());
    }

    public void CheckPos()
    {
        if (AllSegments.Any(s => !s.IsReady)) return;
        for (int i = 0; i < AllSegments.Count; i++)
        {
            AllSegments[i].UpdateSegment(AllSegments.Count > i + 1 ? AllSegments[i + 1] : AllSegments[0]);
        }
        ResetRotation();
    }

    void InitWheel()
    {
        AllSegments.ForEach(g => Destroy(g.gameObject));
        AllSegments.Clear();
        transform.localScale = Vector3.one * Size;
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


    bool JoystickAngle()
    {
        float _x = -Input.GetAxis(JoystickAxisHorizontal), _y = Input.GetAxis(JoystickAxisVertical);
        if (_x > -.01f && _x < .01f && _y > -.01f && _y < .01f)
        {
            ClearSegments();
            return false;
        }
        float _angle = Vector3.SignedAngle(Vector3.up, new Vector3(_x,_y), Vector3.forward);
        //_angle -= 90;
        _angle += transform.eulerAngles.z;
        if (_angle > 360)
        {
            float _diff = _angle - 360;
            _angle = _diff;
        }
        if (_angle < 0)
        {
            float _diff = -180 - _angle;
            _angle = 180 - _diff;
        }
        SetSegmentActive(_angle, true);
        return true;
    }

    void MouseAngle()
    {
        Vector2 _mouse = Input.mousePosition;
        if (_mouse.x < 0 || _mouse.x > Screen.width || _mouse.y < 0 || _mouse.y > Screen.height) return;
        _mouse = new Vector2(-(_mouse.x - (Screen.width / 2)), _mouse.y - (Screen.height / 2));
        float _angle = Vector3.SignedAngle(Vector3.up, _mouse, Vector3.forward);
        _angle += transform.eulerAngles.z - startZAngle;
        if(_angle > 360)
        {
            float _diff = _angle - 360;
            _angle = _diff;
        }
        if (_angle < 0)
        {
            float _diff = -180 - _angle;
            _angle = 180 - _diff;
        }
        bool _farEnough = Vector3.Distance(_mouse, Vector3.zero) > 50;
        SetSegmentActive(_angle, _farEnough);
    }


    public void ClearSegments()
    {
        foreach (WL_Segment _s in AllSegments)
        {
            _s.Activate(false);
        }
    }

    void SetSegmentActive(float _angle, bool _farEnough)
    {
        foreach (WL_Segment _s in AllSegments)
        {
            _s.Activate(false);
            if (_angle >= _s.StartAngle && _angle <= _s.EndAngle && _farEnough) _s.Activate(true);
        }
        if (AllSegments.Any(s => s.SegmentActive) || !_farEnough) return;
        else AllSegments[0].Activate(true);
    }
}
