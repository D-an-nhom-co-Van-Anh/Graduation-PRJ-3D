using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class ShopManager : MonoBehaviour
{
    [SerializeField] private List <Button> items;
    private bool isShopOpened;
    private ShopItem currentSelectedItem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isShopOpened = false;
        for(int i = 0; i < items.Count; i++)
        {
            int index = i;
            items[index].onClick.AddListener(() =>
            {
                Debug.Log(index);
                SelectItem(items[index].GetComponent<ShopItem>());
            });
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (isShopOpened)
            {
                CloseShop();
            } 
            
        }
    }
    public void OpenShop()
    {
        isShopOpened = true;
    }
    public void SelectItem(ShopItem item)
    {
        if (currentSelectedItem != null)
        {
            if (currentSelectedItem == item)
            {
                currentSelectedItem.HideSelectedUI();
                currentSelectedItem = null;
            }
            else
            {
                currentSelectedItem.HideSelectedUI();
                currentSelectedItem = null;
                currentSelectedItem = item;
                currentSelectedItem.ShowSelectedUI();
            }
        }
        else
        {
            currentSelectedItem = item;
            currentSelectedItem.ShowSelectedUI();
        }
    }
    public void CloseShop()
    {
        GameManager_.Instance.DisableUIShop();
    }
    public void BuyItem()
    {
        if (currentSelectedItem != null)
        {
            if (GameManager_.Instance.GetCurrencyManager().GetTotalCurrency() > currentSelectedItem.Cost)
            {
                GameManager_.Instance.GetCurrencyManager().SubtractCash(currentSelectedItem.Cost);
                switch (currentSelectedItem.Type)
                {
                    case ShopItemType.STAMINA:
                        GameManager_.Instance.GetPlayer().AddStamina(currentSelectedItem.Value);
                        break;
                    case ShopItemType.STAMINA_PERSECOND:
                        GameManager_.Instance.GetPlayer().AddStaminaPerSecond(currentSelectedItem.Value);
                        break;
                    case ShopItemType.SPEED:
                        GameManager_.Instance.GetPlayer().AddSpeed(currentSelectedItem.Value);
                        break;
                }
            }
            else
            {
                Debug.Log("No Money");
                // them Ui hien thi khong du tien
            }
        }
        else
        {
            ShowCantBuyText();
        }
    }
    public void ShowCantBuyText()
    {
        // them Ui hoac am thanh de cho biet chua chon item de mua
        Debug.Log("Please Select 1 Item to Buy");
    }
}
