using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[DisallowMultipleComponent]
public class TutorialUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject[] images;
    [SerializeField] private TextMeshProUGUI currentIndexText;
    private int currentIndex;

    private void SetCurrentIndex(int _CurIndex)
    {
        currentIndexText.text = $"{_CurIndex}/{images.Length}";
    }

    public void OnClickTutorialOn()
    {
        //Ã¹ ¹øÂ°»©°í ´Ù ²û
        for (int i = 1; i < images.Length; i++)
        {
            images[i].SetActive(false);
        }


        //ÀÌ°Å ÄÑÁÜ
        panel.SetActive(true);
        //1·Î ÇØÁÜ
        currentIndex = 1;
        //ÀÎµ¦½º¿¡ ¸Â°Ô ÅØ½ºÆ® ¾÷µ¥ÀÌÆ®
        SetCurrentIndex(currentIndex);
        //ÀÌ¹ÌÁöµµ Å´
        images[currentIndex - 1].SetActive(true);
    }

    public void OnClickTouchRange()
    {
        if(currentIndex == images.Length)
        {
            panel.SetActive(false);
            return;
        }

        //²¨ÁÖ°í
        images[currentIndex - 1].SetActive(false);
        //ÀÎµ¦½º ¿Ã·ÁÁÜ
        ++currentIndex;
        SetCurrentIndex(currentIndex);
        images[currentIndex - 1].SetActive(true);
    }
}
