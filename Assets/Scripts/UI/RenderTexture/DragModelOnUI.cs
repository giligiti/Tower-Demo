using UnityEngine;
using UnityEngine.EventSystems;

public class DragModelOnUI : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    [Tooltip("需要旋转的模型")]
    [SerializeField] GameObject obj;
    [SerializeField] RectTransform rawImageRect;
    [Range(0.1f, 2.0f)]
    [SerializeField] float rotateSpeed = 1f;
    private Vector3 lastPosition;
    bool isDrag = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        //如果鼠标点击的时候点击到rawImage元素的话
        if (RectTransformUtility.RectangleContainsScreenPoint(rawImageRect, eventData.position, eventData.pressEventCamera))
        {
            isDrag = true;
            lastPosition = eventData.position;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDrag = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDrag && Input.GetMouseButton(0))
        {
            Vector3 currentPosition = Input.mousePosition;
            Vector3 delta = currentPosition - lastPosition;
            //旋转物体
            obj.transform.Rotate(Vector3.up, -delta.x * rotateSpeed/10);

            lastPosition = currentPosition;

        }
    }
}
