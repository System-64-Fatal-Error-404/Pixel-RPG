using System;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}
    
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private float maxDuration;
    private static int plrAmount;
    
    private float curTime = 0.0f;
    private bool timerHasStarted = false;
    private bool timerIsBackwards = false;

    private Coroutine coolDownBeforeTime;
    public static Transform plrTrans 
    {
        get => GameObject.Find("Player").GetComponent<Transform>();
    }

    private void Awake()
    {
        if (plrAmount > 1)
        {
            
        }
        
        if (Instance && Instance != null)
        {
            Destroy(Instance);
            return;
        }
        
        if (_timerText == null)
        {
            _timerText = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();
        }
    }

    private void Update()
    {
        if (timerIsBackwards)
        {
            curTime = Time.realtimeSinceStartup;
        }
        else if (!timerIsBackwards)
        {
            curTime = Time.realtimeSinceStartup;
        }
        else
        {
            curTime = 0.0f;
        }
    }
    private void FixedUpdate()
    {
        string curSec = ((int)(curTime % 60)).ToString();
        if (curSec.Length < 2)
        {
            curSec = "0" + curSec;
        }
        else
        {
            curSec = curSec;
        }
        _timerText.text = "Time \n" + (int)(curTime / 60) + " : " + curSec;
    }
}
