using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundRacingRun : MonoBehaviour
{
    [SerializeField] private float xLimit, yReset;
    [SerializeField] private Image imgForce;
    [SerializeField] private RectTransform rect;
    [SerializeField] private bool isDestroy;
    public void DestroyObject()
    {
        if (isDestroy)
        {
            TaskUtil.Delay(this, delegate
            {
                Destroy(this.gameObject);
            }, 5);
        }
    }
     void Update()
    {
        if (RacingRunController.instance.isStartGame)
        {
            DestroyObject();
            if (xLimit != 0)
            {
                if (rect.localPosition.x <= xLimit)
                {
                    //rect.localPosition = new Vector3(xReset, yReset, 0);
                    rect.localPosition = new Vector3(imgForce.GetComponent<RectTransform>().anchoredPosition.x + imgForce.GetComponent<RectTransform>().sizeDelta.x - 5, yReset, 0);
                }
            }
            transform.Translate(Vector2.left * Time.deltaTime * RacingRunController.instance.speedBg);
        }
    } 
}   
