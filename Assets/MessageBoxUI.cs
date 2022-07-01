using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MessageBoxUI : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button okButton;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Button exitButton;

    public Button OkButton => okButton;
    public Button ExitButton => exitButton;

    public event Action OnDie;
    private string startOkButtonText;
    private float startOkButtonFontSize;
    private float mainTextFontSize;

    private void Awake()
    {
        startOkButtonText = buttonText.text;
        startOkButtonFontSize = buttonText.fontSize;
        mainTextFontSize = text.fontSize;
    }

    private void OnEnable()
    {
        buttonText.text = startOkButtonText;
        buttonText.fontSize = startOkButtonFontSize;
        text.fontSize = mainTextFontSize;

        okButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
    }

    private void OnDisable()
    {
        OnDie?.Invoke();
        OnDie = null;
    }

    
    public MessageBoxUI SetMessage(string _Message, float _FontSize = 94f)
    {
        text.text = _Message;
        text.fontSize = _FontSize;
        return this;
    }

    public MessageBoxUI SetButtonTextOK()
    {
        buttonText.text = "ok";
        buttonText.fontSize += 5f; //5정도 더 크게 해줌
        return this;
    }
}
