using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFactory : MonoBehaviour
{
    public class ProjectileData
    {
        public Vector3 spawnPosition;
        public Vector2 spawnScale;
        public float spawnAngle;

        public float spawnSpeed;
        public float spawnAcceleration;

        public Vector2 inheritedVelocity;
        public float lifetime;
        public float damage;
        public Damage.Type type;

        public float scaleRate;
        public float rotationRate;
    }

    public int maxProjectiles;
    public Transform baseProjectile;

    private List<Transform> projectiles;

    private void Awake()
    {
        projectiles = new List<Transform>();
        for (int i = 0; i < maxProjectiles; i++)
        {
            Transform t = Instantiate(baseProjectile, transform);
            t.gameObject.SetActive(false);
            
            projectiles.Add(t);
        }
    }

    private Transform GetProjectileFromPool()
    {
        foreach (Transform t in projectiles)
        {
            if (!t.gameObject.activeInHierarchy)
            {
                t.gameObject.SetActive(true);
                return t;
            }
        }

        return null;
    }

    public Transform Spawn(ProjectileData data)
    {
        Transform proj = GetProjectileFromPool();

        proj.GetComponent<ProjectileHandler>().SetupProjectile(data);

        return proj;
    }
}
