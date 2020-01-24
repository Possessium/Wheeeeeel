using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class WL_Wheel : MonoBehaviour
{
    public Material DefaultMat = null;

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
    //float lastMousePos = 0;

    public Sprite SegmentSprite = null;

    Vector3 joystickPos = Vector3.zero;
    [SerializeField] GameObject merdeBordelCulFesse = null;

    private void Start()
    {
        transform.localScale = Vector3.one * Size;
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
        if (Input.GetKey(KeyCode.LeftArrow)) transform.eulerAngles += transform.forward;
        if (Input.GetKey(KeyCode.RightArrow)) transform.eulerAngles -= transform.forward;
        if (merdeBordelCulFesse) merdeBordelCulFesse.transform.localPosition = joystickPos;

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
        if(!JoystickAngle()) MouseAngle();
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
        float _x = Input.GetAxis("Wheel Horizontal"), _y = Input.GetAxis("Wheel Vertical");
        joystickPos = new Vector3(_x, -_y) * (Size*10);
        if (_x > -.01f && _x < .01f && _y > -.01f && _y < .01f)
        {
            ClearSegments();
            return false;
        }
        float _angle = Mathf.Asin(_y) * Mathf.Rad2Deg + 90;
        if (_x < 0)
        {
            _angle += 180;
            float _diff = 360 - _angle;
            _angle = 180 + _diff;
        }
        _angle -= Mathf.CeilToInt(transform.rotation.z < 0 ? -transform.rotation.z : transform.rotation.z);
        SetSegmentActive(_angle);
        return true;
    }

    void MouseAngle()
    {
        Vector2 _mouse = Input.mousePosition;
        if (_mouse.x < 0 || _mouse.x > Screen.width || _mouse.y < 0 || _mouse.y > Screen.height) return;
        _mouse = new Vector2(-(_mouse.x - (Screen.width / 2)), _mouse.y - (Screen.height / 2));
        float _angle = Vector3.SignedAngle(Vector3.up, _mouse, Vector3.forward);
        if(_angle < 0)
        {
            float _diff = -180 - _angle;
            _angle = 180 - _diff;
        }
        SetSegmentActive(_angle);
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
        Debug.Log(_angle);
        foreach (WL_Segment _s in AllSegments)
        {
            _s.Activate(false);
            if (_angle >= _s.StartAngle && _angle < _s.EndAngle) _s.Activate(true);
        }
        if (AllSegments.Any(s => s.SegmentActive)) return;
        else AllSegments[0].Activate(true);
    }
}
