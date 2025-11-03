using UnityEngine;

/// <summary>
/// Unity Layer의 
/// </summary>
public static class Layers
{
    // Layer Int Value
    public static readonly int Projectile; // 투사체
    public static readonly int EnemyProjectile; // 적 투사체
    
    // Layer Masks
    public static readonly int ProjectileMask;
    public static readonly int EnemyProjectileMask;
    
    static Layers()
    {
        Projectile = LayerMask.NameToLayer("Projectile");
        EnemyProjectile = LayerMask.NameToLayer("Enemy Projectile");

        ProjectileMask = 1 << Projectile;
        EnemyProjectileMask = 1 << EnemyProjectile;
    }
}
