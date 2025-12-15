using UnityEngine;
using System.Collections.Generic;

public class Weapon : MonoBehaviour
{
    // list of barrels the weapon fires from
    public List<Transform> Barrels;

    // particle system to instantiate when firing
    public GameObject ShootParticleSystem;

    public AudioSource ShootSound;
    
    // enable particle system at all the barrels in the weapon
    public void Shoot()
    {
        foreach(var barrel in Barrels)
        {
            Instantiate(ShootParticleSystem, barrel);
            if(ShootSound != null) ShootSound.PlayOneShot(ShootSound.clip);
        }
    }
}
