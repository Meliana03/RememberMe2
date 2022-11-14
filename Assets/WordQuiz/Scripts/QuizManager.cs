using System.Collections;
using System.Collections.Generic;
//using System;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{

    public static QuizManager instance;
    [SerializeField] private QuizDataScriptable questionData;

    [SerializeField] private Text questionText;

    [SerializeField] private WordData[] answerWordArray;  //awserWordPrefab;
    [SerializeField] private WordData[] optionsWordArray;

    private char[] charArray = new char[12];  // 12 dri box answer
    private int currentAnswerIndex = 0;
    private bool correctAnswer = true;
    private List<int> selectedWordIndex;

    private int currentQuestionIndex = 0;   // index untuk memberitahu skrng di pertanyaan ke brp
    private GameStatus gameStatus = GameStatus.Playing;
    private string answerWord; // Jawaban dari soal yg ditampilkan
    private int score = 0;

    //Random Question
    private List<int> randomIndex = new List<int>();
    private int a = 0;

    // Time
    float currentTime;
    public float timeRemaining = 25f;
    [SerializeField] Text countdownText;

    // Score / current correct answer
    [SerializeField] Text scoreText;
    float currentScore;
    float currentCorrectAnswer = 0f;

    //Pass
    public Text buttonText;
    float currentPass = 5f;
    public Button buttonPass;
    private int[] PassIndex = new int [5];
    private int countPassIndex = 0;
    private int countPassIndex2 = 0;

    //Clue
    public Text cluebuttonText;
    float currentClue = 5f;
    public Button canUseClue;
    private int[] clueIndex = new int [5];
    private int countIndex = 0;

    //PopUp salah
    private bool wrongAnswer = false;
    private float popUpCurrentTime = 0.0f;
    private float timeToWait = 3.0f;
    [SerializeField] private GameObject popUp;
    
    //PopUp Benar
    private float popUpCATime = 0.0f;
    private float CAtimeToWait = 3.0f;
    [SerializeField] private GameObject popUpBenar;

    //PopUp Lose
    private float popUpLoseTime = 0.0f;
    private float timeToLose = 3.0f;
    [SerializeField] private GameObject popUpLose;

    //PopUp Win
    [SerializeField] private GameObject popUpWin;
    private float popUpWinTime = 0.0f;
    private float timeToWin = 3.0f;

    //Hp
    public int CountHp;
    [SerializeField] private Image[] hearts;

    //WrongAlphabet
    private int[] WrongIndex = new int [10];
    private int countWrongIndex = 0;
 
    private void Awake()
    {
        if(instance == null) instance = this;
        else
            Destroy(this.gameObject);

        selectedWordIndex = new List<int>();

    }

    public void Start()
    {
        RandomQuestionIndex();
        setQuestion();

        currentTime = timeRemaining;
        buttonPass.enabled = true;
        canUseClue.enabled = true;

        for(int i = 0; i < 5; i++)
        {
            PassIndex[i] = -999;
        }

        for(int i = 0; i < 10; i++)
        {
            WrongIndex[i] = -999;
        }
        
    }

    public void Update()
    {
        currentTime -= 1 * Time.deltaTime;
        countdownText.text = currentTime.ToString("0");

        if(currentTime <= 0)
        {
            currentTime = 0;
            UpdateHealth();
        }
        if(currentTime == 0 && a != 29)
        {
            currentTime = timeRemaining;
            setQuestion();
        }

        if(currentCorrectAnswer == 15 && Time.timeScale == 1.0)
        {
            //Time.timeScale = 0f;
            popUpWin.SetActive(true);
            popUpWinTime += Time.deltaTime;
            if(popUpWinTime >= timeToWin)
            {
                SceneManager.LoadScene(sceneName: "WinScene");
            }
        }
        else if(currentCorrectAnswer != 15 && a == 29 && countPassIndex2 == 5)
        {
            SceneManager.LoadScene(sceneName: "LoseScene");
        }
        else if(CountHp == 0)
        {
            popUpLose.SetActive(true);
            popUpLoseTime += Time.deltaTime;
            if(popUpLoseTime >= timeToLose)
            {
                SceneManager.LoadScene(sceneName: "LoseScene");
            }
        }

        if(wrongAnswer == true)
        {
            popUpCurrentTime += Time.deltaTime;
            if(popUpCurrentTime >= timeToWait)
            {
                popUpCurrentTime = 0.0f;
                popUp.SetActive(false);
                wrongAnswer = false;
                for(int i = 0; i < answerWord.Length; i++)
                {
                   if(char.ToUpper(answerWord[i]) == char.ToUpper(answerWordArray[i].charValue))
                    {
                        WrongIndex[i] = -999;
                    }
                }
            }
            else
            {
                popUp.SetActive(true);
            }
        }
        else
        {
            popUp.SetActive(false);
        }

        if(correctAnswer == true)
        {
            popUpCATime += Time.deltaTime;
            if(popUpCATime >= CAtimeToWait)
            {
                popUpCATime = 0.0f;
                popUpBenar.SetActive(false);
                correctAnswer = false;
            }
            else
            {
                popUpBenar.SetActive(true);
            }
        }
        else
        {
            popUpBenar.SetActive(false);
        }
    }

    public void UpdateHealth()
    {
        CountHp = CountHp - 1;
        for(int i = 0; i < hearts.Length; i++)
        {
            if(i < CountHp)
            {
                hearts[i].color = Color.white;
            }
            else
            {
                hearts[i].color = Color.gray;
            }
        }
    }

    public void RandomQuestionIndex()
    {
        int index = 0;
        List<int> tempIndexList = new List<int>();
        for(int i = 0; i < questionData.questions.Count; i++)
        {
            tempIndexList.Add(i);
        }
        for(int i = 0; i < questionData.questions.Count;i++)
        {
            do
            {
                index = Random.Range(0, tempIndexList.Count);
            }while(randomIndex.Contains(tempIndexList[index]));
            
            randomIndex.Add(tempIndexList[index]);
            tempIndexList.RemoveAt(index);
           // Debug.Log(randomIndex[i]);
        }
    }

    private void setQuestion()
    {
        currentAnswerIndex = 0;
        selectedWordIndex.Clear();
        currentQuestionIndex = randomIndex[a];

        questionText.text = questionData.questions[currentQuestionIndex].questionText;
        answerWord = questionData.questions[currentQuestionIndex].answer;

        ResetQuestion();

        for(int i=0; i < answerWord.Length; i++)
        {
            charArray[i] = char.ToUpper(answerWord[i]);
        }

        //Random Huruf sisa (yg gk dipakai)
        for(int i = answerWord.Length; i < optionsWordArray.Length; i++)
        {
            charArray[i] = (char)UnityEngine.Random.Range(65, 91);
        }

        //Mengacak huruf
        charArray = ShuffleList.ShuffleListItems<char>(charArray.ToList()).ToArray();

        for(int i=0; i < optionsWordArray.Length; i++)
        {
            optionsWordArray[i].SetChar(charArray[i]);
        }
       
        if(a  == 29 && currentCorrectAnswer != 15)
        {
            currentQuestionIndex = PassIndex[countPassIndex2];
            countPassIndex2++;
            setQuestion();
        }
        else
        {
            //currentQuestionIndex++;
            a++;
        }

        gameStatus = GameStatus.Playing;
    }

    public void SelectedOption(WordData WordData)
    {
        if(gameStatus == GameStatus.Next || currentAnswerIndex >= answerWord.Length) return;

        //Debug.Log(clueIndex.Length);
        for(int i = 0; i < clueIndex.Length; i ++)
        {
            //Misal index saat ini == clue index ( ada isinya ), index ++ (lewatin kolomnya)
            if (currentAnswerIndex == clueIndex[i]) 
            {
                currentAnswerIndex++;
            }
        }

        if(selectedWordIndex.Contains(-999))
            selectedWordIndex[selectedWordIndex.IndexOf(-999)] = WordData.transform.GetSiblingIndex();
        else
            selectedWordIndex.Add(WordData.transform.GetSiblingIndex());

        answerWordArray[currentAnswerIndex].SetChar(WordData.charValue); //kolom jawaban (_) sesuai dgn currentAnswerIndex misal index 0 brti garis ke 1
        WordData.gameObject.SetActive(false); //menghilangkan huruf yg sudah di pilih
        currentAnswerIndex++;

        // selectedWordIndex.Add(WordData.transform.GetSiblingIndex());
         for(int i = 0; i < answerWord.Length; i++)
        {
            if(answerWordArray[currentAnswerIndex].charValue != '_')
            {
                currentAnswerIndex++;
            }
        }
        Scoring();
        // answerWordArray[currentAnswerIndex].SetChar(WordData.charValue); //kolom jawaban (_) sesuai dgn currentAnswerIndex misal index 0 brti garis ke 1
        // WordData.gameObject.SetActive(false); //menghilangkan huruf yg sudah di pilih
        // currentAnswerIndex++;

        //Debug.Log(CountHp);
        // if(currentAnswerIndex >= answerWord.Length)
        // {
        //     correctAnswer = true;

        //     for(int i = 0; i < answerWord.Length; i++)
        //     {
        //         if(char.ToUpper(answerWord[i]) != char.ToUpper(answerWordArray[i].charValue)) //jika jawaban di (_) gk sama sm jawaban di answer diinspector
        //         {
        //             correctAnswer = false;
        //             break;
        //         }
        //     }

        //     if(currentCorrectAnswer <= 15)
        //     {
        //         if(correctAnswer)
        //         {
        //             gameStatus = GameStatus.Next; 
        //             score += 1;
        //             currentCorrectAnswer += 1;
        //             scoreText.text = currentScore.ToString(score + " /");
        //             // if(currentQuestionIndex < questionData.questions.Count)
        //             // {
        //             //     Invoke("setQuestion", 0.7f); //Panggil set Question waktunya 0.5  
        //             // }
        //             if(currentCorrectAnswer != 15)
        //             {
        //                 Invoke("setQuestion", 0.7f); //Panggil set Question waktunya 0.5  
        //             }
        //         }
        //         else if(!correctAnswer)
        //         {
        //            // Debug.Log("False");
        //             wrongAnswer = true;
        //             UpdateHealth();
        //             EraseWrongAlphabet();

        //             if(popUpCurrentTime == timeToWait)
        //             {
        //                 wrongAnswer = false;
        //             }
                
        //         }
        //     }    
        //     else if(currentCorrectAnswer == 15)
        //     {
        //         Time.timeScale = 0f;
        //         SceneManager.LoadScene(sceneName: "WinScene");
        //     }
        // }
    }

    private void Scoring()
    {
        if(currentAnswerIndex >= answerWord.Length)
        {
            correctAnswer = true;

            for(int i = 0; i < answerWord.Length; i++)
            {
                if(char.ToUpper(answerWord[i]) != char.ToUpper(answerWordArray[i].charValue)) //jika jawaban di (_) gk sama sm jawaban di answer diinspector
                {
                    correctAnswer = false;
                    break;
                }
            }

            if(currentCorrectAnswer <= 15)
            {
                if(correctAnswer)
                {
                    gameStatus = GameStatus.Next; 
                    score += 1;
                    currentCorrectAnswer += 1;
                    scoreText.text = currentScore.ToString(score + " /");
                    // if(currentQuestionIndex < questionData.questions.Count)
                    // {
                    //     Invoke("setQuestion", 0.7f); //Panggil set Question waktunya 0.5  
                    // }
                    if(currentCorrectAnswer != 15)
                    {
                        Invoke("setQuestion", 0.7f); //Panggil set Question waktunya 0.5  
                    }
                }
                else if(!correctAnswer)
                {
                   // Debug.Log("False");
                    wrongAnswer = true;
                    UpdateHealth();
                    EraseWrongAlphabet();

                    if(popUpCurrentTime == timeToWait)
                    {
                        wrongAnswer = false;
                    }
                
                }
            }    
            else if(currentCorrectAnswer == 15)
            {
                Time.timeScale = 0f;
                SceneManager.LoadScene(sceneName: "WinScene");
            }
        }
    }

    //reset kolom jawaban dan kolom huruf ke awal (full semua)
    private void ResetQuestion()
    {
        for(int i = 0; i < answerWordArray.Length; i++)
        {
            answerWordArray[i].gameObject.SetActive(true);
            answerWordArray[i].SetChar('_');
        }
        //Hide kolom jawaban (_) yang gak kepakai
        for(int i = answerWord.Length; i < answerWordArray.Length; i++)
        {
            answerWordArray[i].gameObject.SetActive(false);
        }

        //Reset option answer / huruf
        for(int i = 0; i < optionsWordArray.Length; i++)
        {
            optionsWordArray[i].gameObject.SetActive(true);
        }

        for(int i = 0; i < 5; i++)
        {
            clueIndex[i] = -999;
           
        }

        for(int i = 0; i < 10; i++)
        {
            WrongIndex[i] = -999;
        }

        countIndex = 0;
        popUpCurrentTime = 0.0f;
        wrongAnswer = false;
        currentTime = timeRemaining;
        popUpCATime = CAtimeToWait;
        countWrongIndex = 0;
    }

    public void ResetLastWord()
    {
        if(selectedWordIndex.Count > 0)
        {
            int index = selectedWordIndex[selectedWordIndex.Count - 1];
            optionsWordArray[index].gameObject.SetActive(true);
            selectedWordIndex.RemoveAt(selectedWordIndex.Count - 1);

            currentAnswerIndex--;
            answerWordArray[currentAnswerIndex].SetChar('_');
        }

        if(wrongAnswer == true)
        {
            wrongAnswer = false;
        }
    }

    void ResetWrongIndex()
    {
        for(int i = 0; i < WrongIndex.Length; i++)
        {
            WrongIndex[i] = -999;
        }
    }

     private void EraseWrongAlphabet()
    {
        int index, a = 0;
        countWrongIndex = 0;
        ResetWrongIndex();
        for (int i = 0; i < answerWord.Length; i++)
        {
            Debug.Log(char.ToUpper(answerWord[i]) + " vs " + char.ToUpper(answerWordArray[i].charValue) + " = " + (char.ToUpper(answerWord[i]) != char.ToUpper(answerWordArray[i].charValue)));
            if(char.ToUpper(answerWord[i]) != char.ToUpper(answerWordArray[i].charValue))
            {
                WrongIndex[countWrongIndex] = i;
                
                countWrongIndex++;
               
            }
        }

        while(a < countWrongIndex)
        {
            //menghapus _ di index yang hurufnya salah
            Debug.Log("index: " + selectedWordIndex[WrongIndex[a]]);
            index = selectedWordIndex[WrongIndex[a]];
            optionsWordArray[index].gameObject.SetActive(true);
            //selectedWordIndex.RemoveAt(WrongIndex[a]);/// ini penyebab error yg kemarin, mending dijadiin -999 aja indexnya
            selectedWordIndex[WrongIndex[a]] = -999;

            //Mengganti huruf yg salah jadi _
            currentAnswerIndex = WrongIndex[a];
            answerWordArray[currentAnswerIndex].SetChar('_');
            a++;
        }
        currentAnswerIndex = WrongIndex[0];
        a = 0;
        //index = 0;
    }
    
    public void PassBtnNewText()
    {
        if(currentPass > 0)
        {
            currentPass = currentPass - 1;
            buttonText.text = "x" + currentPass;
            
            //PassIndex[countPassIndex] = currentQuestionIndex - 1; //currentQuestionIndex?
            PassIndex[countPassIndex] = randomIndex[a];
            countPassIndex += 1;
            setQuestion();
        }
        else 
        {
            buttonPass.enabled = false;
        }
    }

    private void CheckIndexClue()
    {
        for(int i = 0; i < clueIndex.Length; i ++)
        {
            //Misal index saat ini == clue index ( ada isinya ), index ++ (lewatin kolomnya)
            if (currentAnswerIndex == clueIndex[i]) 
            {
                currentAnswerIndex++;
            }
        }
    }

    public void ClueBtnText()
    {
        int a = 0;
        if(currentClue > 0)
        {
            currentClue = currentClue - 1;
            cluebuttonText.text = "x" + currentClue;

            for(int i = 0; i < answerWord.Length; i++)
            {
                if(char.ToUpper(answerWord[i]) == char.ToUpper(answerWord[currentAnswerIndex])) // mencari apakah ada huruf yg sama dgn index saat ini
                {
                    answerWordArray[i].SetChar(char.ToUpper(answerWord[i])); // tulis huruf yg ada di kolom jawaban
                    clueIndex[countIndex] = i;
                    countIndex += 1;
                
                  // Hapus opsi pilihan huruf yang sudah ditampilin di clue
                  for(int j = 0; j < charArray.Length; j++)
                    {
                        if(charArray[j] == char.ToUpper(answerWord[i]))
                        {
                            optionsWordArray[j].gameObject.SetActive(false);
                        }
                    }
                }
            }
           CheckIndexClue();

           for(int i = 0; i< answerWord.Length; i++)
            {
                if(char.ToUpper(answerWordArray[i].charValue) != '_') 
                {    
                    Debug.Log(clueIndex[a]);
                    if(char.ToUpper(answerWordArray[i].charValue) != char.ToUpper(answerWordArray[clueIndex[a]].charValue))
                    {
                        answerWordArray[i].SetChar('_');
                    }
                    else
                    {
                        currentAnswerIndex++;
                        a++;
                    }
                }

                for(int j = 0; j < charArray.Length; j++)
                {
                    if(charArray[j] == char.ToUpper(answerWord[i]))
                    {
                        optionsWordArray[j].gameObject.SetActive(true);
                    }
                }
            }
            a = 0;

            for(int i = 0; i< answerWord.Length; i++)
            {
                if(char.ToUpper(answerWordArray[i].charValue) != '_')
                {
                    currentAnswerIndex++;
                }
                else
                {
                    currentAnswerIndex = i;
                    //Debug.Log(i);
                    break;
                }
            }

        //   Kalau currentAnswerIndex == panjang jawaban - 1 (kalau clue di pakai di kolom paling terakhir) cek apakah jawaban benar atau gk
           if(currentAnswerIndex == answerWord.Length)
           {
                correctAnswer = true;
                for(int i = 0; i < answerWord.Length; i++)
                {
                    if(char.ToUpper(answerWord[i]) != char.ToUpper(answerWordArray[i].charValue)) //jika jawaban di (_) gk sama sm jawaban di answer diinspector
                    {
                        correctAnswer = false;
                        break;
                    }
                }

                if(correctAnswer)
                {
                    gameStatus = GameStatus.Next; 
                    score += 1;
                    currentCorrectAnswer += 1;
                    scoreText.text = currentScore.ToString(score + " / 15");
                    if(currentCorrectAnswer != 15)
                    {
                        Invoke("setQuestion", 0.7f); //Panggil set Question waktunya 0.5  
                    }
                }
                else if(!correctAnswer)
                {
                    wrongAnswer = true;
                    //currentTime = currentTime - 2;

                    if(popUpCurrentTime == timeToWait)
                    {
                        wrongAnswer = false;
                    } 
                }
            }
        }
        else 
        {
            canUseClue.enabled = false;
        }
    }

    public void HomeBtn()
    {
        SceneManager.LoadScene(sceneName: "CoverScene");
    }
}



[System.Serializable]
public class QuestionData
{
    //public Sprite questionImage;
    public string questionText;
    public string answer;
}

public enum GameStatus
{
    Playing,
    Next
}
