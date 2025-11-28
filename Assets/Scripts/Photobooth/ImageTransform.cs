using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageTransform : MonoBehaviour
{
    public float smoothing = 70f;

    private Vector2 fingerStart;
    private Vector2 fingerEnd;
    private float pinchStart;
    private float pinchEnd;
    private float rotationStart;
    private float rotationEnd;
    private Vector2 initialScale;
    private float scale;
    private float rotation;
    private Collider2D spriteCollider;

    void Start()
    {
        spriteCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

            if (spriteCollider.OverlapPoint(touchPos))
            {
                if (touch.phase == TouchPhase.Began)
                {
                    fingerStart = touchPos;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    fingerEnd = touchPos;
                    Vector2 offset = fingerEnd - fingerStart;
                    Vector3 newPos = new Vector3(transform.position.x + offset.x, transform.position.y + offset.y, transform.position.z);
                    transform.position = Vector3.Lerp(transform.position, newPos, smoothing);
                    fingerStart = fingerEnd;
                }
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);
            Vector2 touch1Pos = Camera.main.ScreenToWorldPoint(touch1.position);
            Vector2 touch2Pos = Camera.main.ScreenToWorldPoint(touch2.position);

            if (spriteCollider.OverlapPoint(touch1Pos) || spriteCollider.OverlapPoint(touch2Pos))
            {
                if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
                {
                    fingerStart = (touch1Pos + touch2Pos) / 2;
                    pinchStart = Vector2.Distance(touch1.position, touch2.position);
                    rotationStart = Angle(touch1.position, touch2.position);
                    initialScale = transform.localScale;
                }
                else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                {
                    pinchEnd = Vector2.Distance(touch1.position, touch2.position);
                    rotationEnd = Angle(touch1.position, touch2.position);
                    scale = Mathf.Clamp(pinchEnd / pinchStart * initialScale.x, 0, 1.3f);
                    rotation = rotationEnd - rotationStart;

                    transform.localScale = new Vector3(scale, scale, scale);
                    transform.rotation = Quaternion.Euler(0, 0, rotation);

                    fingerEnd = (touch1Pos + touch2Pos) / 2;
                    Vector2 offset = fingerEnd - fingerStart;
                    Vector3 newPos = new Vector3(transform.position.x + offset.x, transform.position.y + offset.y, transform.position.z);
                    transform.position = Vector3.Lerp(transform.position, newPos, smoothing);
                    fingerStart = fingerEnd;
                }
            }
        }
    }

    private float Angle(Vector2 pos1, Vector2 pos2)
    {
        Vector2 from = pos2 - pos1;
        Vector2 to = new Vector2(1, 0);

        float result = Vector2.Angle(from, to);
        Vector3 cross = Vector3.Cross(from, to);

        if (cross.z > 0)
        {
            result = 360f - result;
        }

        return result;
    }
}
