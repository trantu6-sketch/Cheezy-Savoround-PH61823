using UnityEngine;

public enum PizzaType
{
    Pho_mai,
    Xuc_xich,
    Mi_tom,
    Bun_dau_mam_tom
}

public class PizzaPlate : MonoBehaviour
{
    [Tooltip("Dùng tạm để test thuật toán tuần 1")]
    public PizzaType Type = PizzaType.Pho_mai;
    
    private Vector3 _originalPosition;
    private Transform _originalParent;

    public void Initialize(Transform parentSlot)
    {
        _originalParent = parentSlot;
        transform.SetParent(parentSlot);
        transform.localPosition = new Vector3(0, 1f, 0); // Sinh cách khoảng y = 1
        _originalPosition = transform.position;
    }

    public void PickUp()
    {
        // Nâng đĩa lên một chút theo trục Y để tạo hiệu ứng nhấc lên
        transform.position = new Vector3(transform.position.x, _originalPosition.y + 0.5f, transform.position.z);
    }

    public void DragTo(Vector3 worldPosition)
    {
        // Di chuyển đĩa theo chuột, giữ nguyên cao độ lúc đang nhấc
        transform.position = new Vector3(worldPosition.x, transform.position.y, worldPosition.z);
    }

    public void ReturnToOriginalSlot()
    {
        // Trả đĩa về vị trí khay ban đầu
        transform.position = _originalPosition;
        transform.SetParent(_originalParent);
    }

    public void PlaceAt(Vector3 targetPos, Transform newParent)
    {
        // Đặt đĩa vào ô lưới
        transform.position = targetPos;
        transform.SetParent(newParent);
        _originalParent = newParent;
        _originalPosition = targetPos;
    }
}
