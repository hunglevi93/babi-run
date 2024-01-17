using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterType : MonoBehaviour
{
    public Animator animator;
    public RacingRunController runControlller;
    public int score;
    public int line;
    public bool isTop1;
    public Transform transEnd;
    public bool isEnd = false;
    public TYPE_RACING_RUN type;
    public float speedMove;
    public bool isMainCharacter;
    public AudioClip runWinner;

    public void StartIdle()
    {
        animator.Play("idle");
    }
    public void StartRun()
    {
        animator.Play("run");
    }
    public void StartWarmUp()
    {
        animator.Play("warm up");
    }
    public void StartSad()
    {
        animator.Play("sad");
        TaskUtil.Delay(this, delegate
        {
            StartRun();
        }, 1.2f);
    }
    public void StartRunSpeed()
    {
        animator.Play("increase speed");
        score += 1;
        this.transform.DOMoveX(this.transform.position.x + 1.5f, 1).SetEase(Ease.Linear).OnComplete(CompleteMove);
        TaskUtil.Delay(this, delegate
        {
            RacingRunController.instance.CheckRoundFinal();
            StartRun();
        }, 1.2f);
    }
    public void StartBravo()
    {
        animator.Play("bravo");
    }
    public void PlayRunSpeed()
    {
        animator.Play("increase speed");
    }
    public void StartTired()
    {
        Debug.Log("TIRED: " + this.gameObject.name);
        animator.Play("tired");
    }
    public void StartCongratulation()
    {
        animator.Play("congratulation");
    }
    public void GoToLineWin(bool isRank1, Transform transStop)
    {
        transEnd = transStop;
        isEnd = true;
        isTop1 = isRank1;
        if (isRank1)
        {
            PlayRunSpeed();
            speedMove = speedMove + 1.5f;

        }
        else
        {
            StartRun();
            speedMove = speedMove + 1.5f;
        }
    }
    void CompleteMove()
    {
        Debug.Log("Completee");
    }

    private void Update()
    {
        if (isEnd)
        {
            float step = speedMove * Time.deltaTime;
            transform.Translate(Vector2.right * step);
            if (transform.position.x >= transEnd.position.x)
            {
                if (isTop1)
                {
                    Debug.Log("top1: " + transform.position.x + " " + transEnd.position.x);
                    StartBravo();
                    RacingRunController.instance.CheckStopBg();
                }
                else
                {
                    Debug.Log(gameObject.name + " Start tireddddd");
                    RacingRunController.instance.CheckStopBg();
                    StartTired();
                }
                isEnd = false;
            }
        }
        if (isTop1 && !isEnd && !RacingRunController.instance.isStateResult())
        {
            transform.position = transEnd.position;
        }
    }
}
