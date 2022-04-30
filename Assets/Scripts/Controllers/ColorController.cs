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
        /// An event to be fired whenever there is a change in the color settings for alive cells.
        /// </summary>
        [SerializeField] private Vector3Action changeColorAlive = null;

        /// <summary>
        /// An event to be fired whenever there is a change in the color settings for dead cells.
        /// </summary>
        [SerializeField] private Vector3Action changeColorDead = null;

        [Header("Received Events")]
        /// <summary>
        /// An event to be received for changes related to red for alive cells.
        /// </summary>
        [SerializeField] private NumericAction changeRedAlive = null;

        /// <summary>
        /// An event to be received for changes related to green for alive cells.
        /// </summary>
        [SerializeField] private NumericAction changeGreenAlive = null;

        /// <summary>
        /// An event to be received for changes related to blue for alive cells.
        /// </summary>
        [SerializeField] private NumericAction changeBlueAlive = null;

        /// <summary>
        /// An event to be received for changes related to red for dead cells.
        /// </summary>
        [SerializeField] private NumericAction changeRedDead = null;

        /// <summary>
        /// An event to be received for changes related to green for dead cells.
        /// </summary>
        [SerializeField] private NumericAction changeGreenDead = null;

        /// <summary>
        /// An event to be received for changes related to blue for dead cells.
        /// </summary>
        [SerializeField] private NumericAction changeBlueDead = null;

        [Header("Data")]
        /// <summary>
        /// Data container for the color of the alive cells.
        /// </summary>
        [SerializeField] private Vector3Data colorDataAlive = null;

        /// <summary>
        /// Data container for the color of the dead cells.
        /// </summary>
        [SerializeField] private Vector3Data colorDataDead = null;

        /// <summary>
        /// Built-in function, used to register methods to events.
        /// </summary>
        private void OnEnable()
        {
            changeRedAlive.listeners += OnChangeRedAlive;
            changeGreenAlive.listeners += OnChangeGreenAlive;
            changeBlueAlive.listeners += OnChangeBlueAlive;
            changeRedDead.listeners += OnChangeRedDead;
            changeGreenDead.listeners += OnChangeGreenDead;
            changeBlueDead.listeners += OnChangeBlueDead;
        }

        /// <summary>
        /// Built-in function, used to unregister methods to events.
        /// </summary>
        private void OnDisable()
        {
            changeRedAlive.listeners -= OnChangeRedAlive;
            changeGreenAlive.listeners -= OnChangeGreenAlive;
            changeBlueAlive.listeners -= OnChangeBlueAlive;
            changeRedDead.listeners -= OnChangeRedDead;
            changeGreenDead.listeners -= OnChangeGreenDead;
            changeBlueDead.listeners -= OnChangeBlueDead;
        }

        private void Start()
        {
            // Fire these events for whoever listens for the initial values.
            changeRedAlive?.Raise(colorDataAlive.value.x);
            changeGreenAlive?.Raise(colorDataAlive.value.y);
            changeBlueAlive?.Raise(colorDataAlive.value.z);

            changeRedDead?.Raise(colorDataDead.value.x);
            changeGreenDead?.Raise(colorDataDead.value.y);
            changeBlueDead?.Raise(colorDataDead.value.z);
        }

        /// <summary>
        /// Method to be called on receive of red color change.
        /// </summary>
        /// <param name="f">The new value to be applied.</param>
        private void OnChangeRedAlive(float f)
        {
            f = ValidateColorChange(f);
            colorDataAlive.value.x = f;
            changeColorAlive?.Raise(colorDataAlive.value);
        }

        /// <summary>
        /// Method to be called on receive of green color change.
        /// </summary>
        /// <param name="f">The new value to be applied.</param>
        private void OnChangeGreenAlive(float f)
        {
            f = ValidateColorChange(f);
            colorDataAlive.value.y = f;
            changeColorAlive?.Raise(colorDataAlive.value);
        }

        /// <summary>
        /// Method to be called on receive of blue color change.
        /// </summary>
        /// <param name="f">The new value to be applied.</param>
        private void OnChangeBlueAlive(float f)
        {
            f = ValidateColorChange(f);
            colorDataAlive.value.z = f;
            changeColorAlive?.Raise(colorDataAlive.value);
        }

        /// <summary>
        /// Method to be called on receive of red color change.
        /// </summary>
        /// <param name="f">The new value to be applied.</param>
        private void OnChangeRedDead(float f)
        {
            f = ValidateColorChange(f);
            colorDataDead.value.x = f;
            changeColorDead?.Raise(colorDataDead.value);
        }

        /// <summary>
        /// Method to be called on receive of green color change.
        /// </summary>
        /// <param name="f">The new value to be applied.</param>
        private void OnChangeGreenDead(float f)
        {
            f = ValidateColorChange(f);
            colorDataDead.value.y = f;
            changeColorDead?.Raise(colorDataDead.value);
        }

        /// <summary>
        /// Method to be called on receive of blue color change.
        /// </summary>
        /// <param name="f">The new value to be applied.</param>
        private void OnChangeBlueDead(float f)
        {
            f = ValidateColorChange(f);
            colorDataDead.value.z = f;
            changeColorDead?.Raise(colorDataDead.value);
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