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
            PizzaPlate plate = hit.collider.GetComponent<PizzaPlate>();
            if (plate != null)
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
        }
    }

    private void TryDropPlate()
    {
        // Bắn tia từ đĩa xuống dưới để tìm lưới
        Ray ray = new Ray(_draggedPlate.transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GridCell cell = hit.collider.GetComponent<GridCell>();
            if (cell != null && !cell.IsOccupied)
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
