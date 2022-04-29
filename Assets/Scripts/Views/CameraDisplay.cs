using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventMessages;

namespace LifeView {

    [RequireComponent(typeof(Camera))]
    public class CameraDisplay : MonoBehaviour
    {

        [SerializeField] private Vector3Action moveCamera;

        private Camera cam;

        private void OnEnable()
        {
            moveCamera.listeners += MoveCamera;
        }

        private void OnDisable()
        {
            moveCamera.listeners -= MoveCamera;
        }

        private void Start()
        {
            cam = GetComponent<Camera>();
        }

        private void MoveCamera(Vector3 vector)
        {
            Vector3 position = new Vector3(vector.x, vector.y, transform.position.z);
            cam.orthographicSize = vector.z;
            transform.position = position;
        }
    }
}