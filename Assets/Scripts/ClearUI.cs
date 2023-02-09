using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearUI : MonoBehaviour
{
    public void LogoSE()
    {
        SoundManager.I.PlaySE(SESoundData.SE.LOGO);
    }
}
