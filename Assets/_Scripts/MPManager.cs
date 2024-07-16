using UnityEngine;
using UnityEngine.UI;

public class MPManager : MonoBehaviour
{
    public static MPManager Instance { get; private set; }
    public float maxMP = 3f;
    public float MP;
    public float regenInterval = 5f;
    public float CD = 2f;
    public bool dead = true;
    private float regenTimer;
    private float delayTimer;
    private bool isRegenDelayed;
    private bool canUseMP = true;

    public Image[] mpBars;

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
        MP = 0f;
        delayTimer = 0f;
        regenTimer = 0f;
        InitializeMPBars();
    }

    void Update()
    {
        if (!dead)
        {

            if (isRegenDelayed)
            {
                delayTimer += Time.deltaTime;
                if (delayTimer >= CD)
                {
                    isRegenDelayed = false;
                    delayTimer = 0f;
                }
            }
            else// MP自动恢复
            {
                if (MP <= maxMP)
                {
                    MP += Time.deltaTime / regenInterval;
                    regenTimer += Time.deltaTime;
                    if (regenTimer >= regenInterval)
                    {
                        canUseMP = true;
                        regenTimer = 0f;
                    }
                    CalculateBar();
                }
            }
        }
    }

    void CalculateBar()
    {
        if (MP <= 1f)
        {
            mpBars[0].fillAmount = MP;
            mpBars[1].fillAmount = 0f;
            mpBars[2].fillAmount = 0f;
        }
        else if (MP <= 2f)
        {
            mpBars[0].fillAmount = 1f;
            mpBars[1].fillAmount = MP - 1f;
            mpBars[2].fillAmount = 0f;
        }
        else
        {
            mpBars[0].fillAmount = 1f;
            mpBars[1].fillAmount = 1f;
            mpBars[2].fillAmount = MP - 2f;
        }
    }


    public bool UseMP(float amount)
    {
        if (!canUseMP)
        {
            Debug.Log("Cannot use MP yet!");
            return false;
        }
        if (MP > amount)
        {
            Debug.Log("use MP");
            MP -= amount;
            if (MP < 0f)
                MP = 0f;
            CalculateBar();

            canUseMP = false;
            isRegenDelayed = true;
            delayTimer = 0f;
            return true;
        }
        else
        {
            Debug.Log("MP is not enough");
            return false;
        }
    }

    void InitializeMPBars()
    {
        foreach (Image mpBar in mpBars)
        {
            mpBar.fillAmount = 0f;
        }
    }
}
