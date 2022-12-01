using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject goldHPCanvas;
    [SerializeField] private Slider goldSlider;
    [SerializeField] private Text pointsText;
    [SerializeField] static public UIManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        PointScoringArea.OnPointScored += PointsUpdate;
    }

    public GameObject GetGoldCanvas()
    {
        return goldHPCanvas;
    }

    public Slider GetGoldSlider()
    {
        return goldSlider;
    }

    #region Points

    public Text GetPointsDisplay()
    {
        return pointsText;
    }

    private void PointsUpdate(int pointCount)
    {
        pointsText.text = "Points: " + pointCount;
        StartCoroutine(TextColourHighlight(pointsText, Color.magenta, Color.white));
        StartCoroutine(PointsAnimation());
    }

    private IEnumerator PointsAnimation()
    {
        //const int pointsOriginalSize = 27;
        //const int targetSize = 32;
        //const float lerpSpeed = 0.5f;

        //while (true)
        //{
        //    int currentSize = Mathf.Clamp(Mathf.CeilToInt(Mathf.Lerp(pointsOriginalSize, targetSize, Time.deltaTime * lerpSpeed)), pointsOriginalSize, targetSize);
        //    pointsText.fontSize = currentSize;
        //    Debug.Log(currentSize);
        //    yield return null;

        //    if(currentSize == targetSize)
        //    {
        //        break;
        //    }
        //}
        
        yield return null;
    }

    private IEnumerator TextColourHighlight(Text text, Color colourHighlight, Color originalColour)
    {
        const float ColourShowTime = 0.2f;

        ChangeFontColour(text, colourHighlight);
        yield return new WaitForSeconds(ColourShowTime);
        ChangeFontColour(text, originalColour);
    }

    private void ChangeFontColour(Text text, Color colour)
    {
        text.color = colour;
    }

    #endregion

}
