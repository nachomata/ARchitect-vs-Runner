using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapButton : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnButtonDown()
    {
        if (ImageTargetGrid.Instance != null && PlayerController.Instance != null)
        {
            ImageTargetGrid.Instance.snap = true;
            PlayerController.Instance.ParentTransform = ImageTargetGrid.Instance.centerTransform;
        }
            print("snap down");
    }

    // Update is called once per frame
    public void OnButtonUp()
    {
        if (ImageTargetGrid.Instance != null)
        {
            ImageTargetGrid.Instance.snap = false;
        }
        print("snap up");
    }
}
