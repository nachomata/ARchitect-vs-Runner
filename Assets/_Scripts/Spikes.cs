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
        initialPosition = transform.position;
        raisedPosition = new Vector3(transform.position.x, transform.position.y + raisedHeight, transform.position.z);
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
        Vector3 startingPosition = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }
}