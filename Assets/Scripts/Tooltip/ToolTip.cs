using AarquieSolutions.DependencyInjection.ComponentField;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tooltip
{
    public class ToolTip : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI headerField, contentField;
        [SerializeField] private LayoutElement layoutElement;
    
        [Header("Settings")]
        [SerializeField] private int characterWrapLimit;
        [SerializeField] private int headerLength;
        [SerializeField] private int contentLength;

        [GetComponentFromParent] private RectTransform positioningTransform;
        [GetComponent] private RectTransform selfRectTransform;
        
        private void Awake()
        {
            this.InitializeDependencies();
        }
    
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    
        public void Show(string content, string header = "")
        {
            SetText(content, header);
            gameObject.SetActive(true);
        }

        private void SetText(string content, string header = "")
        {
            if (string.IsNullOrEmpty(header))
            {
                headerField.gameObject.SetActive(false);
            }
            else
            {
                headerField.text = header;
                headerField.gameObject.SetActive(true);
            }

            contentField.text = content;

            headerLength = headerField.text.Length;
            contentLength = contentField.text.Length;

            layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;
        }

        private void Update()
        {
            Vector2 mousePosition = Input.mousePosition;
            positioningTransform.position = mousePosition;
        }
    }
}