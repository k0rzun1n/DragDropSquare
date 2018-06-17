using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class GameController : MonoBehaviour
{

    public static List<Color> mColors;
    public static float mCircleTimeoutMin;
    public static float mCircleTimeoutMax;
    public static List<Draggable> squares = new List<Draggable>(3);
    public static List<Finish> circles = new List<Finish>(3);
    private static int mBestScore;
    private static int mCurrentScore = 0;
    private static Text mCurrentScoreText;
    private static Text mBestScoreText;
    private static Text mTimerText;
    private static GameObject mButtons;
    public Draggable squareSprite;
    public Finish circleSprite;
    void Start()
    {
    	startTime = (int)Time.time;

        mCurrentScoreText = GameObject.Find("CurrentScore").GetComponent<Text>();
        mCurrentScoreText.text = mCurrentScore.ToString();

        mBestScoreText = GameObject.Find("BestScore").GetComponent<Text>();
        if (File.Exists(Application.persistentDataPath + "/score.txt"))
            int.TryParse(File.ReadAllText(Application.persistentDataPath + "/score.txt"), out mBestScore);
        mBestScoreText.text = mBestScore.ToString();

        mTimerText = GameObject.Find("Timer").GetComponent<Text>();
        mButtons = GameObject.Find("ButtonsPanel");
		mButtons.SetActive(false);

        var mSettings = JsonUtility.FromJson<Settings>(Resources.Load("Settings").ToString());
        mColors = mSettings.getColors();
        mCircleTimeoutMin = mSettings.circleTimeoutMin;
        mCircleTimeoutMax = mSettings.circleTimeoutMax;

        var spriteRen = squareSprite.GetComponent<SpriteRenderer>();
        var spriteSize = spriteRen.size;
        float xMargin = squareSprite.transform.localScale.x * spriteSize.x / 2 + 0.1f;
        float yMargin = squareSprite.transform.localScale.y * spriteSize.y / 2 + 0.1f;
        var screenSize = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        List<Vector3> spawnPos = new List<Vector3>(6);
        spawnPos.Add(new Vector3(-(screenSize.x - xMargin), (screenSize.y - yMargin), 0));
        spawnPos.Add(new Vector3(-(screenSize.x - xMargin), 0, 0));
        spawnPos.Add(new Vector3(-(screenSize.x - xMargin), -(screenSize.y - yMargin), 0));
        spawnPos.Add(new Vector3((screenSize.x - xMargin), (screenSize.y - yMargin), 0));
        spawnPos.Add(new Vector3((screenSize.x - xMargin), 0, 0));
        spawnPos.Add(new Vector3((screenSize.x - xMargin), -(screenSize.y - yMargin), 0));
        for (int i = 0; i < 3; i++)
        {
            squares.Add(Instantiate(squareSprite, spawnPos[i], Quaternion.identity));
            squares[i].originalPos = spawnPos[i];
            squares[i].colorID = -1;

            circles.Add(Instantiate(circleSprite, spawnPos[i + 3], Quaternion.identity));
            circles[i].colorID = -1;
        };
        for (int i = 0; i < 3; i++)
        {
            squares[i].colorID = getNewSquareColorID();
            circles[i].colorID = getNewCircleColorID();
        };

    }
    public static void addScore()
    {
        mCurrentScore++;
        mCurrentScoreText.text = mCurrentScore.ToString();
    }
    public static void gameOver()
    {
        Time.timeScale = 0.2f;
        mButtons.SetActive(true);
        if (mCurrentScore > mBestScore)
            File.WriteAllText(Application.persistentDataPath + "/score.txt", mCurrentScore.ToString());
    }
    public void restart()
    {
        Time.timeScale = 1;
        startTime = (int)Time.time;
        mCurrentScore = 0;
        mCurrentScoreText.text = mCurrentScore.ToString();
        if (File.Exists(Application.persistentDataPath + "/score.txt"))
            int.TryParse(File.ReadAllText(Application.persistentDataPath + "/score.txt"), out mBestScore);
        mBestScoreText.text = mBestScore.ToString();
        mButtons.SetActive(false);
		for (int i = 0; i < 3; i++)
		{
			squares[i].changeColor();
			circles[i].changeColor();
		}
	}
    public void quit()
    {
        Application.Quit();
    }

    public static int getNewCircleColorID()
    {
        while (true)
        { //add check for amount of colors
            var cid = Random.Range(0, mColors.Count);
            foreach (var c in circles)
                if (cid == c.colorID) cid = -1;
            if (cid != -1)
                return cid;
        }
    }
    public static int getNewSquareColorID()
    {
        while (true)
        { //add check for amount of colors
            var cid = Random.Range(0, mColors.Count);
            foreach (var c in squares)
                if (cid == c.colorID) cid = -1;
            if (cid != -1)
                return cid;
        }
    }
    private int startTime;
    Draggable draggedSquare;
	public static bool isNotPlaying(){
		return mButtons.activeSelf;
	}
    void Update()
    {
        if (isNotPlaying())
            return;
        int timeLeft = 90 - ((int)Time.time - startTime);
        string minutes = (timeLeft / 60).ToString("D2");
        string seconds = (timeLeft % 60).ToString("D2");
        if (mTimerText) mTimerText.text = string.Format("{0}:{1}", minutes, seconds);
        if (timeLeft <= 0) gameOver();

#if !UNITY_EDITOR && UNITY_ANDROID
        // DebugLogger.Log((Input.GetTouch(0).deltaPosition)+" ");
        if (Input.touchCount > 0)
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                var scr = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                var hit = Physics2D.Raycast(scr, Vector2.zero); //zero?
				if (hit.collider.tag == "Player"){ //all Players are Draggable
					draggedSquare = hit.collider.GetComponent<Draggable>();
					draggedSquare.onClick();
				}
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
				draggedSquare.dragged = false;
            }
#endif
    }
}
