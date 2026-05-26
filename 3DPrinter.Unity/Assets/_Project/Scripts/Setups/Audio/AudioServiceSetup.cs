using System;
using _Project.Scripts.Audio;
using UnityEngine;

namespace _Project.Scripts.Setups.Audio
{
    [CreateAssetMenu(fileName = "New audio service setup", menuName = "Setup/Audio")]
    public class AudioServiceSetup : ScriptableObject
    {
        [Header("Sound settings")]
        [SerializeField, Range(0f, 1f), Space] private float _volumeSfx = 0.5f;
        [SerializeField, Range(0f, 1f)] private float _volumeMusic = 0.5f;
        [SerializeField, Range(0f, 1f)] private float _volumeLoop = 0.7f;

        [SerializeField, Space] private Vector2 _sfxPitchRange = new Vector2(0.95f, 1.05f);
        [SerializeField] private Vector2 _loopPitchRange = new Vector2(0.95f, 1.05f);

        [Tooltip("X = FadeIn duration, Y = FadeOut duration, Z = StayIn position")]
        [SerializeField] private Vector3 _fadeInOutRange = new Vector3(0.5f, 0.5f, 0.2f);

        [Tooltip("Fade duration for loop sounds when starting/stopping motion")]
        [SerializeField] private float _loopFadeDuration = 0.15f;

        [Header("Actual sound clips")]
        [SerializeField, Space] private EffectProperty[] _sfxProperties;
        [SerializeField] private LoopProperty[] _loopProperties;
        [SerializeField] private MusicProperty[] _musicProperties;

        public float VolumeSfx => _volumeSfx;
        public float VolumeMusic => _volumeMusic;
        public float VolumeLoop => _volumeLoop;

        public float MusicFadeInDuration => _fadeInOutRange.x;
        public float MusicFadeOutDuration => _fadeInOutRange.y;
        public float FadeStayIn => _fadeInOutRange.z;
        public float LoopFadeDuration => _loopFadeDuration;

        public Vector2 PitchRange => _sfxPitchRange;
        public Vector2 LoopPitchRange => _loopPitchRange;

        public EffectProperty[] Sfx => _sfxProperties;
        public LoopProperty[] Loops => _loopProperties;
        public MusicProperty[] Music => _musicProperties;
    }

    [Serializable]
    public class EffectProperty
    {
        [SerializeField] private SoundType _audioType;
        [SerializeField] private AudioClip[] _audioClips;

        public SoundType Type => _audioType;
        public AudioClip[] Clips => _audioClips;
    }

    [Serializable]
    public class LoopProperty
    {
        [SerializeField] private LoopSoundType _audioType;
        [SerializeField] private AudioClip _audioClip;
        [SerializeField, Range(0f, 1f)] private float _volumeScale = 1f;

        public LoopSoundType Type => _audioType;
        public AudioClip Clip => _audioClip;
        public float VolumeScale => _volumeScale;
    }

    [Serializable]
    public class MusicProperty
    {
        [SerializeField] private MusicType _audioType;
        [SerializeField] private AudioClip[] _audioClips;

        public MusicType Type => _audioType;
        public AudioClip[] Clips => _audioClips;
    }
}
