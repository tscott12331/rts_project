using System;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int Id { get; protected set; }
    public int HP { get; protected set; }
    public int Speed { get; protected set; }

    public GameObject Prefab { get; protected set; }

    public void TakeDamage() {
        throw new NotImplementedException();
    }

    public void Attack() {
        throw new NotImplementedException();
    }

}
