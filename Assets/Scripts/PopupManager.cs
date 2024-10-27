using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class PopupManager : MonoBehaviour
{
    public GameObject popup;
    private string[] popupTexts = {
        "  ʕ•́ᴥ•̀ʔ\n welcome to the beginner tutorial!",
        "Before move, use the arrow keys\n [ ↑ ← ↓ → ] or [ W A S D ]\n to control the direction.",
        "Now, let's try another movement approach:\n\nPress [Space] to jump. Hold to charge a longer jump.", // Index 2
        "Level Up! Great job!",
        "Well done!  ( ͡• ͜ʖ ͡•)." ,
        "The green cubes are always in motion.",
        "The purple cube contains magic that allows you to change your height!\n\n Try using the ↑ or ↓ keys",
        "This is a very dangerous cube that will explode after 2 seconds.",
        "You must collect enough starts to win the game"
    };
    public void ShowPopup(int index)
    {
        if (index >= 0 && index < popupTexts.Length)
        {
            ChangePopupText(popupTexts[index]);
            popup.SetActive(true);
            StartCoroutine(HidePopupAfterDelay(popup, 5f)); // 5 秒后隐藏
        }
        else
        {
            Debug.LogError("Invalid popup index: " + index);
        }
    }

    public void ChangePopupText(string newText)
    {
        if (popup != null)
        {

            TextMeshProUGUI popupText = popup.GetComponentInChildren<TextMeshProUGUI>();


            if (popupText != null)
            {
                popupText.text = newText;
            }
            else
            {
                Debug.LogError("Text component not found in the popup.");
            }
        }
        else
        {
            Debug.LogError("Popup GameObject is not assigned in the inspector.");
        }
    }
    private IEnumerator HidePopupAfterDelay(GameObject popup, float delay)
    {
        yield return new WaitForSeconds(delay);
        popup.SetActive(false);
    }

}
