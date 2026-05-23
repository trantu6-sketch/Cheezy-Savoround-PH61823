using System.Collections.Generic;
using UnityEngine;

public class TrayManager : MonoBehaviour
{
    [SerializeField] private GameObject _slotPrefabLight; // Prefab ô khay màu sáng (chẵn)
    [SerializeField] private GameObject _slotPrefabDark;  // Prefab ô khay màu tối (lẻ)
    [SerializeField] private float _slotSpacing = 1.2f; // Khoảng cách giữa các ô khay

    // Lưu trữ các ô khay
    private List<GameObject> _slots = new List<GameObject>();

    public void GenerateTray(int slotCount)
    {
        ClearTray();

        // Tính toán offset để căn giữa các khay
        float offsetX = (slotCount - 1) * _slotSpacing * 0.5f;

        for (int i = 0; i < slotCount; i++)
        {
            Vector3 localPos = new Vector3(i * _slotSpacing - offsetX, 0, 0); // Khay đĩa chỉ xếp trên trục X
            Vector3 worldPos = transform.position + localPos;

            bool isEven = i % 2 == 0;
            GameObject prefabToUse = isEven ? _slotPrefabLight : _slotPrefabDark;

            GameObject slot = Instantiate(prefabToUse, worldPos, Quaternion.identity, transform);
            slot.name = $"TraySlot_{i}";
            
            _slots.Add(slot);
        }
        
        Debug.Log($"[TrayManager] Đã tạo thành công khay chứa gồm {slotCount} ô.");
    }

    private void ClearTray()
    {
        foreach (var slot in _slots)
        {
            if (slot != null)
            {
                Destroy(slot);
            }
        }
        _slots.Clear();
    }

#if UNITY_EDITOR
    public void DrawGizmos(int slotCount)
    {
        Gizmos.color = Color.cyan; // Khay đĩa sẽ dùng viền Xanh Dương
        float offsetX = (slotCount - 1) * _slotSpacing * 0.5f;

        for (int i = 0; i < slotCount; i++)
        {
            Vector3 localPos = new Vector3(i * _slotSpacing - offsetX, 0, 0);
            Vector3 worldPos = transform.position + localPos;
            
            // Vẽ khung vuông phẳng (2D trên mặt phẳng XZ)
            Vector3 size = new Vector3(_slotSpacing, 0f, _slotSpacing) * 0.95f; 
            Gizmos.DrawWireCube(worldPos, size);
        }
    }
#endif
}
