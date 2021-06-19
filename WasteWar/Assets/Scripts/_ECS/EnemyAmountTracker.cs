using UnityEngine;

public class EnemyAmountTracker : MonoBehaviour
{
    [HideInInspector]
    public int CurrentEnemies { get; set; } = 0;

    public void SetCurrentEnemyAmount(int newAmount) => CurrentEnemies = newAmount;

    public void DecrementCurrentEnemyAmount()
    {
        CurrentEnemies--;
    }
}
