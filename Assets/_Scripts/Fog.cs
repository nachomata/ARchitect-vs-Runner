using UnityEngine;
using Vuforia;
using System.Collections;

public class FogEffectController : MonoBehaviour
{
    public GameObject fogEffectPrefab;

    void Start()
    {
        VuforiaBehaviour.Instance.World.OnObserverCreated += OnObserverCreated;
    }

    void OnDestroy()
    {
        if (VuforiaBehaviour.Instance != null)
        {
            VuforiaBehaviour.Instance.World.OnObserverCreated -= OnObserverCreated;
        }
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
        if (behaviour.TargetName == "fog" &&
            targetStatus.Status == Status.TRACKED)
        {
            
            StartCoroutine(HandleFogEffect(behaviour.transform));
        }
    }

    IEnumerator HandleFogEffect(Transform targetTransform)
    {
        print("fog");
        ; GameObject fogEffect = Instantiate(fogEffectPrefab, targetTransform.position, Quaternion.identity);
        fogEffect.transform.SetParent(targetTransform);

        ParticleSystem particleSystem = fogEffect.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            particleSystem.Play();
        }

        yield return new WaitForSeconds(5f);

        if (particleSystem != null)
        {
            particleSystem.Stop();
        }

        Destroy(fogEffect);
    }
}
