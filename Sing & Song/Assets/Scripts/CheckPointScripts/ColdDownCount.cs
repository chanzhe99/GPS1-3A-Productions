using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class ColdDownCount
{
    private float timeCount;
    [SerializeField] private float coldDownTime;
    private float defalutTime;

    private bool isTiming;

    public ColdDownCount(float startTime, float endTime)
    {
        defalutTime = startTime > endTime ? endTime : startTime;
        coldDownTime = startTime > endTime ? startTime : endTime;
    }

    public ColdDownCount(float coldDown)
    {
        Mathf.Abs(coldDown);
        defalutTime = 0.0f;
        coldDownTime = coldDown;
    }

    public bool CountingAndCheck()
    {
        if(!(isTiming = timeCount >= coldDownTime))
        {
            timeCount += 1.0f * Time.deltaTime;
        }
        return isTiming;
    }

    public void Counting()
    {
        if (!(isTiming = timeCount >= coldDownTime))
        {
            timeCount += 1.0f * Time.deltaTime;
        }

    }

    public bool IsTimerValueIsDefault()
    {
        return timeCount == defalutTime;
    }

    public void ResetTimer()
    {
        timeCount = defalutTime;
    }

    public bool GetIsTiming()
    {
        return isTiming;
    }

}
