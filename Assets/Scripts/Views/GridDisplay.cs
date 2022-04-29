using UnityEngine;
using EventMessages;

namespace LifeView
{
    public class GridDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject cellPrefab;

        [SerializeField] private Vector3Action cellEvent;

        [SerializeField] private Vector3Action setColorData;

        private Color aliveColor;

        private void OnEnable()
        {
            cellEvent.listeners += ReceiveCellEvent;
            setColorData.listeners += SetColor;
        }

        private void OnDisable()
        {
            cellEvent.listeners -= ReceiveCellEvent;
            setColorData.listeners -= SetColor;
        }

        private void ReceiveCellEvent(Vector3 vector)
        {
            Collider2D collider = Physics2D.OverlapCircle(vector, 0.5f);
            if (collider == null)
            {
                Instantiate(cellPrefab, vector, Quaternion.identity, transform);
            }
            else
            {
                collider.gameObject.GetComponent<SpriteRenderer>().color = vector.z == 0 ? Color.black : aliveColor;
            }
        }

        private void SetColor(Vector3 vector)
        {
            aliveColor = new Color(vector.x, vector.y, vector.z);
        }
    }
}