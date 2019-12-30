using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MarkerType { None, Circle, Cross }

public class Cell : MonoBehaviour
{
    [SerializeField] SpriteRenderer markerSpriteRenderer;

    [SerializeField] Sprite circleMarkerSprite;
    [SerializeField] Sprite crossMarkerSprite;

<<<<<<< HEAD
    public int index;
    public Transform leftPos;
    public Transform rightPos;
    public Transform upPos;
    public Transform downPos;
    public Transform leftUpPos;
    public Transform leftDownPos;
    public Transform rightUpPos;
    public Transform rightDownPos;
=======
    BoxCollider2D cachedBoxCollider2D;
    public BoxCollider2D CachedBoxCollider2D
    {
        get
        {
            if (!cachedBoxCollider2D)
                cachedBoxCollider2D = GetComponent<BoxCollider2D>();
            return cachedBoxCollider2D;
        }
    }

    private SpriteRenderer cachedSpriteRenderer;
    public SpriteRenderer CachedSpriteRenderer
    {
        get 
        {
            if (!cachedSpriteRenderer)
                cachedSpriteRenderer = GetComponent<SpriteRenderer>();
            return cachedSpriteRenderer;
        }
    }
>>>>>>> 60dc553554f433e6d2519f1b46bfee1d2f3c71c5

    private MarkerType markerType;
    public MarkerType MarkerType
    {
        get
        {
            return markerType;
        }
        set
        {
<<<<<<< HEAD
=======
            if (markerType != MarkerType.None) return;

>>>>>>> 60dc553554f433e6d2519f1b46bfee1d2f3c71c5
            switch (value)
            {
                case MarkerType.None:
                    markerSpriteRenderer.sprite = null;
                    break;
                case MarkerType.Circle:
                    markerSpriteRenderer.sprite = circleMarkerSprite;
                    break;
                case MarkerType.Cross:
                    markerSpriteRenderer.sprite = crossMarkerSprite;
                    break;
            }
            markerType = value;
        }
    }

<<<<<<< HEAD
    private void Update()
    {
        Debug.DrawRay(leftPos.position, Vector2.left, Color.green);
        Debug.DrawRay(rightPos.position, Vector2.right, Color.green);
        Debug.DrawRay(upPos.position, Vector2.up, Color.green);
        Debug.DrawRay(downPos.position, Vector2.down, Color.green);

        Debug.DrawRay(leftDownPos.position, new Vector2(-1,-1), Color.green);
        Debug.DrawRay(leftUpPos.position,new Vector2(-1,1), Color.green);


        Debug.DrawRay(rightUpPos.position,new Vector2(1, 1), Color.green);
        Debug.DrawRay(rightDownPos.position, new Vector2(1, -1), Color.green);
=======
    public void SetActiveTouch(bool active)
    {
        CachedBoxCollider2D.enabled = active;
        CachedSpriteRenderer.color = (active == true) ? new Color(1,1,1,1) : new Color(1,1,1,0.5f);
>>>>>>> 60dc553554f433e6d2519f1b46bfee1d2f3c71c5
    }
}
