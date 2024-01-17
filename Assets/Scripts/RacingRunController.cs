using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.IO;
using Spine.Unity;
using DG.Tweening;


public enum TYPE_RACING_RUN : int
{
    BUNNY,
    SQUIREL,
    TURTLE,
}
public enum STATE_RACING_RUN: int
{ 
    SELECT_LEVEL,
    GAMEPLAY,
    RESULT,
}
public enum STATE_CHOOSE : int
{
    CORRECT,
    WRONG,
    NONE
}

[Serializable]
public class AnswerRacingRunDetails
{
    public string answer;
    public AudioClip Clip;
}

[Serializable]
public class QuestionRacingRun
{
    public AudioClip clipQuestion;
    public string question;
    public string answerCorrect;
    public List<AnswerRacingRunDetails> answerList;
}
public class RacingRunController : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    public static RacingRunController instance;
    public GameObject[] TurnSendAnswearsCorrects;
    public GameObject[] TurnSendAnswearsWrongs;
    public List<Animator> characters;
    public List<CharacterType> charactersController;
    public List<CharacterType> charactersCompare;
    public List<CharacterType> characterTypesBackup;
    public CharacterType mainCharacter;

    [Header("Voice")]
    public AudioSource sourceSE;
    public AudioSource sourceRun;
    public AudioClip clipWelcome, clipChooseCharacter, clipVictory, clipCongratulation;
    public AudioClip clickAudio, selectAudio;
    public AudioClip clipSelectBunny, clipSelectSquirell, clipSelectTurtle;
    public AudioClip correctAudio, wrongAudio;
    public AudioClip timeAudio;
    public AudioClip runAudio, runFastAudio;

    [Header("State")]
    public TYPE_RACING_RUN typeCharacter = TYPE_RACING_RUN.BUNNY;
    public STATE_RACING_RUN state = STATE_RACING_RUN.SELECT_LEVEL;
    [SerializeField] private GameObject objSelect, objGamePlay, objResult, objBlocker, objTime, objQuestionAndAnswer, objLineWine;
    [SerializeField] private Image imgTime;
    [SerializeField] private Sprite time1, time2, time3;

    [Header("Setting question/answer")]
    public TimeOutTurn timeOutTurn;
    public TurnCountDown countDownTurn;
    public bool isStartGame;
    public Sprite answerTrue, answerFalse, answerDefault;
    public List<AnswerRacingRun> answers;
    public List<QuestionRacingRun> questions;
    public STATE_CHOOSE stateChoose = STATE_CHOOSE.NONE;

    [Header("Result")]
    public int currentQuestion = 0;
    public GameObject HandGuiding;
    public Image Loading;
    public float speedBg;
    public int timeAppear;
    public int timeCountDown;
    public int totalQuestion = 7;
    private int numEndOfCharacter;
    public Transform top1, top2, top3, lineStop1, lineStop2, lineStop3;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        /*     Debug.Log("isPlaySoundOfQuestion la false");*/
        this.gameObject.AddComponent<EventDispatcher>();
        canvas = this.transform.root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        this.RegisterListener(EventID.OnTurnTimeOut, (param) => TurnWrong());
        /*  this.RegisterListener(EventID.OnPause, (param) => PauseSound());
          this.RegisterListener(EventID.OnUnPause, (param) => UnPauseSound())*/
        ;
        TaskUtil.Delay(this, delegate
        {
            GoChooseCharacter();
        }, 2.5f);
    }

    public void SelectCharacter(int id)
    {
        PlaySE(selectAudio);
        objBlocker.SetActive(true);
        switch (id)
        {
            case 0:
                typeCharacter = TYPE_RACING_RUN.BUNNY;

                mainCharacter = charactersController[0];
                TaskUtil.Delay(this, delegate
                {
                    PlaySE(clipSelectBunny);
                }, 1f);
                break;
            case 1:
                typeCharacter = TYPE_RACING_RUN.SQUIREL;
                mainCharacter = charactersController[1];
                TaskUtil.Delay(this, delegate
                {
                    PlaySE(clipSelectSquirell);
                }, 1f);
                break;
            case 2:
                typeCharacter = TYPE_RACING_RUN.TURTLE;
                mainCharacter = charactersController[2];
                TaskUtil.Delay(this, delegate
                {
                    PlaySE(clipSelectTurtle);
                }, 1f);
                break;
        }
        mainCharacter.isMainCharacter = true;
        for (int i = 0; i < charactersController.Count; i++)
        {
            characters[i].GetComponent<CharacterZoom>().DoKillTween();
            if (i == id)
            {
                characters[i].Play("select");
            }
            else
            {
                characters[i].Play("deselect");
            }
        }
        TaskUtil.Delay(this, delegate
        {
            Loading.gameObject.SetActive(true);
        }, 2.5f);
        TaskUtil.Delay(this, delegate
        {
            LoadingMoveGamePlay();
        }, 3.5f);
    }
    public void GoChooseCharacter()
    {
        Debug.Log("Chon nhan vat nao");
        state = STATE_RACING_RUN.SELECT_LEVEL;
        objBlocker.SetActive(true);
        PlaySE(clipChooseCharacter);
        TaskUtil.Delay(this, delegate
        {
            objBlocker.SetActive(false);
        }, 2.8f);
    }
    #region Audio
    public void PlaySE(AudioClip clip)
    {
        if (sourceSE)
        {
            sourceSE.clip = clip;
            sourceSE.PlayOneShot(clip);
        }
    }
    #endregion
    public void LoadingMoveGamePlay()
    {
        GoGamePlay();
        TaskUtil.Delay(this, delegate
        {
            LoadindMoveGamePlayFadeInComplete();
        }, 2);
    }
    public void GoGamePlay()
    {
        state = STATE_RACING_RUN.GAMEPLAY;
        canvas.renderMode = RenderMode.WorldSpace;
        Destroy(objSelect);
        objGamePlay.SetActive(true);
        objBlocker.SetActive(true);
        for (int i = 0; i < charactersController.Count; i++)
        {
            charactersController[i].StartWarmUp();
        }
        switch (typeCharacter)
        {
            case TYPE_RACING_RUN.BUNNY:
                charactersController[0].transform.localPosition = new Vector3(-355, -134, -1);
                charactersController[1].transform.localPosition = new Vector3(-310, -23, -1);
                charactersController[2].transform.localPosition = new Vector3(-335, 68, -1);
                charactersController[0].GetComponent<MeshRenderer>().sortingOrder = 3;
                charactersController[1].GetComponent<MeshRenderer>().sortingOrder = 2;
                charactersController[2].GetComponent<MeshRenderer>().sortingOrder = 1;
                charactersController[0].GetComponent<CharacterType>().line = 1;
                charactersController[1].GetComponent<CharacterType>().line = 2;
                charactersController[2].GetComponent<CharacterType>().line = 3;
                break;
            case TYPE_RACING_RUN.TURTLE:
                charactersController[2].transform.localPosition = new Vector3(-335, -134, -1);
                charactersController[1].transform.localPosition = new Vector3(-310, -23, -1);
                charactersController[0].transform.localPosition = new Vector3(-335, 68, -1);
                charactersController[2].GetComponent<MeshRenderer>().sortingOrder = 3;
                charactersController[1].GetComponent<MeshRenderer>().sortingOrder = 2;
                charactersController[0].GetComponent<MeshRenderer>().sortingOrder = 1;
                charactersController[2].GetComponent<CharacterType>().line = 1;
                charactersController[1].GetComponent<CharacterType>().line = 2;
                charactersController[0].GetComponent<CharacterType>().line = 3;
                break;
            case TYPE_RACING_RUN.SQUIREL:
                charactersController[1].transform.localPosition = new Vector3(-335, -134, -1);
                charactersController[2].transform.localPosition = new Vector3(-310, -23, -1);
                charactersController[0].transform.localPosition = new Vector3(-335, 68, -1);
                charactersController[1].GetComponent<MeshRenderer>().sortingOrder = 3;
                charactersController[2].GetComponent<MeshRenderer>().sortingOrder = 2;
                charactersController[0].GetComponent<MeshRenderer>().sortingOrder = 1;
                charactersController[1].GetComponent<CharacterType>().line = 1;
                charactersController[2].GetComponent<CharacterType>().line = 2;
                charactersController[0].GetComponent<CharacterType>().line = 3;
                break;
        }
        StartCoroutine(TimeWait());
    }
    public IEnumerator TimeWait()
    {
        yield return new WaitForSeconds(6);
        PlaySE(timeAudio);
        objTime.SetActive(true);
        imgTime.sprite = time3;
        yield return new WaitForSeconds(1);
        imgTime.sprite = time2;
        yield return new WaitForSeconds(1);
        imgTime.sprite = time1;
        yield return new WaitForSeconds(1);
        objTime.SetActive(false);
        for (int i = 0; i < charactersController.Count; i++)
        {
            charactersController[i].StartRun();
        }

        isStartGame = true;
        sourceRun.Play();
        GenerateTurnFirst();
    }
    public void GenerateTurnFirst()
    {
        TaskUtil.Delay(this, delegate
        {
            ShowQuestion();
        }, 4);
    }
    public void ShowQuestion()
    {
        stateChoose = STATE_CHOOSE.NONE;
        objQuestionAndAnswer.SetActive(true);
        objBlocker.SetActive(true);
        AudioClip clip = questions[currentQuestion].clipQuestion;
        PlaySE(clip);
        Debug.Log("lỗi đến đây");
        TaskUtil.Delay(this, delegate
        {
            objBlocker.SetActive(false);
            timeOutTurn.SetAliveTime(timeCountDown);
            timeOutTurn.enabled = true;
            countDownTurn.gameObject.SetActive(true);
            for (int i = 0; i < answers.Count; i++)
            {
                answers[i].PlayAnimation();
            }

        }, clip.length);
        for (int i = 0; i < questions[currentQuestion].answerList.Count; i++)
        {
            answers[i].InitAnswer(questions[currentQuestion].answerList[i].answer, questions[currentQuestion].answerList[i].Clip);
        }
    }
    IEnumerator DelayPlayAnswerAudio(float time, AudioClip clip)
    {
        yield return new WaitForSeconds(time);
        PlaySE(clip);
    }
    public void LoadindMoveGamePlayFadeInComplete()
    {
        Loading.gameObject.SetActive(false);
        PlaySE(clipWelcome);
    }
    public void CheckAnswer(string answerInput)
    {
        objBlocker.SetActive(true);
        PlaySE(clickAudio);
        if (questions[currentQuestion].answerCorrect.Equals(answerInput))
        {
            TurnCorrect();

            return;
        }
        else
        {
            TurnWrong();

            return;
        }
    }
    private void TurnCorrect()
    {
        TurnSendAnswearsCorrects[currentQuestion].SetActive(true);
        stateChoose = STATE_CHOOSE.CORRECT;
        PlaySE(correctAudio);
        objBlocker.SetActive(true);
        countDownTurn.gameObject.SetActive(false);
        for (int i = 0; i < answers.Count; i++)
        {
            answers[i].ShowResult();
        }
        TaskUtil.Delay(this, delegate
        {
            ApplyAnimationResult();
            timeOutTurn.enabled = false;
            objQuestionAndAnswer.SetActive(false);
        }, 1.5f);
    }
    public void TurnWrong()
    {
        TurnSendAnswearsWrongs[currentQuestion].SetActive(true);
        objBlocker.SetActive(true);
        stateChoose = STATE_CHOOSE.WRONG;
        PlaySE(wrongAudio);
        countDownTurn.gameObject.SetActive(false);
        for (int i = 0; i < answers.Count; i++)
        {
            answers[i].ShowResult();
        }
        TaskUtil.Delay(this, delegate
        {
            ApplyAnimationResult();
            timeOutTurn.enabled = false;
            objQuestionAndAnswer.SetActive(false);
        }, 1.5f);
    }
    public string getAnswerCorrect()
    {
        return (questions[currentQuestion].answerCorrect);
    }
    public void ApplyAnimationResult()
    {
        PlaySE(runFastAudio);
        characterTypesBackup = charactersController;
        characterTypesBackup.Remove(mainCharacter);
        if (stateChoose == STATE_CHOOSE.CORRECT)
        {
            //correct//
            mainCharacter.StartRunSpeed();
            for (int i = 0; i < characterTypesBackup.Count; i++)
            {
                characterTypesBackup[i].StartSad();
            }
        }
        else
        {
            int rd = UnityEngine.Random.Range(0, 2);
            mainCharacter.StartSad();
            Debug.Log("random false: " + rd);

            int rdChar = UnityEngine.Random.Range(0, 2);
            Debug.Log("random char: " + rd);
            //all//
            for (int i = 0; i < characterTypesBackup.Count; i++)
            {
                if (i == rdChar)
                {
                    characterTypesBackup[i].StartRunSpeed();
                }
                else
                {
                    characterTypesBackup[i].StartSad();
                }
            }
        }
        if (currentQuestion == (totalQuestion - 1))
        {
            GetResultFinal();
            return;
        }
        else
        {
            TaskUtil.Delay(this, delegate
            {
                currentQuestion += 1;
                Debug.Log(currentQuestion);
                currentQuestion = currentQuestion == (totalQuestion - 1) ? (totalQuestion - 1) : currentQuestion;
                ShowQuestion();
            }, timeAppear);
        }
    }
    void GetResultFinal()
    {
        objLineWine.SetActive(true);
        var charactersSort = charactersCompare.OrderByDescending(x => x.score).ThenByDescending(y => y.isMainCharacter);
        int i = -1;
        foreach (CharacterType type in charactersSort)
        {
            i += 1;
            Debug.Log("Score: " + type.name + " " + type.score);
            RandomPositionEnd(getLineStop(type.line).gameObject, i);
        }
        //lineStop1.gameObject.SetActive(true);
        //lineStop2.gameObject.SetActive(true);
        //lineStop3.gameObject.SetActive(true);
    }
    void RandomPositionEnd(GameObject obj, int slot)
    {
        if (slot == 0)
        {
            obj.transform.localPosition = new Vector2(2052, obj.transform.localPosition.y);
        }
        else if (slot == 1)
        {
            obj.transform.localPosition = new Vector2(1970, obj.transform.localPosition.y);
        }
        else if (slot == 2)
        {
            obj.transform.localPosition = new Vector2(2120, obj.transform.localPosition.y);
        }
        obj.SetActive(true);
        Debug.Log("sort: " + obj.name + " " + obj.transform.localPosition);
    }
    Transform getLineStop(int line)
    {
        switch (line)
        {
            case 1:
                return lineStop1;
            case 2:
                return lineStop2;
            case 3:
                return lineStop3;
        }
        return null;
    }
    public void CheckStopBg()
    {
        numEndOfCharacter += 1;
        if (numEndOfCharacter == 1)
        {
            PlaySE(clipVictory);
        }
        if (numEndOfCharacter == charactersController.Count)
        {
            isStartGame = false;
            PauseSE();
            TaskUtil.Delay(this, delegate
            {
                ShowRank();
            }, 3);
        }
    }
    public void ShowRank()
    {
        Loading.gameObject.SetActive(true);
        //Loading.DOFade(1, 1).OnComplete(LoadingMoveRankFadeInComplete);
        TaskUtil.Delay(this, delegate
        {
            LoadingMoveRankFadeInComplete();
        }, 1);
    }
    void LoadingMoveRankFadeInComplete()
    {
        state = STATE_RACING_RUN.RESULT;
        Loading.gameObject.SetActive(true);
        //objGamePlay.SetActive(false);
        objResult.SetActive(true);
        objLineWine.SetActive(false);
        PlaySECongratulation(clipCongratulation);
        int count = -1;
        var charactersSort = charactersCompare.OrderByDescending(x => x.score).ThenByDescending(y => y.isMainCharacter);
        foreach (CharacterType type in charactersSort)
        {
            type.gameObject.transform.SetParent(objResult.transform);
            count += 1;
            if (count == 0)
            {
                type.transform.position = top1.transform.position;
                type.StartCongratulation();
            }
            else if (count == 1)
            {
                type.transform.position = top2.transform.position;
                type.StartIdle();
            }
            else if (count == 2)
            {
                type.transform.position = top3.transform.position;
                type.StartIdle();
            }
            ResizeCharacter(type);
        }
        objGamePlay.SetActive(false);
        for (int i = 0; i < charactersCompare.Count; i++)
        {
            charactersCompare[i].isEnd = false;
        }
        TaskUtil.Delay(this, delegate
        {
            LoadingMoveRankFadeOutComplete();
        }, 1);
    }
    void ResizeCharacter(CharacterType characterType)
    {
        switch (characterType.type)
        {
            case TYPE_RACING_RUN.TURTLE:
                characterType.gameObject.transform.localScale = new Vector3(60, 60, 60);
                break;
            case TYPE_RACING_RUN.SQUIREL:
                characterType.gameObject.transform.localScale = new Vector3(45, 45, 45);
                break;
            case TYPE_RACING_RUN.BUNNY:
                characterType.gameObject.transform.localScale = new Vector3(60, 60, 60);
                break;
        }
    }
    void LoadingMoveRankFadeOutComplete()
    {
        Loading.gameObject.SetActive(false);

    }
    void PauseSE()
    {
        if (sourceRun)
        {
            sourceRun.Pause();
        }
    }
    void PlaySECongratulation(AudioClip clip)
    {
        if (sourceRun)
        {
            sourceRun.clip = clip;
            sourceRun.loop = false;
            sourceRun.PlayOneShot(clip);
        }
    }
    public bool isStateResult()
    {
        return (state == STATE_RACING_RUN.RESULT);
    }
    public void CheckRoundFinal()
    {
        //move end
        if (currentQuestion == (totalQuestion - 1))
        {
            TaskUtil.Delay(this, delegate
            {
                var charactersSort = charactersCompare.OrderByDescending(x => x.score).ThenByDescending(y => y.isMainCharacter);
                int i = -1;
                foreach (CharacterType type in charactersSort)
                {
                    i += 1;
                    Debug.Log("Score: " + type.name + " " + type.score);

                    switch (i)
                    {
                        case 0:
                            type.GoToLineWin(true, getLineStop(type.line));
                            PlaySE(type.runWinner);
                            break;
                        case 1:
                            type.GoToLineWin(false, getLineStop(type.line));
                            break;
                        case 2:
                            type.GoToLineWin(false, getLineStop(type.line));
                            break;
                    }
                }
            }, 4);

        }
    }
}


