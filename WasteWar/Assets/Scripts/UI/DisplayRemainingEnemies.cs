using UnityEngine;
using UnityEngine.UI;

public class DisplayRemainingEnemies : MonoBehaviour
{
    public EnemyAmountTracker tracker;
    public Text amountText;

    private int currentEnemies = 0;

    private void Update()
    {
        if (currentEnemies != tracker.CurrentEnemies)
        {
            amountText.text = tracker.CurrentEnemies.ToString();
            currentEnemies = tracker.CurrentEnemies;
        }
    }
}
