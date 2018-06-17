using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Finish : MonoBehaviour
{

    SpriteRenderer mSpriteRenderer;
    public int colorID;
    public RectTransform bar;
    private RectTransform mBar;
    private float mStartTime;
    private float mTimeout;
    private float mScale;
    void Start()
    {
        //add timeout bar
        var crtsz = GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta;
        var w2v = Camera.main.WorldToViewportPoint(transform.position);
        var p = new Vector3(
            crtsz.x * w2v.x - crtsz.x / 2,
            crtsz.y * w2v.y - crtsz.y / 2,
              0);
        mBar = Instantiate(bar, p, Quaternion.identity);
        mBar.transform.SetParent(GameObject.Find("Canvas").transform, false);

        mSpriteRenderer = GetComponent<SpriteRenderer>();

        reset();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.isNotPlaying())
            return;
        float progress = (Time.time - mStartTime) / mTimeout;
        if (progress >= 1) changeColor();
        mBar.localScale = new Vector3(1, mScale * (1 - progress), 1);
        mBar.Rotate(0, 0, 20 * progress);
    }
    public void changeColor()
    {
        // colorID = -1;
        colorID = GameController.getNewCircleColorID();
        reset();
    }
    private void reset()
    {
        mSpriteRenderer.color = GameController.mColors[colorID];
        mBar.GetComponent<Image>().color = GameController.mColors[colorID];
        mBar.localScale = Vector3.one;
        mStartTime = Time.time;
        mTimeout = Random.value * (GameController.mCircleTimeoutMax - GameController.mCircleTimeoutMin) + GameController.mCircleTimeoutMin;
        mScale = mTimeout / GameController.mCircleTimeoutMax;

    }

}
