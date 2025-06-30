using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text fpsText;
    private void Awake()
    {
        Transform child = transform.Find("FPSCounter");

        if (child != null) fpsText = child.GetComponent<TMP_Text>();
    }


    private void Update()
    {
        if(fpsText != null) fpsText.text = (Mathf.RoundToInt(1.0f / Time.deltaTime)).ToString();
    }
}
