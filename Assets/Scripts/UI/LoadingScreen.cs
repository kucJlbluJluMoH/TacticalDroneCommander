using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;

namespace UI
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private GameObject _loadingPanel;
        [SerializeField] private TextMeshProUGUI _loadingText;
        [SerializeField] private Slider _progressBar;
        
        private void Awake()
        {
            if (_loadingPanel != null)
            {
                _loadingPanel.SetActive(false);
            }
        }
        
        public void Show(string message = "Loading...")
        {
            if (_loadingPanel != null)
            {
                _loadingPanel.SetActive(true);
            }
            
            if (_loadingText != null)
            {
                _loadingText.text = message;
            }
            
            if (_progressBar != null)
            {
                _progressBar.value = 0f;
            }
        }
        
        public void Hide()
        {
            if (_loadingPanel != null)
            {
                _loadingPanel.SetActive(false);
            }
        }
        
        public void UpdateProgress(float progress, string message = null)
        {
            if (_progressBar != null)
            {
                _progressBar.value = Mathf.Clamp01(progress);
            }
            
            if (_loadingText != null && !string.IsNullOrEmpty(message))
            {
                _loadingText.text = message;
            }
        }
        
        public async UniTask ShowAsync(string message = "Loading...")
        {
            Show(message);
            await UniTask.Yield();
        }
        
        public async UniTask HideAsync()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            Hide();
        }
    }
}

