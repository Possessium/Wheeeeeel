using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class WL_Segment : MonoBehaviour
{
    [SerializeField] UnityEvent clickEvent = null;

    Color startColor = Color.white;

    public bool SegmentActive { get; private set; } = false;

    Image rend = null;
    WL_Wheel wheel = null;

    public float StartAngle { get; private set; } = 0;
    public float EndAngle { get; private set; } = 0;
    public int Index { get; private set; } = 0;
    float size = 0;
    int cuts = 0;
    float angle = 0;
    public bool MouseSelected { get; private set; } = false;

    Image childImage = null;

    public bool IsReady { get; private set; } = false;

    private void OnMouseDown()
    {
        clickEvent?.Invoke();
    }

    private void OnMouseEnter()
    {
        MouseSelected = true;
        wheel.ClearSegments();
        Activate(true);
    }

    private void OnMouseExit()
    {
        MouseSelected = false;
        rend.material.color = startColor;
        Activate(false);
    }

    private void OnDestroy()
    {
        if (childImage) Destroy(childImage.gameObject);
    }

    public void InitSight(int _index, float _size, int _cuts, WL_Wheel _wheel)
    {
        wheel = _wheel;
        transform.localScale = Vector3.one;
        gameObject.layer = 5;
        gameObject.AddComponent<CanvasRenderer>();
        cuts = _cuts;
        Index = _index;
        size = _size;
        DrawSegment();
    }

    public void Activate(bool _state)
    {
        SegmentActive = _state;
        if(_state)
        {
            childImage.color = wheel.HighlighColor;
        }
        else
        {
            childImage.color = startColor;
        }
    }

    public void UpdateSegment()
    {
        if (childImage) childImage.transform.localEulerAngles = new Vector3(0, 0, -transform.parent.eulerAngles.z);
    }

    public void UpdateSegment(WL_Segment _nextSegment)
    {
        if(childImage)
        {
            Vector3 _dir = (Vector3.zero + transform.up + _nextSegment.transform.up) / 3;
            childImage.transform.localPosition = (_dir * wheel.Size) * 10;
        }
    }
    
    public void DrawSegment()
    {
        angle = 360f / cuts;
        float _start = (Index * angle) - transform.parent.eulerAngles.z;
        if(_start < 0)
        {
            _start = 360 - transform.parent.eulerAngles.z;
        }
        float _end = _start + angle;
        if(_end > 360)
        {
            _end = _end - 360;
        }
        StartAngle = _start;
        EndAngle = _end;

        transform.eulerAngles = new Vector3(0, 0, -angle * Index);

        GameObject _go = new GameObject($"Image of {name}", typeof(Image));
        _go.transform.SetParent(wheel.transform);
        childImage = _go.GetComponent<Image>();
        childImage.sprite = wheel.AllImages.Count > Index ? wheel.AllImages[Index] : default;
        childImage.name = $"{StartAngle} - {EndAngle}";
        IsReady = true;
        wheel.CheckPos();
    }
}
