using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class EnemySpawnerSystem : ComponentSystem
{
    private Random random;
    private bool hasCreated = false;

    protected override void OnCreate()
    {
        base.OnCreate();
        random = new Random(56);

    }
    protected override void OnUpdate()
    {
        if (!hasCreated)
        {
            Entities
                .ForEach((ref EnemySpawnerComponent prefab) =>
            {
                UnityEngine.Debug.Log("spawning");
                SpawnEntities(prefab, prefab.spawnAmount);
                hasCreated = true;
            });
        }
        //Entities.ForEach((ref Attacker entity) =>
        //    MoveToRandomLocation(entity.entity));
    }

    private void SpawnEntities(EnemySpawnerComponent prefab, int spawnAmount)
    {
        NativeArray<Entity> newEnemies = EntityManager.Instantiate(prefab.prefabEnemy, prefab.spawnAmount, Allocator.Temp);
        foreach (var enemy in newEnemies)
        {
            MoveToRandomLocation(enemy);
        }
    }

    private void MoveToRandomLocation(Entity enemy)
    {
        EntityManager.SetComponentData(enemy, new Translation
        {
            Value = new float3(random.NextFloat(0, 100), 2, random.NextFloat(0, 100))
        });
    }
}
