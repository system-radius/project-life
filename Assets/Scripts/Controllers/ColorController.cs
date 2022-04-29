using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventMessages;

namespace LifeController
{
    /// <summary>
    /// A controller that is designed to handle and send events
    /// related to the color settings.
    /// </summary>
    public class ColorController : MonoBehaviour
    {
        [Header("Fired Events")]
        /// <summary>
        /// An event to be fired whenever there is a change in the color settings.
        /// </summary>
        [SerializeField] private Vector3Action changeColor = null;

        [Header("Received Events")]
        /// <summary>
        /// An event to be received for changes related to red.
        /// </summary>
        [SerializeField] private NumericAction changeRed = null;

        /// <summary>
        /// An event to be received for changes related to green.
        /// </summary>
        [SerializeField] private NumericAction changeGreen = null;

        /// <summary>
        /// An event to be received for changes related to blue.
        /// </summary>
        [SerializeField] private NumericAction changeBlue = null;

        [Header("Data")]
        [SerializeField] private Vector3Data colorData = null;

        private void OnEnable()
        {
            changeRed.listeners += OnChangeRed;
            changeGreen.listeners += OnChangeGreen;
            changeBlue.listeners += OnChangeBlue;
        }

        private void OnDisable()
        {
            changeRed.listeners -= OnChangeRed;
            changeGreen.listeners -= OnChangeGreen;
            changeBlue.listeners -= OnChangeBlue;
        }

        private void Start()
        {
            // Fire these events for whoever listens for the initial values.
            changeRed?.Raise(colorData.value.x);
            changeGreen?.Raise(colorData.value.y);
            changeBlue?.Raise(colorData.value.z);
        }

        /// <summary>
        /// Method to be called on receive of red color change.
        /// </summary>
        /// <param name="f">The new value to be applied.</param>
        private void OnChangeRed(float f)
        {
            f = ValidateColorChange(f);
            colorData.value.x = f;
            changeColor?.Raise(colorData.value);
        }

        /// <summary>
        /// Method to be called on receive of green color change.
        /// </summary>
        /// <param name="f">The new value to be applied.</param>
        private void OnChangeGreen(float f)
        {
            f = ValidateColorChange(f);
            colorData.value.y = f;
            changeColor?.Raise(colorData.value);
        }

        /// <summary>
        /// Method to be called on receive of blue color change.
        /// </summary>
        /// <param name="f">The new value to be applied.</param>
        private void OnChangeBlue(float f)
        {
            f = ValidateColorChange(f);
            colorData.value.z = f;
            changeColor?.Raise(colorData.value);
        }

        /// <summary>
        /// A validation method to ensure that the color changes are within range.
        /// </summary>
        /// <param name="f">The value to be validated.</param>
        /// <returns>The value post-validation.</returns>
        private float ValidateColorChange(float f)
        {
            return Mathf.Clamp(f, 0, 1);
        }
    }
}