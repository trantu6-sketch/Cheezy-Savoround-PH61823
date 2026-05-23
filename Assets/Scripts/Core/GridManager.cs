using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject _cellPrefabLight; // Prefab ô màu sáng (chẵn)
    [SerializeField] private GameObject _cellPrefabDark;  // Prefab ô màu tối (lẻ)
    [SerializeField] private float _cellSpacing = 1.0f; // Khoảng cách giữa các ô

    // Dictionary lưu trạng thái các ô
    private Dictionary<Vector2Int, GameObject> _gridCells = new Dictionary<Vector2Int, GameObject>();

    public void GenerateGrid(int levelId, int width, int height)
    {
        ClearGrid();

        // Tính toán offset để căn lưới ra giữa (center)
        float offsetX = (width - 1) * _cellSpacing * 0.5f;
        float offsetZ = (height - 1) * _cellSpacing * 0.5f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                Vector3 localPos = new Vector3(x * _cellSpacing - offsetX, 0, y * _cellSpacing - offsetZ);
                Vector3 worldPos = transform.position + localPos; // Sinh lưới theo vị trí thực của GridManager
                
                // Thuật toán Checkerboard (Bàn cờ caro)
                bool isEven = (x + y) % 2 == 0;
                GameObject prefabToUse = isEven ? _cellPrefabLight : _cellPrefabDark;
                
                GameObject cellObj = Instantiate(prefabToUse, worldPos, Quaternion.identity, transform);
                cellObj.name = $"Cell_{x}_{y}";
                
                GridCell cellComp = cellObj.GetComponent<GridCell>();
                if (cellComp == null) 
                {
                    cellComp = cellObj.AddComponent<GridCell>();
                }
                cellComp.Initialize(gridPos);
                
                _gridCells[gridPos] = cellObj;
            }
        }
        
        Debug.Log($"[GridManager] Đã tạo thành công lưới {width}x{height} cho màn {levelId}");
    }

    private void ClearGrid()
    {
        foreach (var cell in _gridCells.Values)
        {
            if (cell != null)
            {
                Destroy(cell);
            }
        }
        _gridCells.Clear();
    }

#if UNITY_EDITOR
    public void DrawGizmos(int width, int height)
    {
        Gizmos.color = Color.green;
        float offsetX = (width - 1) * _cellSpacing * 0.5f;
        float offsetZ = (height - 1) * _cellSpacing * 0.5f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 localPos = new Vector3(x * _cellSpacing - offsetX, 0, y * _cellSpacing - offsetZ);
                Vector3 worldPos = transform.position + localPos; // Vẽ theo vị trí của GridManager
                
                // Vẽ khung vuông phẳng (2D trên mặt phẳng XZ) kích thước bằng 95% ô lưới
                Vector3 size = new Vector3(_cellSpacing, 0f, _cellSpacing) * 0.95f; 
                Gizmos.DrawWireCube(worldPos, size);
            }
        }
    }
#endif
}
