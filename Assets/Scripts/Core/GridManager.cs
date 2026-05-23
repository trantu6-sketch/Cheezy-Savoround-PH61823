using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject _cellPrefabLight; // Prefab ô màu sáng (chẵn)
    [SerializeField] private GameObject _cellPrefabDark;  // Prefab ô màu tối (lẻ)
    [SerializeField] private float _cellSpacing = 1.0f; // Khoảng cách giữa các ô
    [SerializeField] private TextAsset _testLevelJson; // Kéo file JSON vào đây để test trực tiếp (để trống sẽ tự load màn 1)

    // Dictionary lưu trạng thái các ô
    private Dictionary<Vector2Int, GameObject> _gridCells = new Dictionary<Vector2Int, GameObject>();
    private LevelData _currentLevel;

    private void Start()
    {
        if (_testLevelJson != null)
        {
            LoadFromTextAsset(_testLevelJson);
        }
        else
        {
            LoadLevel(1); // Mặc định load màn 1 bằng Resources.Load
        }
    }

    public void LoadLevel(int levelId)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>($"Levels/level_{levelId}");
        if (jsonFile == null)
        {
            Debug.LogError($"[GridManager] Không tìm thấy file JSON cho Level {levelId} trong Resources/Levels!");
            return;
        }
        
        LoadFromTextAsset(jsonFile);
    }

    public void LoadFromTextAsset(TextAsset jsonFile)
    {
        _currentLevel = JsonUtility.FromJson<LevelData>(jsonFile.text);
        
        ClearGrid();
        GenerateGrid(_currentLevel.gridWidth, _currentLevel.gridHeight);
    }

    private void GenerateGrid(int width, int height)
    {
        // Tính toán offset để căn lưới ra giữa (center)
        float offsetX = (width - 1) * _cellSpacing * 0.5f;
        float offsetZ = (height - 1) * _cellSpacing * 0.5f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                Vector3 worldPos = new Vector3(x * _cellSpacing - offsetX, 0, y * _cellSpacing - offsetZ);
                
                // Thuật toán Checkerboard (Bàn cờ caro)
                bool isEven = (x + y) % 2 == 0;
                GameObject prefabToUse = isEven ? _cellPrefabLight : _cellPrefabDark;
                
                GameObject cell = Instantiate(prefabToUse, worldPos, Quaternion.identity, transform);
                cell.name = $"Cell_{x}_{y}";
                
                _gridCells[gridPos] = cell;
            }
        }
        
        Debug.Log($"[GridManager] Đã tạo thành công lưới {width}x{height} cho màn {_currentLevel.levelId}");
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
}
