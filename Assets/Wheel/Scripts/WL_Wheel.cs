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

    public string JoystickAxisHorizontal = "Wheel Horizontal";
    public string JoystickAxisVertical = "Wheel Vertical";


    public bool UseColor = true;
    public bool UseMaterial = false;
    public bool UseImage = false;
    public bool MultipleMat = false;
    public bool MultipleColor = false;
    public bool MultipleImages = false;

    public Material SingleMat = null;
    public Sprite SingleImage = null;
    public Color SingleColor = Color.black;
    public Color HighlighColor = Color.red;

    public List<Material> AllMats = new List<Material>();
    public List<UnityEvent> AllEvents = new List<UnityEvent>();
    public List<Color> AllColors = new List<Color>();
    public List<Sprite> AllImages = new List<Sprite>();

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

    void ResetRotation()
    {
        AllSegments.ForEach(g => g.UpdateSegment());
    }

    public void CheckPos()
    {
        for (int i = 0; i < AllSegments.Count; i++)
        {
            AllSegments[i].UpdateSegment(AllSegments.Count > i + 1 ? AllSegments[i + 1] : AllSegments[0]);
        }
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


    // PAS TOUCHE CA MARCHE BODEL DE FION
    bool JoystickAngle()
    {
        float _x = -Input.GetAxis(JoystickAxisHorizontal), _y = -Input.GetAxis(JoystickAxisVertical);
        joystickPos = new Vector3(-_x, _y) * (Size * 50);
        if (_x > -.01f && _x < .01f && _y > -.01f && _y < .01f)
        {
            ClearSegments();
            return false;
        }
        float _angle = Vector3.SignedAngle(Vector3.up, new Vector3(_x,_y), Vector3.forward);
        _angle += transform.eulerAngles.z;
        if (_angle < 0)
        {
            float _diff = -180 - _angle;
            _angle = 180 - _diff;
        }
        SetSegmentActive(_angle);
        return true;
    }

    void MouseAngle()
    {
        Vector2 _mouse = Input.mousePosition;
        if (_mouse.x < 0 || _mouse.x > Screen.width || _mouse.y < 0 || _mouse.y > Screen.height) return;
        _mouse = new Vector2(-(_mouse.x - (Screen.width / 2)), _mouse.y - (Screen.height / 2));
        float _angle = Vector3.SignedAngle(Vector3.up, _mouse, Vector3.forward);
        _angle += transform.eulerAngles.z;
        if(_angle < 0)
        {
            float _diff = -180 - _angle;
            _angle = 180 - _diff;
        }
        SetSegmentActive(_angle);
    }
    //


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
            if (_angle >= _s.StartAngle && _angle <= _s.EndAngle) _s.Activate(true);
        }
        if (AllSegments.Any(s => s.SegmentActive)) return;
        else AllSegments[0].Activate(true);
    }
}
