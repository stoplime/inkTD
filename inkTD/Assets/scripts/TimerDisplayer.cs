using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerDisplayer : MonoBehaviour
{

    [Tooltip("The object that contains the text component that will become the timer.")]
    public GameObject timerTextObject;

    [Tooltip("The text object for the balance.")]
    public GameObject balanceTextObject;

    [Tooltip("The object that holds the text for the income amount.")]
    public GameObject incomeTextObject;

    [Tooltip("The value the timer will count down from.")]
    public int value = 25;

    [Tooltip("The string appended before the timer's value.")]
    public string appendedTimerInfo = "Next: ";

    [Tooltip("The string appended before the balance's value.")]
    public string appendedBalanceInfo = "";

    [Tooltip("The string appended before the income's value.")]
    public string appendedIncomeInfo = "Income: ";

    private int currentValue;

    private Text incomeText;
    private Text balanceText;
    private Text text; //Text for the income timer.
    private TaylorTimer timer;

	// Use this for initialization
	void Start ()
    {
        if (timerTextObject != null)
            text = timerTextObject.GetComponent<Text>();

        currentValue = value;

        if (incomeTextObject != null)
            incomeText = incomeTextObject.GetComponent<Text>();

        if (balanceTextObject != null)
            balanceText = balanceTextObject.GetComponent<Text>();

        if (balanceText != null)
        {
            balanceText.text = appendedBalanceInfo + PlayerManager.GetBalance(PlayerManager.CurrentPlayer).ToString();
            PlayerManager.OnCurrentPlayerBalanceChange += PlayerManager_OnCurrentPlayerBalanceChange;
        }

        if (incomeText != null)
        {
            incomeText.text = appendedIncomeInfo + PlayerManager.GetIncome(PlayerManager.CurrentPlayer).ToString();
            PlayerManager.OnCurrentPlayerIncomeChange += PlayerManager_OnCurrentPlayerIncomeChange;
        }

        timer = new TaylorTimer(1000);
        timer.Elapsed += Timer_Elapsed;
        FixText();
	}

    private void PlayerManager_OnCurrentPlayerIncomeChange(object sender, EventArgs e)
    {
        incomeText.text = appendedIncomeInfo + PlayerManager.GetIncome(PlayerManager.CurrentPlayer).ToString();
    }

    private void PlayerManager_OnCurrentPlayerBalanceChange(object sender, EventArgs e)
    {
        balanceText.text = appendedBalanceInfo + PlayerManager.GetBalance(PlayerManager.CurrentPlayer).ToString();
    }

    private void FixText()
    {
        text.text = appendedTimerInfo + currentValue.ToString();
    }

    private void Timer_Elapsed(object sender, EventArgs e)
    {
        currentValue -= 1;
        if (currentValue <= 0)
        {
            currentValue = value;
            FixText();
            //PlayerManager.ApplyIncome(PlayerManager.CurrentPlayer);
            PlayerManager.ApplyIncomeToAll();
        }
        else
        {
            FixText();
        }
    }

    // Update is called once per frame
    void Update ()
    {
        timer.Update();
	}
}
