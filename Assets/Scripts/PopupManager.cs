using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class PopupManager : MonoBehaviour
{
    public GameObject popup;
    private string[] popupTexts = {
        "  ʕ•́ᴥ•̀ʔ\n welcome to the beginner tutorial!",// 0
        "Use the arrow keys\n [ ↑ ← ↓ → ] or [ W A S D ]\n to control the direction.\nPress [Enter] to roll.", //1
        "Now, let's try another movement approach:\nPress and hold [Space] to charge a jump. Decide the direction before jumping as well.", //  2
        "Remeber, use \n [ ↑ ← ↓ → ] or [ W A S D ]\n to change the direction", //3
        "The purple cube contains magic that allows you to change your height!. \nSometimes rolling is much safer than jump!",//4
        "The elongated effect lasts 20s.\n Use mouse scroll to change the length." ,//5
        "The green cubes are always in motion.",//6
        "Use this spring cube to bounce safely between platforms!",//7
        "The forward cube is a very dangerous one that will explode after 2 seconds.",//8
        "You must collect enough starts to win the game",//9
        "Some cubes could trigger a enemy.\n Don''t panic, avoid them or jump on their head to defeat them!",//10
        "Red star could make you invicible and avoid damage, simply rolling on enemy could defeat them.",//11
        "You can collect 5 coins to make you invicible.",//12
        "Some enemies are floating, defeat them can sometimes gain rewards."//13
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
