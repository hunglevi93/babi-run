using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOutTurn : MonoBehaviour
{
    [SerializeField] private float timeAlive;
    private void OnEnable()
    {
        SetTimeOut();
    }
    private void SetTimeOut()
    {
        TaskUtil.Delay(this, delegate 
        {
            if(RacingRunController.instance.stateChoose == STATE_CHOOSE.CORRECT || RacingRunController.instance.stateChoose == STATE_CHOOSE.WRONG)
            {
                return;
            }
            if (this.gameObject.activeSelf)
            {
                this.PostEvent(EventID.OnTurnTimeOut);
            }
        }, timeAlive);
    }
    public void SetAliveTime(float time)
    {
        timeAlive = time;
    }
}
