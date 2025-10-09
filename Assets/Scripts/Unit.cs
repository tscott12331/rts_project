using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField]
    int _id;
    [SerializeField]
    int _hp;
    [SerializeField]
    int _speed;

    public int getId() {
        return _id;
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
