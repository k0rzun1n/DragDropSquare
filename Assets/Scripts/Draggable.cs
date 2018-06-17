using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Draggable : MonoBehaviour
{
    SpriteRenderer mSpriteRenderer;
    public Vector3 originalPos;
    public float dragStart = -10;
    public bool dragged = false;
    public int colorID;
    public ParticleSystem collisionExplosion;
    void Start()
    {
        mSpriteRenderer = GetComponent<SpriteRenderer>();
        mSpriteRenderer.color = GameController.mColors[colorID];
    }
    public void onClick()
    {
        if (GameController.isNotPlaying())
            return;
        dragged = true;
        // Debug.Log(Time.time - dragStart);
        if (Time.time - dragStart < .5f)
        {
            dragStart = -1;
            changeColor();
        }
        else
            dragStart = Time.time;
    }
    public void changeColor()
    {
        // colorID = -1;
        colorID = GameController.getNewSquareColorID();
        mSpriteRenderer.color = GameController.mColors[colorID];
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Finish fin = collision.GetComponent<Finish>();
        if (fin && fin.colorID == colorID)
        {
            var m = Instantiate(collisionExplosion, transform.position, Quaternion.identity).main;
            m.startColor = mSpriteRenderer.color ;
            fin.changeColor();
            changeColor();
            dragStart = -10;
            transform.position = originalPos;
            dragged = false;
            GameController.addScore();
        }
    }

    void Update()
    {
#if !UNITY_EDITOR        
        if (dragged && Input.GetTouch(0).phase == TouchPhase.Moved)
            transform.Translate((Input.GetTouch(0).deltaPosition) / 100); //100==default pixels to units
#endif
        if (!dragged)
            transform.Translate((originalPos - transform.position) / 10);
    }

#if UNITY_EDITOR
    private Vector3 dragFrom;
    void OnMouseDown()
    {
        onClick();
        dragFrom = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // DebugLogger.Log("Touched" + Time.frameCount);
        // Debug.Log("Touched" + Time.frameCount);
        // m_SpriteRenderer.color = new Color(Random.value,Random.value,Random.value);
    }
    void OnMouseUp()
    {
        // gameObject.SetActive(false);
        dragged = false;
    }

    void OnMouseDrag()
    {
        if (dragged)
        {
            var dragTo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Debug.Log("drag " + dragTo);
            transform.Translate(dragTo - dragFrom);
            dragFrom = dragTo;
        }
    }
#endif
}
