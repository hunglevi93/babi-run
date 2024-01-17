using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnCountDown : MonoBehaviour
{
	[SerializeField] Slider mLeftCountdown, mRightCountdown;
	[SerializeField] float mTotalCountdownTime;
	[SerializeField] float mCurrentCountdownTime;
    private void OnEnable()
    {
        mCurrentCountdownTime = 0;
    }
    private void LateUpdate()
    {
        mCurrentCountdownTime += Time.deltaTime;
        mCurrentCountdownTime = Mathf.Clamp(mCurrentCountdownTime, 0, mTotalCountdownTime);
        mLeftCountdown.gameObject.SetActive(true);
        mRightCountdown.gameObject.SetActive(true);

        UpdateCountdownSlider();
    }
    void UpdateCountdownSlider()
    {
        float curentValue = 1 - (float)mCurrentCountdownTime / (float)mTotalCountdownTime;
        curentValue = Mathf.Clamp(curentValue, 0, 1);
        mLeftCountdown.value = curentValue;
        mRightCountdown.value = curentValue;
    }
}
