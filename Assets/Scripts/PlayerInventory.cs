using TMPro;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public TMP_Text inventoryText;

    private int keycardCount;

    void Start()
    {
        if (inventoryText == null)
        {
            GameObject inventoryTextObject = GameObject.Find("InventoryText");

            if (inventoryTextObject != null)
            {
                inventoryText = inventoryTextObject.GetComponent<TMP_Text>();
            }
        }

        UpdateInventoryText();
    }

    public void AddKeycard()
    {
        keycardCount++;
        UpdateInventoryText();
    }

    public bool HasKeycard()
    {
        return keycardCount > 0;
    }

    void UpdateInventoryText()
    {
        if (inventoryText != null)
        {
            inventoryText.gameObject.SetActive(true);
            inventoryText.text = "Keycards: " + keycardCount;
        }
    }
}
