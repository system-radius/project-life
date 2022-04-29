using UnityEngine;
using EventMessages;

namespace LifeView
{
    public class GridDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject cellPrefab;

        [SerializeField] private Vector3Action cellEvent;

        private void OnEnable()
        {
            cellEvent.listeners += ReceiveCellEvent;
        }

        private void OnDisable()
        {
            cellEvent.listeners -= ReceiveCellEvent;
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
                collider.gameObject.GetComponent<SpriteRenderer>().color = vector.z == 0 ? Color.black : Color.white;
            }
        }
    }
}