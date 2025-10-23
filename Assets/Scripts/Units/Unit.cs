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

    public int getId() {
        return _id;
    }

    public GameObject getPrefab() {
        return _prefab;
    }

    public int getHP() {
        return _hp;
    }

    public int getSpeed() {
        return _speed;
    }

    public void takeDamage() {
        
    }

    public void attack() {

    }

}
