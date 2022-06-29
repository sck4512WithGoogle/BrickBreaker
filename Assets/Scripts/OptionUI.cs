using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MJ.Manager;

public class OptionUI : MonoBehaviour
{
    [SerializeField] private Graphic soundOptionOnImage;
    [SerializeField] private Graphic soundOptionOffImage;
    [SerializeField] private Graphic aimingOptionOnImage;
    [SerializeField] private Graphic aimingOptionOffImage;

    [Header("컬러")]
    [SerializeField] private Color onColor;
    [SerializeField] private Color offColor;

    private void Start()
    {
        //옵션 초기화
        SetSoundImages(OptionManager.IsSound);
        SetAimingImages(OptionManager.IsAiming);
    }


    public void SetSoundImages(bool _IsOptionActive)
    {
        if(_IsOptionActive)
        {
            soundOptionOnImage.color = onColor;
            soundOptionOffImage.color = offColor;
        }
        else
        {
            soundOptionOnImage.color = offColor;
            soundOptionOffImage.color = onColor;
        }
    }
    public void SetSoundOption(bool _IsOptionActive)
    {
        OptionManager.SetSoundOption(_IsOptionActive);
    }


    public void SetAimingImages(bool _IsOptionActive)
    {
        if (_IsOptionActive)
        {
            aimingOptionOnImage.color = onColor;
            aimingOptionOffImage.color = offColor;
        }
        else
        {
            aimingOptionOnImage.color = offColor;
            aimingOptionOffImage.color = onColor;
        }
    }
    public void SetAimingOption(bool _IsOptionActive)
    {
        OptionManager.SetAimingOption(_IsOptionActive);
    }
}
