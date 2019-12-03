using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPlayableCharacter : MonoBehaviour
{
    public float DisplayTime = 4.0f;
    public GameObject DialogBox;

    private float timerDisplay;

    // Start is called before the first frame update
    void Start()
    {
        DialogBox.SetActive(false);
        timerDisplay = -1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerDisplay >= 0)
        {
            timerDisplay -= Time.deltaTime;
            if (timerDisplay < 0)
            {
                DialogBox.SetActive(false);
            }
        }
    }

    public void DisplayDialog()
    {
        timerDisplay = DisplayTime;
        DialogBox.SetActive(true);
    }
}
