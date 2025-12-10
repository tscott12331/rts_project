using UnityEngine;

public class Player
{
    public delegate void PlayerLostHandler(ObjectOwner owner);
    public static event PlayerLostHandler PlayerLost;

    public ObjectOwner Owner { get; private set; }
    public Transform StartPoint;

    public Structure MainHQ { get; private set; }


    public Player(ObjectOwner owner, Transform startPoint) {
        this.Owner = owner;
        this.StartPoint = startPoint;

        AttackUnit.StructureDestroyed += AttackUnit_StructureDestroyed;

    }

    ~Player() {
        AttackUnit.StructureDestroyed -= AttackUnit_StructureDestroyed;
    }

    public void PlaceInitialStructures() {
        // place player HQ
        MainHQ = StructureManager.Instance.PlaceStructure(0, StartPoint.position, StartPoint.rotation, Owner);
    }



    private void AttackUnit_StructureDestroyed(Structure structure) {
        if(structure == MainHQ) PlayerLost?.Invoke(Owner);
    }

    
}
