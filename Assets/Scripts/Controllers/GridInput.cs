using UnityEngine;
using UnityEngine.InputSystem;
using EventMessages;

namespace LifeController
{
    public class GridInput : MonoBehaviour
    {

        [SerializeField] private Vector3Action setGridValue = null;

        private bool leftMouseActive;

        private bool rightMouseActive;

        private void Update()
        {
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            if (leftMouseActive)
            {
                worldPosition.z = 0;
                setGridValue?.Raise(worldPosition);
            }
            else if (rightMouseActive)
            {
                worldPosition.z = 1;
                setGridValue?.Raise(worldPosition);
            }
        }

        public void OnLeftMouseButtonDown(InputAction.CallbackContext context)
        {
            leftMouseActive = context.ReadValueAsButton();
        }

        public void OnRightMouseButtonDown(InputAction.CallbackContext context)
        {
            rightMouseActive = context.ReadValueAsButton();
        }
    }
}