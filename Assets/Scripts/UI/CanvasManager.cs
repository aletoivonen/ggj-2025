using UnityEngine;

namespace Zubble.UI
{
    [RequireComponent(typeof(CheckOS))]
    public class CanvasManager : MonoBehaviour
    {
        [SerializeField] private GameObject _mobileInputs;

        private CheckOS _checkOS;

        private void Awake()
        {
            _checkOS = GetComponent<CheckOS>();

            _mobileInputs.SetActive(_checkOS.IsAndroid() || _checkOS.IsIos());
        }
    }
}