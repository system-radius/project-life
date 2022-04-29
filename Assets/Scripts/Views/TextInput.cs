using System;
using UnityEngine;
using TMPro;
using EventMessages;

namespace LifeView
{
    [RequireComponent(typeof(TMP_InputField))]
    public class TextInput : MonoBehaviour
    {
        [SerializeField] private NumericAction setValue = null;

        private TMP_InputField field;

        private void OnEnable()
        {
            setValue.listeners += ReceiveInput;
        }

        private void OnDisable()
        {
            setValue.listeners -= ReceiveInput;
        }

        private void Awake()
        {
            field = GetComponent<TMP_InputField>();
        }

        public void OnEndEdit(string input)
        {
            setValue?.Raise(float.Parse(input));
        }

        public void ReceiveInput(float f)
        {
            field.text = f.ToString();
        }
    }
}