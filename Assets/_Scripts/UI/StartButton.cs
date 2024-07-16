using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public Image blackLayout;
    public Button startButton;
    public Button restartButton;
    public GameObject joystick;
    // Start is called before the first frame update
    public void OnButtonDown()
    { 
        joystick.SetActive(true);
        if (MPManager.Instance != null)
        {
            MPManager.Instance.dead = false;
        }
        if (Timer.Instance != null)
        {
            print("timer!");
            Timer.Instance.restartTimer();
            Timer.Instance.dead = false;
        }
        if(blackLayout != null)
        {
            print("no black");
            blackLayout.enabled = false;
        }
        if (startButton != null)
        {
            print("no start button");
            startButton.gameObject.SetActive(false);
        }
        if (restartButton != null)
        {
            print("no restart button");
            restartButton.gameObject.SetActive(false);
        }
        print("start down");
        // print(ImageTargetGrid.Instance.startPosition);
        if(ImageTargetGrid.Instance.startPosition == Vector3.zero)
            ImageTargetGrid.Instance.startPosition = PlayerController.Instance.transform.position;
        PlayerController.Instance.SetPlayerPosition(ImageTargetGrid.Instance.startPosition); 

    }
}
