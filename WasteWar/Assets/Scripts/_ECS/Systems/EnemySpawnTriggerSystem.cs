using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EnemySpawnTriggerSystem : SystemBase
{
    private Counter spawnTimer;

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        Entities
            .WithoutBurst()
            .WithAll<EnemySpawnerComponent>()
            .ForEach((Counter counter) =>
            {
                spawnTimer = counter;
            }).Run();
    }
    protected override void OnUpdate()
    {
        if (spawnTimer.currentTime <= 0 && spawnTimer.currentWave < spawnTimer.waveCount)
        {
            EntityManager.CreateEntity(typeof(EnemySpawnerSystemEnabler));
            spawnTimer.ResetTimer();
        }
    }
}
