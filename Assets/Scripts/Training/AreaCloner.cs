using UnityEngine;

public class AreaCloner : MonoBehaviour
{
    public GameObject baseArea;
    public int numAreas = 1;
    public float separationX = 10f;
    public float separationY = 10f;

    private int _areaCount;
    private Vector2 _basePos;

    void Awake()
    {
        _basePos = baseArea.transform.position;
    }

    void OnEnable()
    {
        Replicate();
    }

    void Replicate()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        _areaCount = 0;
        int cols = Mathf.CeilToInt(Mathf.Sqrt(numAreas));
        for (int i = 0; i < numAreas; i++)
        {
            int row = i / cols;
            int col = i % cols;
            Vector2 offset = new Vector2(col * separationX, row * separationY);

            if (_areaCount == 0)
            {
                baseArea.transform.position = _basePos;
            }
            else
            {
                var clone = Instantiate(
                    baseArea,
                    (Vector2)_basePos + offset,
                    baseArea.transform.rotation,
                    transform
                );
                clone.name = baseArea.name;
            }
            _areaCount++;
        }
    }
}
