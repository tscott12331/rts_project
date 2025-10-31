using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField]
    int _id;
    [SerializeField]
    int _hp;
    [SerializeField]
    int _speed;

    [SerializeField]
    GameObject _prefab;

    public int GetId() {
        return _id;
    }

    public GameObject GetPrefab() {
        return _prefab;
    }

    public int GetHP() {
        return _hp;
    }

    public int GetSpeed() {
        return _speed;
    }

    public void TakeDamage() {
        
    }

    public void Attack() {

    }

}
