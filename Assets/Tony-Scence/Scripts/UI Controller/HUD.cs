using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    [Header("Panel & Text References")]
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private GameObject whiteDot;

    [Header("World Space Offset")]
    private Vector3 dotOffset = Vector3.zero;
    private Vector3 panelOffset = new Vector3(0, 0.25f, 0);

    private Transform currentTarget;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (currentTarget == null) return;

        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null) return;

        // Position White Dot directly on item center
        if (whiteDot != null && whiteDot.gameObject.activeSelf)
        {
            Vector3 dotScreenPos = mainCamera.WorldToScreenPoint(currentTarget.position + dotOffset);
            whiteDot.gameObject.SetActive(dotScreenPos.z > 0);
            whiteDot.transform.position = dotScreenPos;
        }

        // Position Message Panel floating slightly above item
        if (messagePanel != null && messagePanel.gameObject.activeSelf)
        {
            Vector3 panelScreenPos = mainCamera.WorldToScreenPoint(currentTarget.position + panelOffset);
            messagePanel.gameObject.SetActive(panelScreenPos.z > 0);
            messagePanel.transform.position = panelScreenPos;
        }
    }

    public void ShowWhiteDot(Transform target)
    {
        currentTarget = target;
        if (messagePanel != null) messagePanel.gameObject.SetActive(false);
        if (whiteDot != null) whiteDot.gameObject.SetActive(true);
    }

    public void OpenMessagePanel(string message, Transform target)
    {
        currentTarget = target;
        if (whiteDot != null) whiteDot.gameObject.SetActive(false);
        if (messageText != null) messageText.text = message;
        if (messagePanel != null)
        {
            messagePanel.gameObject.SetActive(true);
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(messagePanel.GetComponent<RectTransform>());
        }
    }

    public void CloseAll()
    {
        currentTarget = null;
        if (whiteDot != null) whiteDot.gameObject.SetActive(false);
        if (messagePanel != null) messagePanel.gameObject.SetActive(false);
    }
}
