using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private float raisedHeight = 5f;
    [SerializeField] private float activeTime = 2f;
    [SerializeField] private float inactiveTime = 2f;
    [SerializeField] private float movementDuration = 0.5f;

    private Vector3 initialPosition;
    private Vector3 raisedPosition;

    void Start()
    {
        initialPosition = transform.localPosition;
        raisedPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + raisedHeight, transform.localPosition.z);
        StartCoroutine(SpikeCycle());
    }

    private IEnumerator SpikeCycle()
    {
        while (true)
        {
            yield return StartCoroutine(MoveSpikes(raisedPosition, movementDuration));
            yield return new WaitForSeconds(activeTime);
            yield return StartCoroutine(MoveSpikes(initialPosition, movementDuration));
            yield return new WaitForSeconds(inactiveTime);
        }
    }

    private IEnumerator MoveSpikes(Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startingPosition = transform.localPosition;

        while (elapsedTime < duration)
        {
            transform.localPosition = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetPosition;
    }
}