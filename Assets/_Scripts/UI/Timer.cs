using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public static Timer Instance { get; private set; }
    // Start is called before the first frame update
    public TMP_Text timerText;
    public float t = 0f;
    private float startTime;
    public bool dead = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    public void restartTimer()
    {
        startTime = Time.time;
    }
    void Update()
    {
        //print(dead);
        if (!dead)
        {
            t = Time.time - startTime;
            string minutes = ((int)t / 60).ToString();
            string seconds = (t % 60).ToString("f2");
            timerText.text = minutes + ":" + seconds;
        }
    }
}
