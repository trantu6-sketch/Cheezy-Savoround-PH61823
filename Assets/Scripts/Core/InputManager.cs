using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    private Camera _mainCamera;
    private PizzaPlate _draggedPlate;
    private Plane _dragPlane;
    private float _dragHeight = 1.0f; // Độ cao của đĩa khi kéo

    public static event Action<PizzaPlate, GridCell> OnPlatePlaced;

    private void Awake()
    {
        _mainCamera = Camera.main;
        // Mặt phẳng dùng để kéo thả (nằm ngang y = dragHeight)
        _dragPlane = new Plane(Vector3.up, new Vector3(0, _dragHeight, 0));
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryPickUpPlate();
        }
        else if (Input.GetMouseButton(0) && _draggedPlate != null)
        {
            DragPlate();
        }
        else if (Input.GetMouseButtonUp(0) && _draggedPlate != null)
        {
            TryDropPlate();
        }
    }

    private void TryPickUpPlate()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.TryGetComponent(out PizzaPlate plate))
            {
                _draggedPlate = plate;
                _draggedPlate.PickUp();
            }
        }
    }

    private void DragPlate()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (_dragPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPos = ray.GetPoint(distance);
            _draggedPlate.DragTo(worldPos);

            // Vẽ gizmo (Debug Ray) màu cam đậm hướng xuống dưới khi đang kéo
            Debug.DrawRay(_draggedPlate.transform.position, Vector3.down * 5f, new Color(1.0f, 0.5f, 0.0f));
        }
    }

    private void TryDropPlate()
    {
        // Bắn tia từ đĩa xuống dưới để tìm lưới
        Ray ray = new Ray(_draggedPlate.transform.position, Vector3.down);
        
        // Vẽ gizmo (Debug Ray) màu cam đậm lưu lại 2 giây để dễ quan sát khi nhả chuột
        Debug.DrawRay(ray.origin, ray.direction * 5f, new Color(1.0f, 0.5f, 0.0f), 2f);

        // Dùng RaycastAll để tia có thể đi xuyên qua đĩa đang cản đường (nếu đĩa cũ có collider to che mất)
        RaycastHit[] hits = Physics.RaycastAll(ray);
        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent(out GridCell cell) && !cell.IsOccupied)
            {
                // Snap vào ô lưới
                cell.PlacePlate(_draggedPlate);
                OnPlatePlaced?.Invoke(_draggedPlate, cell);
                _draggedPlate = null;
                return; // Thành công
            }
        }

        // Không tìm thấy ô hoặc ô đã có đĩa -> trả về chỗ cũ
        _draggedPlate.ReturnToOriginalSlot();
        _draggedPlate = null;
    }
}
