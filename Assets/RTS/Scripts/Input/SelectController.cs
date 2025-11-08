using UnityEngine;

public class SelectController : MonoBehaviour
{
    [SerializeField]
    RectTransform selectRect;
    [SerializeField]
    LayerMask unitLayer;

    [SerializeField]
    float MAX_MOUSE_RAY;

    // start position of select rect
    Vector2 startPosition = Vector2.zero;

    void Update()
    {
        // start making select rect when click
        if(Input.GetMouseButtonDown(0))
        {
            startPosition = (Vector2) Input.mousePosition;
            selectRect.position = startPosition;
            selectRect.transform.gameObject.SetActive(true);
        }
        // continue select rect when mouse down
        if(Input.GetMouseButton(0))
        {
            Vector2 curMousePos = (Vector2) Input.mousePosition;
            selectRect.sizeDelta = new Vector2(Mathf.Abs(curMousePos.x - startPosition.x), Mathf.Abs(curMousePos.y - startPosition.y));
            selectRect.position = startPosition + (curMousePos - startPosition) / 2;
        }
        // finalize select rect and select units when mouse up
        if(Input.GetMouseButtonUp(0))
        {

            bool isShifting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            if(!isShifting) UnitManager.Instance.DeselectAll();
            
            // shoot ray from mouse
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, MAX_MOUSE_RAY, unitLayer, QueryTriggerInteraction.Ignore);
            if(hit) // if we hit a unit
            {
                var unitGO = hitInfo.transform.gameObject;
                unitGO.TryGetComponent<Unit>(out var unit);

                // select unit if owner is player
                if(unit.Owner == ObjectOwner.Player)
                {
                    // determine how to handle selection based on shift pressed
                    if (isShifting && UnitManager.Instance.UnitIsSelected(unit))
                    {
                        UnitManager.Instance.DeselectUnit(unit);
                    }
                    else
                    {
                        UnitManager.Instance.SelectUnit(unit);
                    }
                } else
                {
                    // if we hit an enemy, deselect if not shifting
                    if(!isShifting) UnitManager.Instance.DeselectAll();
                }
                // hitInfo.transform.Find("UnitSelected").gameObject.SetActive(true);
            } else
            {
                // we didn't hit a unit, deselect if not shifting
                if(!isShifting) UnitManager.Instance.DeselectAll();
            }

            // determine dimensions and positions of final select rect
            float halfWidth = selectRect.rect.width / 2;
            float halfHeight = selectRect.rect.height / 2;
            float left = selectRect.transform.position.x - halfWidth;
            float right = selectRect.transform.position.x + halfWidth;
            float top = selectRect.transform.position.y + halfHeight;
            float bottom = selectRect.transform.position.y - halfHeight;

            foreach(Unit unit in UnitManager.Instance.Units)
            {
                // ignore if unit is enemy
                if (unit.Owner != ObjectOwner.Player) continue;

                // determine unit's screen point
                Vector2 screenPoint = Camera.main.WorldToScreenPoint(unit.transform.position);

                // run selection logic if unit is within bounds of select rect
                if(screenPoint.x >= left && screenPoint.x <= right
                    && screenPoint.y >= bottom && screenPoint.y <= top)
                {
                    // determine selection based on if shifting
                    if (isShifting && UnitManager.Instance.UnitIsSelected(unit))
                    {
                        UnitManager.Instance.DeselectUnit(unit);
                    } else
                    {
                        UnitManager.Instance.SelectUnit(unit);
                    }
                }
            }

            // disable select rect when mouse up
            selectRect.transform.gameObject.SetActive(false);
        }
    }
}
