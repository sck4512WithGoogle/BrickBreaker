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
        //ù ��°���� �� ��
        for (int i = 1; i < images.Length; i++)
        {
            images[i].SetActive(false);
        }


        //�̰� ����
        panel.SetActive(true);
        //1�� ����
        currentIndex = 1;
        //�ε����� �°� �ؽ�Ʈ ������Ʈ
        SetCurrentIndex(currentIndex);
        //�̹����� Ŵ
        images[currentIndex - 1].SetActive(true);
    }

    public void OnClickTouchRange()
    {
        if(currentIndex == images.Length)
        {
            panel.SetActive(false);
            return;
        }

        //���ְ�
        images[currentIndex - 1].SetActive(false);
        //�ε��� �÷���
        ++currentIndex;
        SetCurrentIndex(currentIndex);
        images[currentIndex - 1].SetActive(true);
    }
}
