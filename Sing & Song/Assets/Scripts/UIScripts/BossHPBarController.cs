using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHPBarController : MonoBehaviour
{
    [SerializeField] private Slider bossHPBarSlider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetUpHPBarSlider(float maxHP)
    {
        bossHPBarSlider.minValue = 0.0f;
        bossHPBarSlider.maxValue = maxHP;
    }

    public void UpdateHPBarProgress(float currentHP, float progressBarTransitionSpeed = 3.0f)
    {
        StopAllCoroutines();
        StartCoroutine(UpdateProgressSmoothly(currentHP, progressBarTransitionSpeed));
    }

    private IEnumerator UpdateProgressSmoothly(float targetUpdateAmount, float progressBarTransitionSpeed)
    {
        float valueProgress = 0.0f;
        float beforeUpdateAmount = 0.0f;
        float timeProgress = 0.0f;

        valueProgress = beforeUpdateAmount = bossHPBarSlider.value;

        while (Mathf.Abs(valueProgress - targetUpdateAmount) != 0.0f)
        {
            timeProgress += progressBarTransitionSpeed * Time.deltaTime;
            if (timeProgress > 1.0f) timeProgress = 1.0f;

            valueProgress = Mathf.Lerp(beforeUpdateAmount, targetUpdateAmount, timeProgress);

            //Debug.Log(valueProgress);
            bossHPBarSlider.value = valueProgress;

            yield return null;
        }
    }
}
