using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClassifiedResultUIController : MonoBehaviour
{
    [SerializeField] private Image rightImage;
    [SerializeField] private Image wrongImage;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float timeShowingResult = 1f;

    private Coroutine showResultCR;
    public void DisplayClassifiedResult(bool isRight)
    {
        rightImage.gameObject.SetActive(false);
        wrongImage.gameObject.SetActive(false);

        var resultImg = isRight ? rightImage : wrongImage;
        if (showResultCR != null)
        {
            StopCoroutine(CR_ShowingClassifiedResult(resultImg));
        }
        showResultCR = StartCoroutine(CR_ShowingClassifiedResult(resultImg));

    }

    public void Show()
    {
        Display(true);
    }

    public void Hide()
    {
        Display(false);
    }

    public void Display(bool isShowing)
    {
        canvasGroup.alpha = isShowing ? 1 : 0;
    }

    private IEnumerator CR_ShowingClassifiedResult(Image resultImage)
    {
        resultImage.gameObject.SetActive(true);

        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;

        float value = 0;
        while (value < timeShowingResult)
        {
            value += Time.deltaTime;
            resultImage.gameObject.transform.localScale = Vector3.Lerp(startScale, endScale, value / timeShowingResult);
            yield return null;
        }
    }
}
