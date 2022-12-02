
/* Created By: Justin Butler
 * Date Created: 11/30/2022
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    ///////////////////////////////////
    // Fields
    ///////////////////////////////////

    // Visible
    [Space(10)]
    [Header("References")]
    [SerializeField] Image menuScroll;
    [Space(10)]
    [Header("Modifiers")]
    [SerializeField] Vector3 maxHeight = new Vector3(0, 1300f, 0);
    [SerializeField] Vector3 minHeight = new Vector3(0, -25f, 0);
    [SerializeField] float moveDuration = 1.0f;
    public bool solo = true;

    // Hidden
    bool open = false;

    ///////////////////////////////////
    // Executions
    ///////////////////////////////////

    // Start is called before the first frame update
    void Start()
    {
        InitialStartup();
    }

    // Update is called once per frame
    void Update()
    {

    }

    ///////////////////////////////////
    // Public Methods
    ///////////////////////////////////

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ToggleMenu()
    {
        open ^= true;
        if (open)
            StartCoroutine(MoveMenu(minHeight));
        else
            StartCoroutine(MoveMenu(maxHeight));
    }

    public void SoloGame()
    {
        solo = true;
        ToggleMenu();
    }

    public void VSGame()
    {
        solo = false;
        ToggleMenu();
    }

    ///////////////////////////////////
    // Private Methods
    ///////////////////////////////////

    void InitialStartup()
    {
        open = true;
        menuScroll.rectTransform.localPosition = minHeight;
    }

    IEnumerator MoveMenu(Vector3 tp)
    {
        Vector3 sp = menuScroll.rectTransform.localPosition;
        float timeElapsed = 0.0f;

        while (timeElapsed < moveDuration)
        {
            menuScroll.rectTransform.localPosition = Vector3.Lerp(sp, tp, timeElapsed / moveDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        menuScroll.rectTransform.localPosition = tp;
    }
}
