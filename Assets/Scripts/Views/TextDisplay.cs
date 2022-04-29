using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EventMessages;

namespace LifeView
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextDisplay : MonoBehaviour
    {
        [SerializeField] private NumericAction displayChange;

        private TextMeshProUGUI gui;

        private void OnEnable()
        {
            displayChange.listeners += ChangeDisplayedText;
        }

        private void OnDisable()
        {
            displayChange.listeners -= ChangeDisplayedText;
        }

        private void Start()
        {
            gui = GetComponent<TextMeshProUGUI>();
        }

        private void ChangeDisplayedText(float f)
        {
            gui.text = f.ToString();
        }
    }
}
