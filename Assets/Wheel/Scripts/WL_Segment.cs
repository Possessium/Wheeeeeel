using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class WL_Segment : MonoBehaviour
{
    [SerializeField] UnityEvent clickEvent = null;

    Color startColor = Color.black;

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
        rend = gameObject.AddComponent<Image>();
        if(rend)
        {
            rend.sprite = wheel.SegmentSprite;
            rend.material = wheel.SingleMat;
            rend.material.color = Color.white;
            rend.type = Image.Type.Filled;
            rend.fillMethod = Image.FillMethod.Radial360;
            rend.fillClockwise = true;
            rend.fillOrigin = 2;
        }
        cuts = _cuts;
        Index = _index;
        size = _size;
        DrawSegment();
        //DrawMesh();
    }

    public void Activate(bool _state)
    {
        SegmentActive = _state;
        if(_state)
        {
            rend.color = wheel.HighlighColor;
            //transform.localPosition += (transform.right * 5) + (transform.up * 5);
        }
        else
        {
            rend.color = startColor;
            //transform.localPosition = Vector3.zero;
        }
    }

    public void UpdateSegment()
    {
        if (childImage) childImage.transform.rotation = new Quaternion(0, 0, -transform.rotation.z, 0);
    }
    public void UpdateSegment(WL_Segment _nextSegment)
    {
        if(childImage)
        {
            childImage.transform.rotation = new Quaternion(0, 0, -transform.rotation.z, 0);
            Vector3 _dir = (Vector3.zero + transform.up + _nextSegment.transform.up) / 3;
            childImage.transform.localPosition = (_dir * wheel.Size) * 10;
        }
    }
    
    public void DrawSegment()
    {
        angle = 360f / cuts;
        float _start = Index * angle;
        float _end = _start + angle;
        StartAngle = _start;
        EndAngle = _end;

        rend.fillAmount = angle / 360;
        transform.eulerAngles = new Vector3(0, 0, -angle * Index);

        if(wheel.UseColor)
        {
            startColor = wheel.MultipleColor ? (Index >= wheel.AllColors.Count ? startColor : wheel.AllColors[Index]) : wheel.SingleColor;
            rend.color = startColor;
            rend.material = wheel.DefaultMat;
        }
        else if(wheel.UseMaterial)
        {
            rend.material = wheel.MultipleMat ? (wheel.AllMats.Count >= Index ? wheel.DefaultMat : wheel.AllMats[Index]) : wheel.SingleMat;
        }
        if(wheel.UseImage)
        {
            if(!wheel.MultipleImages)
            {
                GameObject _go = new GameObject($"Image of {name}", typeof(Image));
                _go.transform.SetParent(wheel.transform);

                //Vector2 _pos = new Vector2(Mathf.Cos(StartAngle * Mathf.Deg2Rad), Mathf.Sin(StartAngle * Mathf.Deg2Rad)) * wheel.Size;
                //Vector2 _pos2 = new Vector2(Mathf.Cos(EndAngle * Mathf.Deg2Rad), Mathf.Sin(EndAngle * Mathf.Deg2Rad)) * wheel.Size;


                //_go.transform.localPosition = (Vector2.zero + _pos + _pos2) / 3;
                //childImage.transform.rotation = new Quaternion(0, 0, -transform.rotation.z, 0);
                childImage = _go.GetComponent<Image>();
                childImage.sprite = wheel.SingleImage;
            }
            else
            {
                GameObject _go = new GameObject($"Image of {name}", typeof(Image));
                //_go.transform.SetParent(this.transform);
                _go.transform.localPosition = transform.forward * 5;
                //childImage.transform.rotation = new Quaternion(0, 0, -transform.rotation.z, 0);
                childImage = _go.GetComponent<Image>();
                childImage.sprite = wheel.AllImages.Count > Index ? wheel.AllImages[Index] : wheel.SingleImage;
            }
        }
        IsReady = true;
        wheel.CheckPos();
    }
}
