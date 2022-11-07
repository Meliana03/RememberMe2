using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToCoverScript : MonoBehaviour
{
    [SerializeField] private GameObject showPopUp;
    //public int clicked = 1;

    public void Retry()
    {
        SceneManager.LoadScene(sceneName: "TutorialScene");
    }

    public void Next()
    {
        SceneManager.LoadScene(sceneName: "CoverScene");
    }

    public void PopUP()
    {
            showPopUp.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
