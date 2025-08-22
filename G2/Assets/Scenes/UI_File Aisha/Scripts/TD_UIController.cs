using UnityEngine;
using UnityEngine.UIElements;

public class TD_UIController : MonoBehaviour
{
    private Button towerButton;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

       
        towerButton = root.Q<Button>("TowerButton");

      
        if (towerButton != null)
        {
            towerButton.clicked += OnTowerButtonClicked;
        }
    }

    private void OnTowerButtonClicked()
    {
        Debug.Log("Tower Button Clicked!");
    }
}
