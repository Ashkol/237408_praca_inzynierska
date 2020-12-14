using UnityEngine;
using UnityEngine.UI;

public class TimerBehaviour : MonoBehaviour
{
    float elapsedTime = 0;
    public Text text;


    void Update()
    {
        elapsedTime += Time.deltaTime;
        text.text = elapsedTime.ToString("n1") + "s";
    }
}
