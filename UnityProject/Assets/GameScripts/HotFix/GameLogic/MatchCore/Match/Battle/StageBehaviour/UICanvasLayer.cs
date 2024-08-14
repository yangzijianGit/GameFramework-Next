using UnityEngine;

public class UICanvasLayer : MonoBehaviour
{

    [SerializeField]
    int m_nSortOrder;

    private void Start()
    {
        var tParentCanvas = transform.parent.GetComponentInParent<Canvas>();
        string sortingLayerName = tParentCanvas.sortingLayerName;
        var canvas = gameObject.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
        }
        canvas.overrideSorting = true;
        canvas.sortingLayerName = sortingLayerName;
    }

}