using System;
using Services;
using UnityEngine;

namespace BT
{
    public class PlayerChooseScreen : MonoBehaviour
    {
        [SerializeField] private ChoosePlayerButton _templateButton;

        private int _playerCount = 1;
        
        public event Action ChooseCompletedEvent;

        private void Awake()
        {
            for(int i = 0; i < ProjectContext.Instance.PlayerBindInputService.DeviceCount; i++)
            {
                var btn = Instantiate(_templateButton, _templateButton.transform.parent);
                btn.Init(i+1, ChooseCompleted);
            }

            _templateButton.gameObject.SetActive(false);
        }

        private void ChooseCompleted()
        {
            ProjectContext.Instance.GameSettings.SetPlayerCount(_playerCount);
            ChooseCompletedEvent?.Invoke();
        }
    }
}
