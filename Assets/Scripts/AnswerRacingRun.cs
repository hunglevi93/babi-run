using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AnswerRacingRun : MonoBehaviour
{
    [SerializeField] Image background;
    [SerializeField] Text txtAnswer;
    [SerializeField] AudioClip clipAnswer;
    [SerializeField] Animator animator;
    public string answer;
    public bool isChoosed = false;
    private Color32 colorDefault = new Color32(50, 50, 50, 255);
    private Color32 colorCorrect = new Color32(255, 255, 255, 255);
    public void PlayAnimation()
    {
        animator.enabled = true;
        animator.Play("scale");
    }
    public void InitAnswer(string answerQuestion, AudioClip clip)
    {
        answer = answerQuestion;
        txtAnswer.text = answer;
        background.sprite = RacingRunController.instance.answerDefault;
        isChoosed = false;
        txtAnswer.color = colorDefault;
        clipAnswer = clip;
    }
    public void ChooseAnswer()
    {
        Debug.Log("Choose answer");
        isChoosed = true;
        TaskUtil.Delay(this, delegate
        {
            RacingRunController.instance.PlaySE(clipAnswer);
        }, 1);
        RacingRunController.instance.CheckAnswer(answer);
        animator.Play("default");
    }
    public void ShowResult()
    {
        //Debug.Log("Show result: " + answer + " " + RacingRunControlller.instance.getAnswerCorrect());
        if (answer.Equals(RacingRunController.instance.getAnswerCorrect()))
        {
            txtAnswer.color = colorCorrect;
            background.sprite = RacingRunController.instance.answerTrue;
        }
        else
        {
            if (isChoosed)
            {
                background.sprite = RacingRunController.instance.answerFalse;
            }
        }
    }
}
