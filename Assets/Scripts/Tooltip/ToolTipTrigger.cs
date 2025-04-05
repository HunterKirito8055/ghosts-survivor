using UnityEngine;

namespace Tooltip
{
    public class ToolTipTrigger : MonoBehaviour
    {
        [SerializeField] protected string header;
        [SerializeField] protected string content;
    
        private void OnMouseEnter()
        {
            ToolTipManager.Instance.Show(content, header);
        }

        private void OnMouseExit()
        {
            ToolTipManager.Instance.Hide();
        }
    }
}
