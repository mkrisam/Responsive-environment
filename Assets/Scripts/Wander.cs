using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float rotateSpeed = 100f;

    private bool isWandering = false;
    private bool isRotatingRight = false;
    private bool isRotatingLeft = false;
    private bool isWalking = false;


    // Update is called once per frame
    void Update()
    {
        if (!isWandering)
        {
            StartCoroutine(WanderState());
        }
        if (isRotatingRight)
        {
            transform.Rotate(transform.up * Time.deltaTime * rotateSpeed);
        }
        if (isRotatingLeft)
        {
            transform.Rotate(-transform.up * Time.deltaTime * rotateSpeed);
        }
        if (isWalking)
        {
            transform.position += transform.forward * walkSpeed * Time.deltaTime;
        }
    }

    IEnumerator WanderState()
    {
        int rotateTime = Random.Range(1, 3);
        int rotateCoolDown = Random.Range(0, 2);
        int rotateLorR = Random.Range(0, 3);
        int walkCoolDown = Random.Range(0, 2);
        int walkTime = Random.Range(1, 5);

        isWandering = true;

        yield return new WaitForSeconds (walkCoolDown);
        isWalking = true;
        yield return new WaitForSeconds(walkTime);
        isWalking = false;
        yield return new WaitForSeconds(rotateCoolDown);
        if (rotateLorR == 1)
        {
            isRotatingLeft = true;
            yield return new WaitForSeconds(rotateTime);
            isRotatingLeft = false;
        }
        if (rotateLorR == 2)
        {
            isRotatingRight = true;
            yield return new WaitForSeconds(rotateTime);
            isRotatingRight = false;
        }
        isWandering = false;
    }
}
