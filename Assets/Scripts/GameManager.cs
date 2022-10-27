using System;
using System.Collections;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}
    
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private float maxDuration;
    [SerializeField] private GameObject stone;
    private static int plrAmount;

    private float randomTimeDuration;
    
    private float curTime = 0.0f;
    private bool isCounting = false;
    private bool timerIsBackwards = false;

    private static Coroutine coolDownBeforeTime;
    
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

        StartCoroutine(RandomTimerCountDown());
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

    IEnumerator RandomTimerCountDown()
    {
        while (true)
        {
            randomTimeDuration -= Time.deltaTime;

            if (randomTimeDuration <= 0)
            {
                var stonePickUp = Instantiate(stone, new Vector2(Random.Range(-17.0f, 17.0f), 2.0f),
                    Quaternion.Euler(Vector3.zero));
                randomTimeDuration = Random.Range(0.8f, 5.0f);
                yield return null;
            }

            yield return null;
        }
    }
}
