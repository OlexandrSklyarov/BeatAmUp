using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services.Scenes.LoadingScreen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Services.Scenes
{
    public class LoadingScreenView : MonoBehaviour
    {
        public static LoadingScreenView Instance { get; private set; }
    
    
        [SerializeField] private Canvas _loadingCanvas;
        [SerializeField] private Image _loadingProgressSlider;
        [SerializeField] private TextMeshProUGUI _loadingInfo;

        private float _currentLoadingProgress;

        private const float BAR_SPEED = 0.95f;
        private const float WAIT_COMPLETED_TIME = 0.5f;
    

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    

        public async Task LoadAsync(Queue<ILoadingOperation> operations)
        {
            _loadingProgressSlider.fillAmount = 0f;
            _loadingCanvas.enabled = true;

            StartCoroutine(UpdateProgressBar());

            foreach (var operation in operations)
            {
                await operation.Load(OnProgressChange);
            }

            await WaitForProgressBarFill();
            
            if (_loadingCanvas == null) return;

            _loadingCanvas.enabled = false;
        }

    
        private async Task WaitForProgressBarFill()
        {
            if (_loadingProgressSlider.fillAmount < _currentLoadingProgress)
            {
                await Task.Delay(1);
            }

            await Task.Delay(TimeSpan.FromSeconds(WAIT_COMPLETED_TIME));
        }


        private void OnProgressChange(float progress) => _currentLoadingProgress = progress;


        private IEnumerator UpdateProgressBar()
        {
            while (_loadingCanvas.enabled)
            {
                if (_loadingProgressSlider.fillAmount < _currentLoadingProgress)
                {
                    _loadingProgressSlider.fillAmount += Time.deltaTime * BAR_SPEED;
                    var percentage = Mathf.RoundToInt(_loadingProgressSlider.fillAmount * 100f);
                    _loadingInfo.text = $"{percentage} %";
                }
                yield return null;
            }
        }
    }
}
