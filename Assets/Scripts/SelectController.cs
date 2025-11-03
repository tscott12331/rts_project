using UnityEngine;

public class SelectController : MonoBehaviour
{
    [SerializeField]
    RectTransform selectRect;
    [SerializeField]
    LayerMask unitLayer;

    [SerializeField]
    float MAX_MOUSE_RAY;

    Vector2 startPosition = Vector2.zero;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            startPosition = (Vector2) Input.mousePosition;
            selectRect.position = startPosition;
            selectRect.transform.gameObject.SetActive(true);
        }
        if(Input.GetMouseButton(0))
        {
            Vector2 curMousePos = (Vector2) Input.mousePosition;
            selectRect.sizeDelta = new Vector2(Mathf.Abs(curMousePos.x - startPosition.x), Mathf.Abs(curMousePos.y - startPosition.y));
            selectRect.position = startPosition + (curMousePos - startPosition) / 2;
        }
        if(Input.GetMouseButtonUp(0))
        {

            bool isShifting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            if(!isShifting) UnitManager.Instance.DeselectAll();
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, MAX_MOUSE_RAY, unitLayer);
            if(hit)
            {
                var unitGO = hitInfo.transform.gameObject;
                unitGO.TryGetComponent<Unit>(out var unit);
                if (isShifting && UnitManager.Instance.UnitIsSelected(unit))
                {
                    UnitManager.Instance.DeselectUnit(unit);
                } else
                {
                    UnitManager.Instance.SelectUnit(unit);
                }
                // hitInfo.transform.Find("UnitSelected").gameObject.SetActive(true);
            } else
            {
                if(!isShifting) UnitManager.Instance.DeselectAll();
            }

            float halfWidth = selectRect.rect.width / 2;
            float halfHeight = selectRect.rect.height / 2;
            float left = selectRect.transform.position.x - halfWidth;
            float right = selectRect.transform.position.x + halfWidth;
            float top = selectRect.transform.position.y + halfHeight;
            float bottom = selectRect.transform.position.y - halfHeight;
            foreach(Unit unit in UnitManager.Instance.Units)
            {
                Vector2 screenPoint = Camera.main.WorldToScreenPoint(unit.transform.position);

                if(screenPoint.x >= left && screenPoint.x <= right
                    && screenPoint.y >= bottom && screenPoint.y <= top)
                {
                    if (isShifting && UnitManager.Instance.UnitIsSelected(unit))
                    {
                        UnitManager.Instance.DeselectUnit(unit);
                    } else
                    {
                        UnitManager.Instance.SelectUnit(unit);
                    }
                }
            }

            selectRect.transform.gameObject.SetActive(false);
        }
    }
}
