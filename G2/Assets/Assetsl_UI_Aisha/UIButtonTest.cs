using UnityEngine;
using TMPro;

public class UIButtonTest : MonoBehaviour
{
    public TextMeshProUGUI textLabel;

    public void ChangeText()
    {
        textLabel.text = "You clicked the button!";
    }
}
