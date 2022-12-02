//#define DB

/* Created By: Justin Butler
 * Date Created: 12/1/2022
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreCard : MonoBehaviour
{
    ///////////////////////////////////
    // Fields
    ///////////////////////////////////

    // Visible
    [Tooltip("Element 0 = 1, Element 6 = Current Radial, Element 7 = 3x, Element 14 = Chance")]
    [SerializeField] TextMeshProUGUI[] textP1;
    [Tooltip("Element 0 = 1, Element 6 = Current Radial, Element 7 = 3x, Element 14 = Chance")]
    [SerializeField] TextMeshProUGUI[] textP2;
    [Tooltip("Element 0 = 1, Element 6 = Fill, Element 7 = 3x, Element 14 = Chance")]
    [SerializeField] GameObject[] p1Buttons;
    [Tooltip("Element 0 = 1, Element 6 = Fill, Element 7 = 3x, Element 14 = Chance")]
    [SerializeField] GameObject[] p2Buttons;
    [Tooltip("Element 0 = 1, Element 6 = Fill, Element 7 = 3x, Element 14 = Chance")]
    [SerializeField] GameObject[] p1Disable;
    [Tooltip("Element 0 = 1, Element 6 = Fill, Element 7 = 3x, Element 14 = Chance")]
    [SerializeField] GameObject[] p2Disable;
    [Space(10)]
    [Header("References")]
    [SerializeField] Roll_Dice rd;
    [SerializeField] Menu m;
    [SerializeField] TextMeshProUGUI p1Score;
    [SerializeField] TextMeshProUGUI p2Score;
    [SerializeField] Image p1Radial;
    [SerializeField] Image p1Radial_Mask;
    [SerializeField] Image p2Radial;
    [SerializeField] Image p2Radial_Mask;
    [SerializeField] GameObject barriar;
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] TextMeshProUGUI winner;
    [Space(10)]
    [Header("Modifiers")]
    public bool player1 = true;
    [ColorUsage(true, true)]
    [SerializeField] Color failed;
    [ColorUsage(true, true)]
    [SerializeField] Color success;
    [ColorUsage(true, true)]
    [SerializeField] Color normal;
    [TextArea(5, 10)]
    [SerializeField] string pl1Wins;
    [TextArea(5, 10)]
    [SerializeField] string pl2Wins;
    [TextArea(5, 10)]
    [SerializeField] string tie;

    // Hidden
    bool yahtzee1 = false;
    bool yahtzee2 = false;
    bool[] lockedP1 = new bool[14];
    bool[] lockedP2 = new bool[14];
    int[] scoreP1 = new int[14];
    int[] scoreP2 = new int[14];

    ///////////////////////////////////
    // Exectutions
    ///////////////////////////////////

    // Start is called before the first frame update
    void Start()
    {
        SetupUI();
    }

    // Update is called once per frame
    void Update()
    {

    }

    ///////////////////////////////////
    // Pubilc Methods
    ///////////////////////////////////

    public void LockScore(int b)
    {
        if (player1)
        {
            p1Buttons[b].SetActive(false);
            p1Disable[b].SetActive(false);
            lockedP1[b] = true;
            scoreP1[b] = Convert.ToInt32(textP1[b].text);
        }
        else
        {
            p2Buttons[b].SetActive(false);
            p2Disable[b].SetActive(false);
            lockedP2[b] = true;
            scoreP2[b] = Convert.ToInt32(textP2[b].text);
        }
        UpperSection(player1);
        rd.ResetRolls();
        rd.ResetSaved();
        FinalScore();
        if (CheckWin())
            ShowWinner();
        if (!m.solo)
            player1 ^= true;
        ResetScoreBoard();
        rd.ResetDiceSlots();
    }

    public void ShowClac()
    {
        StartCoroutine(ScoreCalc());
    }

    public void FullRestart()
    {
        for (int i = 0; i < lockedP1.Length; i++)
        {
            lockedP1[i] = false;
            lockedP2[i] = false;
            scoreP1[i] = 0;
            scoreP2[i] = 0;
        }

        SetupUI();
        ResetScoreBoard();
        UpdateUpperRadial(player1, 0);
        UpdateUpperRadial(!player1, 0);
        p1Radial_Mask.color = normal;
        p2Radial_Mask.color = normal;
    }

    ///////////////////////////////////
    // Private Methods
    ///////////////////////////////////

    #region Win Condition
    bool CheckWin()
    {
        bool gameOver = false;
        // goal is 26
        int count = 0;
        for (int i = 0; i < lockedP1.Length; i++)
        {
            if (lockedP1[i])
                count++;
            if (lockedP2[i])
                count++;
        }
        if (count == 26 || (count == 13 && m.solo))
            gameOver = true;

        return gameOver;
    }

    void ShowWinner()
    {
        int p1 = Convert.ToInt32(p1Score.text);
        int p2 = Convert.ToInt32(p2Score.text);

        gameOverScreen.SetActive(true);

        if (p1 > p2)
            winner.text = pl1Wins;
        else if (p1 < p2)
            winner.text = pl2Wins;
        else if (p1 == p2)
            winner.text = tie;
    }
    #endregion

    #region Score Logic

    IEnumerator ScoreCalc()
    {
        yield return new WaitForSeconds(rd.RollDelay());
        if (rd.Rolled())
            barriar.SetActive(false);

        One();
        Two();
        Three();
        Four();
        Five();
        Six();
        //UpperSection();
        ThreeX();
        FourX();
        FullHouse();
        Small();
        Large();
        Yahtzee();
        Chance();

        yield return null;
    }

    void FinalScore()
    {
        int p1 = 0;
        int p2 = 0;
        foreach (int v in scoreP1)
        {
            p1 += v;
            p1Score.text = $"{p1}";
        }
        foreach (int v in scoreP2)
        {
            p2 += v;
            p2Score.text = $"{p2}";
        }
    }

    void One()
    {
        int v = 0;
        for (int i = 0; i < 5; i++)
        {
            if (rd.value[i] == 1)
            {
                v += rd.value[i];
                #region Debugging
#if DB
                Debug.Log("<b><color=red>One calc: " + v + "</color></b>");
#endif
                #endregion
            }
        }

        #region Debugging
#if DB
        Debug.Log("<b><color=green>One Score: " + v + "</color></b>");
#endif
        #endregion
        if (player1 && !lockedP1[0])
            textP1[0].text = $"{v}";
        else if (!player1 && !lockedP2[0])
            textP2[0].text = $"{v}";
    }

    void Two()
    {
        int v = 0;
        for (int i = 0; i < 5; i++)
        {
            if (rd.value[i] == 2)
            {
                v += rd.value[i];
                #region Debugging
#if DB
                Debug.Log("<b><color=red>Two calc: " + v + "</color></b>");
#endif
                #endregion
            }
        }

        #region Debugging
#if DB
        Debug.Log("<b><color=green>Two Score: " + v + "</color></b>");
#endif
        #endregion
        if (player1 && !lockedP1[1])
            textP1[1].text = $"{v}";
        else if (!player1 && !lockedP2[1])
            textP2[1].text = $"{v}";
    }

    void Three()
    {
        int v = 0;
        for (int i = 0; i < 5; i++)
        {
            if (rd.value[i] == 3)
            {
                v += rd.value[i];
                #region Debugging
#if DB
                Debug.Log("<b><color=red>Three calc: " + v + "</color></b>");
#endif
                #endregion
            }
        }

        #region Debugging
#if DB
        Debug.Log("<b><color=green>Three Score: " + v + "</color></b>");
#endif
        #endregion
        if (player1 && !lockedP1[2])
            textP1[2].text = $"{v}";
        else if (!player1 && !lockedP2[2])
            textP2[2].text = $"{v}";
    }

    void Four()
    {
        int v = 0;
        for (int i = 0; i < 5; i++)
        {
            if (rd.value[i] == 4)
            {
                v += rd.value[i];
                #region Debugging
#if DB
                Debug.Log("<b><color=red>Four calc: " + v + "</color></b>");
#endif
                #endregion
            }
        }

        #region Debugging
#if DB
        Debug.Log("<b><color=green>Four Score: " + v + "</color></b>");
#endif
        #endregion
        if (player1 && !lockedP1[3])
            textP1[3].text = $"{v}";
        else if (!player1 && !lockedP2[3])
            textP2[3].text = $"{v}";
    }

    void Five()
    {
        int v = 0;
        for (int i = 0; i < 5; i++)
        {
            if (rd.value[i] == 5)
            {
                v += rd.value[i];
                #region Debugging
#if DB
                Debug.Log("<b><color=red>Five calc: " + v + "</color></b>");
#endif
                #endregion
            }
        }

        #region Debugging
#if DB
        Debug.Log("<b><color=green>Five Score: " + v + "</color></b>");
#endif
        #endregion
        if (player1 && !lockedP1[4])
            textP1[4].text = $"{v}";
        else if (!player1 && !lockedP2[4])
            textP2[4].text = $"{v}";
    }

    void Six()
    {
        int v = 0;
        for (int i = 0; i < 5; i++)
        {
            if (rd.value[i] == 6)
            {
                v += rd.value[i];
                #region Debugging
#if DB
                Debug.Log("<b><color=red>Six calc: " + v + "</color></b>");
#endif
                #endregion
            }
        }

        #region Debugging
#if DB
        Debug.Log("<b><color=green>Six Score: " + v + "</color></b>");
#endif
        #endregion
        if (player1 && !lockedP1[5])
            textP1[5].text = $"{v}";
        else if (!player1 && !lockedP2[5])
            textP2[5].text = $"{v}";
    }

    void UpperSection(bool p1)
    {
        int count = 0;
        for (int i = 0; i <= 5; i++)
        {
            if (p1)
                count += scoreP1[i];
            else
                count += scoreP2[i];
        }
        if (count >= 63)
            if (p1)
                scoreP1[6] = 35;
            else
                scoreP2[6] = 35;

        #region Debugging
#if DB
        Debug.Log("<b><color=yellow>Upper Section count: " + count + "</color></b>");
#endif
        #endregion
        if (player1)
        {
            textP1[6].text = $"{count}";
            UpdateUpperRadial(p1, count);
        }
        else
        {
            textP2[6].text = $"{count}";
            UpdateUpperRadial(p1, count);
        }

    }

    void ThreeX()
    {
        int v = 0;
        int[] count = new int[7];
        bool three = false;
        foreach (int n in rd.value)
            count[n]++;
        for (int i = 0; i < count.Length; i++)
        {
            int numberCount = count[i];
            #region Debugging
#if DB
            if (numberCount > 0)
                Debug.Log("<b><color=green>" + i + "</color></b> occurs <i><color=red>" + numberCount + "</color></i> times");
#endif
            #endregion
            if (numberCount >= 3)
                three = true;
        }
        if (three)
            for (int i = 0; i < 5; i++)
            {
                v += rd.value[i];
                #region Debugging
#if DB
                Debug.Log("<b><color=red>Three of a Kind calc: " + v + "</color></b>");
#endif
                #endregion
            }

        #region Debugging
#if DB
        Debug.Log("<b><color=green>Three of a Kind Score: " + v + "</color></b>");
#endif
        #endregion
        if (player1 && !lockedP1[7])
            textP1[7].text = $"{v}";
        else if (!player1 && !lockedP2[7])
            textP2[7].text = $"{v}";
    }

    void FourX()
    {
        int v = 0;
        int[] count = new int[7];
        bool four = false;
        foreach (int n in rd.value)
            count[n]++;
        for (int i = 0; i < count.Length; i++)
        {
            int numberCount = count[i];
            #region Debugging
#if DB
            if (numberCount > 0)
                Debug.Log("<b><color=green>" + i + "</color></b> occurs <i><color=red>" + numberCount + "</color></i> times");
#endif
            #endregion
            if (numberCount >= 4)
                four = true;
        }
        if (four)
            for (int i = 0; i < 5; i++)
            {
                v += rd.value[i];
                #region Debugging
#if DB
                Debug.Log("<b><color=red>Four of a Kind calc: " + v + "</color></b>");
#endif
                #endregion
            }

        #region Debugging
#if DB
        Debug.Log("<b><color=green>Four of a Kind Score: " + v + "</color></b>");
#endif
        #endregion
        if (player1 && !lockedP1[8])
            textP1[8].text = $"{v}";
        else if (!player1 && !lockedP2[8])
            textP2[8].text = $"{v}";
    }

    void FullHouse()
    {
        int v = 0;
        int[] count = new int[7];
        bool three = false;
        bool two = false;
        foreach (int n in rd.value)
            count[n]++;
        for (int i = 0; i < count.Length; i++)
        {
            int numberCount = count[i];
            #region Debugging
#if DB
            if (numberCount > 0)
                Debug.Log("<b><color=green>" + i + "</color></b> occurs <i><color=red>" + numberCount + "</color></i> times");
#endif
            #endregion
            if (numberCount >= 3)
                three = true;
            if (numberCount == 2)
                two = true;
        }
        if (three && two)
            v = 25;

        #region Debugging
#if DB
        Debug.Log("<b><color=green>Full House Score: " + v + "</color></b>");
#endif
        #endregion
        if (player1 && !lockedP1[9])
            textP1[9].text = $"{v}";
        else if (!player1 && !lockedP2[9])
            textP2[9].text = $"{v}";
    }

    void Small()
    {
        int v = 0;
        bool[] large = new bool[6];

        for (int i = 0; i < 5; i++)
        {
            switch (rd.value[i])
            {
                case 1:
                    large[0] = true;
                    break;
                case 2:
                    large[1] = true;
                    break;
                case 3:
                    large[2] = true;
                    break;
                case 4:
                    large[3] = true;
                    break;
                case 5:
                    large[4] = true;
                    break;
                case 6:
                    large[5] = true;
                    break;
            }
        }
        if (large[2] && large[3] && ((large[0] && large[1]) || (large[1] && large[4]) || (large[4] && large[5])))
            v = 30;

        #region Debugging
#if DB
        Debug.Log("<b><color=green>Small Score: " + v + "</color></b>");
#endif
        #endregion
        if (player1 && !lockedP1[10])
            textP1[10].text = $"{v}";
        else if (!player1 && !lockedP2[10])
            textP2[10].text = $"{v}";
    }

    void Large()
    {
        int v = 0;
        bool[] large = new bool[6];

        for (int i = 0; i < 5; i++)
        {
            switch (rd.value[i])
            {
                case 1:
                    large[0] = true;
                    break;
                case 2:
                    large[1] = true;
                    break;
                case 3:
                    large[2] = true;
                    break;
                case 4:
                    large[3] = true;
                    break;
                case 5:
                    large[4] = true;
                    break;
                case 6:
                    large[5] = true;
                    break;
            }
        }
        if (large[1] && large[2] && large[3] && large[4] && (large[0] || large[5]))
            v = 40;

        #region Debugging
#if DB
        Debug.Log("<b><color=green>Large Score: " + v + "</color></b>");
#endif
        #endregion
        if (player1 && !lockedP1[11])
            textP1[11].text = $"{v}";
        else if (!player1 && !lockedP2[11])
            textP2[11].text = $"{v}";
    }

    void Yahtzee()
    {
        int v = 0;
        int[] count = new int[7];
        bool y = false;
        foreach (int n in rd.value)
            count[n]++;
        for (int i = 0; i < count.Length; i++)
        {
            int numberCount = count[i];
            #region Debugging
#if DB
            if (numberCount > 0)
                Debug.Log("<b><color=green>" + i + "</color></b> occurs <i><color=red>" + numberCount + "</color></i> times");
#endif
            #endregion
            if (numberCount == 5)
                y = true;
        }
        if (((player1 && !lockedP1[12] && !yahtzee1) || (!player1 && !lockedP2[12] && !yahtzee2)) && y)
        {
            if (player1)
                yahtzee1 = true;
            else
                yahtzee2 = true;
            v = 50;
        }
        else if (y && ((player1 && yahtzee1) || (!player1 && yahtzee2)))
        {
            int t = 0;
            if (player1)
            {
                // You can accidently skip your joker rule, yet still keep the extra points. BUG
                t = scoreP1[12];
                v += (t + 100);
                scoreP1[12] = v;
                textP1[12].text = $"{v}";
                JokerRule();
            }
            else
            {
                t = scoreP2[12];
                v += (t + 100);
                scoreP2[12] = v;
                textP2[12].text = $"{v}";
                JokerRule();
            }
        }
        else if (y)
        {
            JokerRule();
        }

        #region Debugging
#if DB
        Debug.Log("<b><color=green>Yahtzee Score: " + v + "</color></b>");
#endif
        #endregion
        if (player1 && !lockedP1[12])
            textP1[12].text = $"{v}";
        else if (!player1 && !lockedP2[12])
            textP2[12].text = $"{v}";
    }

    void Chance()
    {
        int v = 0;
        for (int i = 0; i < 5; i++)
        {
            v += rd.value[i];
            #region Debugging
#if DB
            Debug.Log("<b><color=red>Chance calc: " + v + "</color></b>");
#endif
            #endregion
        }
        #region Debugging
#if DB
        Debug.Log("<b><color=green>Chance Score: " + v + "</color></b>\n<b><color=black>-----------------------------------------------------</color></b>");
#endif
        #endregion
        if (player1 && !lockedP1[13])
            textP1[13].text = $"{v}";
        else if (!player1 && !lockedP2[13])
            textP2[13].text = $"{v}";
    }

    void JokerRule()
    {
        int fh = 25;
        int sm = 30;
        int lr = 40;
        // FH
        if (player1 && !lockedP1[9])
            textP1[9].text = $"{fh}";
        else if (!player1 && !lockedP2[9])
            textP2[9].text = $"{fh}";
        // Small
        if (player1 && !lockedP1[10])
            textP1[10].text = $"{sm}";
        else if (!player1 && !lockedP2[10])
            textP2[10].text = $"{sm}";
        // Large
        if (player1 && !lockedP1[11])
            textP1[11].text = $"{lr}";
        else if (!player1 && !lockedP2[11])
            textP2[11].text = $"{lr}";

        #region Debugging
#if DB
        Debug.Log("<b><color=blue>Joker Score: Wow! Lucky you!</color></b>");
#endif
        #endregion
    }
    #endregion

    void SetupUI()
    {
        gameOverScreen.SetActive(false);
        barriar.SetActive(true);

        for (int i = 0; i < textP1.Length; i++)
        {
            textP1[i].text = "";
            textP2[i].text = "";
            if (i != 6)
            {
                p2Buttons[i].SetActive(false);
                p2Disable[i].SetActive(true);
            }
        }
        textP1[6].text = "0";
        textP2[6].text = "0";
        p1Score.text = "";
        p2Score.text = "";
    }

    void ResetScoreBoard()
    {
        barriar.SetActive(true);

        if (player1)
        {
            for (int i = 0; i <= 13; i++)
            {
                if (!lockedP2[i])
                {
                    if (i != 6)
                    {
                        textP2[i].text = "";
                        p2Buttons[i].SetActive(false);
                        p2Disable[i].SetActive(true);
                    }
                }
                if (!lockedP1[i])
                {
                    if (i != 6)
                    {
                        p1Buttons[i].SetActive(true);
                        p1Disable[i].SetActive(false);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i <= 13; i++)
            {
                if (!lockedP1[i])
                {
                    if (i != 6)
                    {
                        textP1[i].text = "";
                        p1Buttons[i].SetActive(false);
                        p1Disable[i].SetActive(true);
                    }
                }
                if (!lockedP2[i])
                {
                    if (i != 6)
                    {
                        p2Buttons[i].SetActive(true);
                        p2Disable[i].SetActive(false);
                    }
                }
            }
        }
    }

    void UpdateUpperRadial(bool p1, int c)
    {
        float f1 = p1Radial.fillAmount;
        float f2 = p2Radial.fillAmount;

        if (p1)
        {
            f1 = (float)c / (float)63;
            if (f1 >= 1)
                p1Radial_Mask.color = success;
            else if (lockedP1[0] && lockedP1[1] && lockedP1[2] && lockedP1[3] && lockedP1[4] && lockedP1[5])
                p1Radial_Mask.color = failed;
            p1Radial.fillAmount = f1;
        }
        else
        {
            f2 = (float)c / (float)63;
            if (f2 >= 1)
                p2Radial_Mask.color = success;
            else if (lockedP2[0] && lockedP2[1] && lockedP2[2] && lockedP2[3] && lockedP2[4] && lockedP2[5])
                p2Radial_Mask.color = failed;
            p2Radial.fillAmount = f2;
        }
    }
}
