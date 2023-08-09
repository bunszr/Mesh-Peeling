using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PeeledPercentagePrinter : MonoBehaviour
{
    [SerializeField] Text textMesh;
    [SerializeField] Image sliderImage;
    PeelingMesh peelingMesh;
    int oldPercentOfPeeling = -1;
    private void Start()
    {
        peelingMesh = FindObjectOfType<PeelingMesh>();
    }

    private void Update()
    {
        sliderImage.fillAmount = peelingMesh.PercentOfPeeling;
        int percentInt = Mathf.RoundToInt(peelingMesh.PercentOfPeeling * 100f);
        if (percentInt != oldPercentOfPeeling)  // UniRx solve this problem...
        {
            oldPercentOfPeeling = percentInt;
            textMesh.text = "%" + Mathf.RoundToInt(peelingMesh.PercentOfPeeling * 100f);
        }
    }
}