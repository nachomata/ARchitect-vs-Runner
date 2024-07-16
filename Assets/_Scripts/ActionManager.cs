using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Vuforia;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
public class ActionManager : MonoBehaviour
{
    
    public static ActionManager Instance { get; private set; }
    public string keyCard;
    public string fogCard;
    public string LightningCard;
    public string speedUpCard;
    public GameObject fogEffectPrefab;
    public GameObject LightningPrefab;
    public float LightningArea = 1f;
    public int playerHP = 3;
    public UnityEngine.UI.Image[] Hearts;
    public UnityEngine.UI.Image BlackMask;
    public TMP_Text Dead;
    private List<ImageTargetBehaviour> trackedMarkers = new List<ImageTargetBehaviour>();

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
        VuforiaBehaviour.Instance.World.OnObserverCreated += OnObserverCreated;

        if (BlackMask != null)
            BlackMask.enabled = true;
        if (Dead != null)
            Dead.enabled = false;
    }

    void OnDestroy()
    {
        if (VuforiaBehaviour.Instance != null)
        {
            VuforiaBehaviour.Instance.World.OnObserverCreated -= OnObserverCreated;
        }
    }

    void Update()
    {
        if (MPManager.Instance != null)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (MPManager.Instance.UseMP(2))
                {
                    print("damage");
                    StartCoroutine(OnDamage());
                }
                print(playerHP);
            }
        }
        UpdateHP();
        CheckTime();
    }


    public IEnumerator OnDamage()
    {
        Timer.Instance.restartTimer();
        playerHP -= 1;
        yield return new WaitForSeconds(1.5f);
        if(PlayerController.Instance != null)
            // PlayerController.Instance._animator.SetBool("Death_b", false);
        generatePlayer();
    }

    private void OnObserverCreated(ObserverBehaviour observer)
    {
        if (observer is ImageTargetBehaviour imageTarget)
        {
            imageTarget.OnTargetStatusChanged += OnTargetStatusChanged;
        }
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
    {
        // if (behaviour.TargetName == "start" &&
        //     targetStatus.Status == Status.TRACKED)
        //     generatePlayer();


        if (MPManager.Instance != null)
        {
            
            if (behaviour.TargetName == fogCard &&
                targetStatus.Status == Status.TRACKED)
            {
                if (MPManager.Instance.UseMP(1))
                    StartCoroutine(HandleFogEffect());
            }
            else if (behaviour.TargetName == LightningCard &&
                    targetStatus.Status == Status.TRACKED)
            {
                if (MPManager.Instance.UseMP(2))
                    StartCoroutine(HandleLightningEffect());
            }
            else if (behaviour.TargetName == speedUpCard &&
                    targetStatus.Status == Status.TRACKED)
            {
                if (MPManager.Instance.UseMP(3))
                    StartCoroutine(HandleSpeedUp());
            }
        }
    }


    void generatePlayer()
    {
        // GameObject player = GameObject.FindGameObjectWithTag("Player");
        // player.transform.position = ImageTargetGrid.Instance.startPosition;   
        PlayerController.Instance.SetPlayerPosition(ImageTargetGrid.Instance.startPosition); 
    }

    IEnumerator HandleSpeedUp()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");

        if (playerObject != null)
        {
            PlayerController playerController = playerObject.GetComponent<PlayerController>();

            if (playerController == null)
            {
                Debug.LogError("PlayerController component not found on the Player object.");
                yield return new WaitForSeconds(0f);
            }
            else
            {
                playerController.speedScale = 2.5f;
                yield return new WaitForSeconds(3f);
                playerController.speedScale = 1f;
            }
        }
        else
        {
            Debug.LogError("No GameObject with tag 'Player' found.");
            yield return new WaitForSeconds(0f);
        }
        yield return new WaitForSeconds(0f);
    }

    IEnumerator HandleFogEffect()
    {

        trackedMarkers.Clear();
        trackedMarkers = VuforiaBehaviour.Instance.World.GetObserverBehaviours().OfType<ImageTargetBehaviour>().ToList();

        foreach (var marker in trackedMarkers)
        {
            if (marker.TargetName == keyCard &&
                (marker.TargetStatus.Status == Status.TRACKED || marker.TargetStatus.Status == Status.EXTENDED_TRACKED))
            {
                print("fog");
                GameObject fogEffect = Instantiate(fogEffectPrefab, marker.transform.position, Quaternion.Euler(-90, 0, 0));
                fogEffect.transform.SetParent(marker.transform);

                ParticleSystem particleSystem = fogEffect.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    particleSystem.Play();
                }

                yield return new WaitForSeconds(3f);

                if (particleSystem != null)
                {
                    particleSystem.Stop();
                }

                Destroy(fogEffect);
                break;
            }
        }

    }

    IEnumerator HandleLightningEffect()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("No GameObject with tag 'Player' found in the scene.");
            yield break;
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                print("Lightning");
                Vector3 playerPosition = player.transform.position;

                yield return new WaitForSeconds(1f);

                GameObject LightningEffect = Instantiate(LightningPrefab, playerPosition + transform.up * 5, Quaternion.Euler(-90, 0, 0));

                ParticleSystem particleSystem = LightningEffect.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    particleSystem.Play();
                }

                float distance = Vector3.Distance(playerPosition, player.transform.position);

                if (distance < LightningArea)
                {
                    StartCoroutine(OnDamage());
                    Debug.Log("Player HP: " + playerHP);
                }
                yield return new WaitForSeconds(1f);

                if (particleSystem != null)
                {
                    particleSystem.Stop();
                }
                Destroy(LightningEffect);
            }
        }
    }

    void UpdateHP()
    {
        if (playerHP == 0)
        {
            Hearts[0].enabled = false;
            Hearts[1].enabled = false;
            Hearts[2].enabled = false;
            MPManager.Instance.dead = true;
            if (Timer.Instance != null)
            {
                Timer.Instance.dead = true;
            }
            if (BlackMask != null)
                BlackMask.enabled = true;
            if (Dead != null)
            {
                Dead.enabled = true;
            }
        }
        else if (playerHP == 1)
        {
            Hearts[0].enabled = true;
            Hearts[1].enabled = false;
            Hearts[2].enabled = false;
        }
        else if (playerHP == 2)
        {
            Hearts[0].enabled = true;
            Hearts[1].enabled = true;
            Hearts[2].enabled = false;
        }
        else if (playerHP == 3)
        {
            Hearts[0].enabled = true;
            Hearts[1].enabled = true;
            Hearts[2].enabled = true;
        }
    }

    void CheckTime()
    {
        if (Timer.Instance != null)
        {
            if (Timer.Instance.t > 90f)
            {
                if (BlackMask != null)
                    BlackMask.enabled = true;
                if (Dead != null)
                {
                    Dead.enabled = true;
                }
                Timer.Instance.dead = true;
                MPManager.Instance.dead = true;
            }
        }
    }
}

