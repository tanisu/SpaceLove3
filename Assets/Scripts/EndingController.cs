using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EndingController : MonoBehaviour
{
    [SerializeField] Text endingText;
    public string[] headText;
    bool isStart;
    public void ViewEndingText()
    {
        
        if (!isStart)
        {
            endingText.gameObject.SetActive(true);
            StartCoroutine(_changeText());
            isStart = true;
        }
        

    }

    IEnumerator _changeText()
    {
        
        yield return new WaitForSeconds(5.5f);
        foreach(string str in headText)
        {
            endingText.text = $"{str} \n Tanis \n Taniyama";
            yield return new WaitForSeconds(3f);
        }
        yield return new WaitForSeconds(2.5f);
        endingText.text = "END";

    }
}
