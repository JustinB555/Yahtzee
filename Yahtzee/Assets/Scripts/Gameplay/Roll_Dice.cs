//#define DB

/* Created By: Justin Butler
 * Date Created: 11/30/2022
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Roll_Dice : MonoBehaviour
{
    ///////////////////////////////////
    // Fields
    ///////////////////////////////////

    // Visible

    [SerializeField] Image[] slots;
    public bool[] saved;
    public int[] value;
    [SerializeField] Sprite[] dice;
    [SerializeField] Sprite[] rolls;
    [Space(10)]
    [Header("References")]
    [SerializeField] Image numLeft;
    [SerializeField] GameObject RollButton;
    [SerializeField] Image rollRadial;
    [Space(10)]
    [Header("Modifiers")]
    [SerializeField] float rollDelay = 0.05f;
    [ColorUsage(true, true)]
    [SerializeField] Color normal;
    [ColorUsage(true, true)]
    [SerializeField] Color selected;

    // Hidden

    bool fresh = true;
    int rollsLeft = 3;
    bool rolled = false;

    ///////////////////////////////////
    // Exectutions
    ///////////////////////////////////

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        #region Debugging
#if DB
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            rollsLeft = 3;
            UpdateRollCounter(rollsLeft);
        }
#endif
        #endregion
    }

    ///////////////////////////////////
    // Public Methods
    ///////////////////////////////////

    public void SaveSlot(int s)
    {
        if (fresh) { return; }

        saved[s] ^= true;
        if (saved[s])
            slots[s].color = selected;
        else
            slots[s].color = normal;
    }

    public void RolltheDice()
    {
        if (rollsLeft > 0)
        {
            rolled = true;
            rollsLeft--;
            UpdateRollCounter(rollsLeft);

            StartCoroutine(RollDice(0));
            StartCoroutine(RollDice(1));
            StartCoroutine(RollDice(2));
            StartCoroutine(RollDice(3));
            StartCoroutine(RollDice(4));
        }
    }

    public bool Rolled()
    {
        return rolled;
    }

    public void ResetRolls()
    {
        rollsLeft = 3;
        UpdateRollCounter(rollsLeft);
    }

    public float RollDelay()
    {
        return rollDelay * 7;
    }

    public void ResetSaved()
    {
        for (int i = 0; i < saved.Length; i++)
        {
            if (saved[i])
            {
                saved[i] = false;
                slots[i].color = normal;
            }
        }
    }

    public void ResetDiceSlots()
    {
        foreach (Image i in slots)
            i.sprite = dice[7];
    }

    ///////////////////////////////////
    // Private Methods
    ///////////////////////////////////

    int ChooseRanNum()
    {
        int ranNum = Random.Range(1, 7);
#region Debugging
#if DB
        Debug.Log("<i><color=red>" + ranNum + "</color></i>");
#endif
#endregion
        return ranNum;
    }

    // s = 0,1,2,3,4 | r = 1,2,3,4,5,6 & invalid
    int ChooseDiceSprite(int s, int r)
    {
        switch (r)
        {
            case 1:
                slots[s].sprite = dice[0];
                break;
            case 2:
                slots[s].sprite = dice[1];
                break;
            case 3:
                slots[s].sprite = dice[2];
                break;
            case 4:
                slots[s].sprite = dice[3];
                break;
            case 5:
                slots[s].sprite = dice[4];
                break;
            case 6:
                slots[s].sprite = dice[5];
                break;
            default:
                slots[s].sprite = dice[6];
                throw new System.Exception("Random Number not valid!!!");
        }
        return r;
    }

    IEnumerator RollDice(int s)
    {
        if (fresh)
            fresh = false;

        if (!saved[s])
        {
            int rolls = Random.Range(3, 6);
            for (int i = 0; i <= rolls; i++)
            {
#region Debugging
#if DB
        Debug.Log("<b><color=yellow>" + slots[s] + "</color></b>");
#endif
#endregion
                value[s] = ChooseDiceSprite(s, ChooseRanNum());
                yield return new WaitForSeconds(rollDelay);
            }
        }
    }

    // rolls = 0,1,2,3 | l = 3,2,1,0
    void UpdateRollCounter(int l)
    {
        numLeft.sprite = rolls[l];
        rollRadial.fillAmount = (float)l / (float)3;
        if (l <= 0)
        {
            RollButton.SetActive(false);
        }
        else
            RollButton.SetActive(true);
    }
}
