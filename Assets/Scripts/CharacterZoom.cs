using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterZoom : MonoBehaviour
{
    Tween tween;
    private bool isKillTween;
    // Start is called before the first frame update
    void Start()
    {
        Zoom();
    }

    public void DoKillTween()
    {
        isKillTween = true;
        tween.Kill();
        Debug.Log("on kill tweeem: " + this.gameObject.name);
    }

    void Zoom()
    {
        tween = this.transform.DOPunchScale(new Vector3(10, 10, 10), 1, 1, 1).OnComplete(ZoomSuccess);
    }
    void ZoomSuccess()
    {
        TaskUtil.Delay(this, delegate
        {
            if (isKillTween) return;
            Zoom();
        }, 2);
    }
}
