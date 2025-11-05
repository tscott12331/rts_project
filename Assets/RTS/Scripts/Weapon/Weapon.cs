using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform Barrel;

    public GameObject ShootParticleSystem;
    
    public void Shoot()
    {
        Instantiate(ShootParticleSystem, Barrel);
    }
}
