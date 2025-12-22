using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CurrencyManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyTxt;
    private float totalCurrency;
    public bool isCoinBoosted;
    private const string CURRENCY_PLAYERPREFS_KEY ="Currency";
    public string Format(float num)
    {
        if (num >= 1_000_000_000_000) // Trillions
            return (num / 1_000_000_000_000D).ToString("0.0#") + "T";
        if (num >= 1_000_000_000) // Billions
            return (num / 1_000_000_000D).ToString("0.0#") + "BB";
        if (num >= 1_000_000) // Millions
            return (num / 1_000_000D).ToString("0.0#") + "M";
        if (num >= 1_000) // Thousands
            return (num / 1_000D).ToString("0.0#") + "K";

        return num.ToString("0"); // Default for small numbers
    }
    protected void Start()
    {
        Debug.Log(PlayerPrefs.GetFloat(CURRENCY_PLAYERPREFS_KEY));
        LoadCurrency();
       
    }
    void Awake()
    {
        totalCurrency = PlayerPrefs.GetFloat(CURRENCY_PLAYERPREFS_KEY);
    }
    public void LoadCurrency()
    {
        if (PlayerPrefs.HasKey(CURRENCY_PLAYERPREFS_KEY))
        {
            Debug.Log(PlayerPrefs.GetFloat(CURRENCY_PLAYERPREFS_KEY));
            totalCurrency = PlayerPrefs.GetFloat(CURRENCY_PLAYERPREFS_KEY);
            SetCurrencyText();
        }
        else
        {
            Debug.Log("khong co key");
        }
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat(CURRENCY_PLAYERPREFS_KEY, totalCurrency);
    }
    private void OnApplicationPause(bool pause)
    {
        PlayerPrefs.SetFloat(CURRENCY_PLAYERPREFS_KEY, totalCurrency);
    }
    public void AddCash(float cash)
    {
        totalCurrency += cash;
        Debug.Log(totalCurrency);
        PlayerPrefs.SetFloat(CURRENCY_PLAYERPREFS_KEY, totalCurrency);
        Debug.Log(PlayerPrefs.GetFloat(CURRENCY_PLAYERPREFS_KEY));
        SetCurrencyText();
    }
    public float GetTotalCurrency()
    {
        return totalCurrency;
    }
    public void SubtractCash(float cash)
    {
        totalCurrency += cash;
        SetCurrencyText();
    }
    public void SetCurrencyText()
    {
        if (currencyTxt != null)
        {
            currencyTxt.SetText(Format(totalCurrency));
        }
        else
        {
            Debug.Log("No textMeshPro");
        }
    }

}
