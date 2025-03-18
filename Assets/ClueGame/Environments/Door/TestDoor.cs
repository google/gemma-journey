using UnityEngine;
using UnityEngine.UI;

namespace GoogleDeepMind.GemmaSampleGame
{
    public class TestDoor : MonoBehaviour
    {
        [SerializeField] private DoorManager _doorManager;
        [SerializeField] private Button _openButton;
        [SerializeField] private Button _closeButton;

        private void Start()
        {
            _openButton.onClick.AddListener(() =>
            {
                _doorManager.Open();
            });
            _closeButton.onClick.AddListener(() =>
            {
                _doorManager.Close();
            });
        }
    }
}

