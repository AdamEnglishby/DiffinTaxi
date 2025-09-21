
using System;
using System.Collections;
using Adam.Runtime.Player;
using UnityEngine;
using UnityEngine.UIElements;

namespace Adam.Runtime
{
    
    [RequireComponent(typeof(UIDocument))]
    public class GameManager : MonoBehaviour
    {

        [SerializeField] private CarController carController;
        [SerializeField] private float totalTimerSeconds = 120f;
        [SerializeField] private float scoreMultiplier = 1500f;
        [SerializeField] private float driftMultiplier = 3000f;
        [SerializeField] private float scorePerCone = 10000f;
        [SerializeField] private UIDocument endScreen;
        
        private UIDocument _document;
        private bool _drifting, _started;

        private TextElement _timer, _score, _drift;
        
        private float _totalScore, _totalDrift, _currentTime;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();

            _timer = _document.rootVisualElement.Q<Label>("timer");
            _score = _document.rootVisualElement.Q<Label>("score");
            _drift = _document.rootVisualElement.Q<Label>("drift");
            
            carController.OnDriftStart += OnDriftStart;
            carController.OnDriftEnd += OnDriftEnd;
            carController.OnConeHit += OnConeHit;

            carController.inputHandler.OnInput += () =>
            {
                if (!_started) StartGame();
            };
        }

        private void StartGame()
        {
            _started = true;
            _currentTime = totalTimerSeconds;
        }

        private void EndGame()
        {
            if(!_started) return;
            endScreen.rootVisualElement.Q<VisualElement>("container").style.opacity = 1;
            Time.timeScale = 0;
        }

        private void OnConeHit(Cone cone)
        {
            cone.Consumed = true;
            _totalScore += scorePerCone;
        }

        private void OnDriftStart()
        {
            _drifting = true;
            _totalDrift = 0f;
        }

        private void OnDriftEnd()
        {
            _drifting = false;
            StartCoroutine(EndDrift(_totalDrift));
        }

        private IEnumerator EndDrift(float driftAmount)
        {
            yield return new WaitForSeconds(1f);
            _totalScore += driftAmount;
            if (!_drifting)
            {
                _totalDrift = 0f;
            }
        }

        private void Update()
        {
            if (_started)
            {
                _currentTime -= Time.deltaTime;
            }
            
            _totalScore += Time.deltaTime * scoreMultiplier * carController.RigidbodyReference.linearVelocity.magnitude;
            if (_drifting)
            {
                _totalDrift += Time.deltaTime * driftMultiplier * carController.RigidbodyReference.linearVelocity.magnitude;
            }

            CheckForTimeUp();
            UpdateUI();
        }

        private void CheckForTimeUp()
        {
            if (_currentTime <= 0)
            {
                EndGame();
            }
        }

        private void UpdateUI()
        {
            _score.text = Mathf.FloorToInt(_totalScore).ToString("N0");
            _drift.text = Mathf.FloorToInt(_totalDrift).ToString("N0");
            
            var minutes = (int) (_currentTime / 60) % 60;
            var seconds = (int) _currentTime % 60;
            _timer.text = minutes.ToString("N0") + ":" + seconds.ToString("N0");
        }
        
    }
    
}
