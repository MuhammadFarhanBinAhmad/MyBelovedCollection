using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UI_PlayerCombo : MonoBehaviour
{
    [SerializeField] SO_Combo _Combo;

    [SerializeField] TextMeshProUGUI text_LevelCombo;
    [SerializeField] TextMeshProUGUI text_NumberCombo;

    Dictionary<int,int> _comboText = new Dictionary<int,int>();

    float text_duration; // Total time for bump
    float text_timer;
    float level_sizeStart, counter_sizeStart;
    float level_sizePeak, counter_sizePeak;
    float level_sizeEnd, counter_sizeEnd;


    public static UI_PlayerCombo Instance { get; private set; }

    void OnEnable()
    {
        Instance = this;
        text_duration = _Combo.text_duration;
        level_sizeStart = _Combo.level_sizeStart;
        level_sizePeak = _Combo.level_sizePeak;
        level_sizeEnd = _Combo.level_sizeEnd;

        counter_sizeStart = _Combo.counter_sizeStart;
        counter_sizePeak = _Combo.counter_sizePeak;
        counter_sizeEnd = _Combo.counter_sizeEnd;


        ResetText();
    }
    void Update()
    {
        PopPushTextSize();
    }

    void ResetText()
    {
        text_LevelCombo.text = "";
    }
    public void ChangeComboNumberLevelText(int combocount)
    {

        if (combocount == 0)
        {
            text_LevelCombo.text = "";
            return;
        }

        text_NumberCombo.text = "x" + combocount.ToString();
        foreach (var level in _Combo.levels)
        {
            if (combocount == level.requisite)
            {
                if (!string.IsNullOrEmpty(level.levelName))
                {
                    text_LevelCombo.text =
                    $"<size={level.sizePercent}%><color=#{UnityEngine.ColorUtility.ToHtmlStringRGB(level.color)}>{level.levelName[0]}</color></size>{level.levelName.Substring(1)}";
                }

                text_timer = 0;
                return;
            }
        }
        text_timer = 0;

    }

    void PopPushTextSize()
    {
        if (text_timer > text_duration)
            return;

        float t = text_timer / text_duration;
        // Animate size
        float l_newSize = EvaluateBump(t, level_sizeStart, level_sizePeak, level_sizeEnd);
        float c_newSize = EvaluateBump(t, counter_sizeStart, counter_sizePeak, counter_sizeEnd);

        if (text_LevelCombo != null)
            text_LevelCombo.fontSize = l_newSize;

        if(text_NumberCombo != null)
            text_NumberCombo.fontSize = c_newSize;

        // Update timer
        text_timer += Time.deltaTime;
    }
    float EvaluateBump(float t, float baseValue, float peakValue, float endValue)
    {
        t = Mathf.Clamp01(t);

        if (t < 0.5f)
        {
            float tt = t / 0.5f;
            return Mathf.Lerp(baseValue, peakValue, SmoothStep(tt));
        }
        else
        {
            float tt = (t - 0.5f) / 0.5f;
            return Mathf.Lerp(peakValue, endValue, SmoothStep(tt));
        }
    }

    float SmoothStep(float x)
    {
        return x * x * (3f - 2f * x);
    }


}
