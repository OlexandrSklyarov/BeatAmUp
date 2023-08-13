using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BT
{    
    public class ChoosePlayerButton : MonoBehaviour
    {    
        [SerializeField] private Button _selectButton;
        [SerializeField] private TextMeshProUGUI _text;

        public void Init(int count, Action callBack)
        {
            _text.text = $"{count} Player";
            _selectButton.onClick.AddListener(() => callBack?.Invoke());
        }        
    }
}
