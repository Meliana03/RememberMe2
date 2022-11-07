using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CoverScript : MonoBehaviour
{
    [SerializeField] private GameObject showTutorial;
    [SerializeField] private GameObject showCover;
    public int clicked = 1;
    public Image oldImage;
    public Sprite newImage1;
    public Sprite newImage2;
    public Sprite newImage3;
    public Sprite newImage4;
    public Sprite newImage5;

    public void ButtonStartClicked()
    {
        SceneManager.LoadScene(sceneName: "StoryScene");
    }

    public void ButtonTutorialClicked()
    {
        showTutorial.SetActive(true);
        showCover.SetActive(false);
        ImageChange();
      //  Debug.Log(clicked);

    }

    public void ImageChange()
    {
        //clicked = 1;
        if(clicked == 1)
        {
            oldImage.sprite = newImage1;
            clicked = 2;
        }
        else if(clicked == 2)
        {
           oldImage.sprite = newImage2;
            clicked = 3;

        }
        else if(clicked == 3)
        {
            oldImage.sprite = newImage3;
            clicked = 4;
            
        }
        else if(clicked == 4)
        {
            oldImage.sprite = newImage4;
            clicked = 5;
        }
        else if(clicked == 5)
        {
            oldImage.sprite = newImage5;
            clicked = 6;
        }
        else if(clicked == 6)
        {
            showTutorial.SetActive(false);
            showCover.SetActive(true);
            clicked = 1;
        }
        
    }
}
