using UnityEngine;
using UnityEngine.UIElements;

namespace GoogleDeepMind.GemmaSampleGame.UI
{
    [CreateAssetMenu(fileName = "TextFieldData", menuName = "Gemma Sample Game/UI/Text Field Data")]
    public class TextFieldData : ScriptableObject
    {
        private string _value;
        private event System.Action<string> onValueChanged;

        public string Value => _value;

        public void SetValue(string newValue)
        {
            _value = newValue;
            onValueChanged?.Invoke(_value);
        }

        public void AddListener(System.Action<string> listener)
        {
            onValueChanged += listener;
        }

        public void RemoveListener(System.Action<string> listener)
        {
            onValueChanged -= listener;
        }
    }
}