using UnityEngine;
using System.Collections.Generic;

public class Weapon : MonoBehaviour
{
    public List<Transform> Barrels;

    public GameObject ShootParticleSystem;
    
    public void Shoot()
    {
        foreach(var barrel in Barrels)
        {
            Instantiate(ShootParticleSystem, barrel);
        }
    }
}
