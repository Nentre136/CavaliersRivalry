using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderToText : MonoBehaviour
{
    [SerializeField]
    private TMP_Text changeText;
    [SerializeField]
    private Slider slider;
    public void ChangeCountText()
    {
        changeText.text = slider.value.ToString();
    }
}
