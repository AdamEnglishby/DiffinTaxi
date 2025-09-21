using UnityEngine;

namespace Adam.Runtime
{
    
    [RequireComponent(typeof(AudioSource))]
    public class AudioLooper : MonoBehaviour
    {
        
        [SerializeField] private double loopStartTime;
        [SerializeField] private double loopEndTime;

        private int _loopStartSamples;
        private int _loopEndSamples;
        private int _loopLengthSamples;

        private AudioSource _audioSource;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _loopStartSamples = (int)(loopStartTime * _audioSource.clip.frequency);
            _loopEndSamples = (int)(loopEndTime * _audioSource.clip.frequency);
            _loopLengthSamples = _loopEndSamples - _loopStartSamples;
        }

        private void Update()
        {
            if (_audioSource.timeSamples >= _loopEndSamples) { _audioSource.timeSamples -= _loopLengthSamples; }
        }
        
    }
    
}