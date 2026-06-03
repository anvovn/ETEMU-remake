using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class PopupTextStyler : MonoBehaviour
{
    [Header("Appearance")]
    public Color textColor = Color.white;
    public float fontSize = 40f;

    [Header("Player Tracking")]
    public Transform playerTarget;
    public float screenHeightOffset = 200f;

    private TMP_Text _text;
    private RectTransform _rectTransform;
    private Canvas _canvas;

    void Awake()
    {
        _text = GetComponent<TMP_Text>();
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();

        _text.enableAutoSizing = false;
        _text.color = textColor;
        _text.fontSize = fontSize;
    }

    void Start()
    {
        if (playerTarget == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                playerTarget = player.transform;
        }
    }

    void LateUpdate()
    {
        if (playerTarget == null || Camera.main == null || _canvas == null)
            return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(playerTarget.position);

        // Don't position when player is behind the camera
        if (screenPos.z < 0f)
            return;

        screenPos.y += screenHeightOffset;

        if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            _rectTransform.position = new Vector3(screenPos.x, screenPos.y, 0f);
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform,
                screenPos,
                _canvas.worldCamera,
                out Vector2 localPoint
            );
            _rectTransform.localPosition = localPoint;
        }
    }
}
