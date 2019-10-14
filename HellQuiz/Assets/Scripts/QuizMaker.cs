using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

[System.Serializable]
public class Question
{
    public string Q;
    public string trueAns;
    public List<string> Answers = new List<string>();
    public void Shuffle()
    {
        List<string> tmp = new List<string>();
        foreach (string ans in Answers)
        {
            tmp.Add(ans);
        }
        Answers.Clear();
        int qC = tmp.Count;
        for (int i = 0; i < qC; i++)
        {
            int index = Random.Range(0, tmp.Count - 1);
            Answers.Add(tmp[index]);
            tmp.Remove(tmp[index]);
        }
    }
}

[System.Serializable]
public class QuestionScreen
{
    public Text QuestionTitle;
    public Button[] buttons;
}
public class QuizMaker : MonoBehaviour
{
    public string[] Lines;
    public List<Question> Questions;
    public QuestionScreen CurrentScreen;
    public int CurrentQuestion = 0;
    public Text SubCount;
    public int WrongAnswers;
    public AudioClip WaitSound;
    public AudioClip WinSound;
    public AudioClip LoseSound;
    AudioSource AudioPlayer;
    bool ButtonPressed;
    void Start()
    {
        Lines = File.ReadAllLines(@"test.txt");
        SubCount.text = "Неправильный ответ карается очком";
        Debug.Log(Lines);
        for (int i = 0; i < Lines.Length; i += 5)
        {
            Debug.Log(Lines[i]);
            Question q = new Question();
            q.Q = Lines[i];
            for (int j = i + 1; j < i + 5; j++)
            {
                q.Answers.Add(Lines[j]);
            }
            q.trueAns = q.Answers[0];
            q.Shuffle();
            Questions.Add(q);
        }
        SetQuestion();
        AudioPlayer = GetComponent<AudioSource>();
    }

    public void SetQuestion()
    {
        ButtonPressed = false;
        CurrentScreen.QuestionTitle.text = Questions[CurrentQuestion].Q;
        for (int i = 0; i < Questions[CurrentQuestion].Answers.Count; i++)
        {
            CurrentScreen.buttons[i].GetComponentInChildren<Text>().text = Questions[CurrentQuestion].Answers[i];
        }
    }
    public void NextQuestion(int N)
    {
        //Debug.Log("Pressed: " + CurrentScreen.buttons[N-1].GetComponentInChildren<Text>().text);
        //Debug.Log("True: " + Questions[CurrentQuestion].trueAns);
        StopAllCoroutines();
        if (ButtonPressed == false)
        {
            ButtonPressed = true;
            StartCoroutine(DelayAns(N));
        }
        #region old
        /*
        if (ButtonPressed == false)
        {
            if (CurrentScreen.buttons[N - 1].GetComponentInChildren<Text>().text == Questions[CurrentQuestion].trueAns)
            {
                Debug.Log("Verno");
            }
            else
            {
                WrongAnswers++;
                Debug.Log("Ti Pidor");
                SubCount.text = "Гони сабки, мудак. " + WrongAnswers.ToString() + " шт.";
            }
            CurrentQuestion++;
            if (CurrentQuestion >= Questions.Count)
            {
                Debug.Log("ENDGAME");
                return;
            }
            SetQuestion();
        }
        */
        #endregion
    }

    public IEnumerator DelayAns(int N)
    {
        CurrentScreen.buttons[N - 1].image.color = new Vector4(252 / 255F, 1, 0, 1);
        AudioPlayer.PlayOneShot(WaitSound);
        //AudioSource.PlayClipAtPoint(WaitSound, new Vector3(0, 0, 0));
        yield return new WaitForSeconds(WaitSound.length);
       
        if (CurrentScreen.buttons[N - 1].GetComponentInChildren<Text>().text == Questions[CurrentQuestion].trueAns)
        {
            AudioPlayer.PlayOneShot(WinSound);
            //AudioSource.PlayClipAtPoint(WinSound, new Vector3(0, 0, 0));
            while(AudioPlayer.isPlaying)
            {
                CurrentScreen.buttons[N - 1].image.color = new Vector4(18 / 255F, 1, 0, 1);
                yield return new WaitForSeconds(0.2F);
                CurrentScreen.buttons[N - 1].image.color = new Vector4(1, 1, 1, 1);
                yield return new WaitForSeconds(0.2F);
            }
        }
        else
        {
            AudioSource.PlayClipAtPoint(LoseSound, new Vector3(0, 0, 0));
            AudioPlayer.PlayOneShot(LoseSound);
            while (AudioPlayer.isPlaying)
            {
                CurrentScreen.buttons[N - 1].image.color = new Vector4(1, 0, 0, 1);
                yield return new WaitForSeconds(0.2F);
                CurrentScreen.buttons[N - 1].image.color = new Vector4(1, 1, 1, 1);
                yield return new WaitForSeconds(0.2F);
            }
            WrongAnswers++;
            SubCount.text = "Гони сабки, мудак. " + WrongAnswers.ToString() + " шт.";
        }
        CurrentQuestion++;
        if (CurrentQuestion >= Questions.Count)
        {
            Debug.Log("ENDGAME");
        }
        SetQuestion();
    }

    void Update()
    {
        
    }
}
