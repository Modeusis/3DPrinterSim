using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Printer;
using _Project.Scripts.Setups.Audio;
using _Project.Scripts.Utilities.Events;
using _Project.Scripts.Utilities.Pool;
using Game.Scripts.Utilities.Events;
using PrimeTween;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace _Project.Scripts.Audio
{
    public class AudioService : MonoBehaviour
    {
        [Header("Mixer groups (optional)")]
        [SerializeField] private AudioMixerGroup _sfxMixerGroup;
        [SerializeField] private AudioMixerGroup _musicMixerGroup;

        [Header("SFX Player Pool")]
        [SerializeField] private AudioPlayer _sfxPlayerPrefab;
        [SerializeField] private Transform _playersContainer;
        [SerializeField, Min(1)] private int _minPoolSize = 5;
        [SerializeField, Min(1)] private int _maxPoolSize = 20;

        [Header("Music player")]
        [SerializeField] private AudioSource _musicPlayer;
        [SerializeField] private bool _isMusicPlayingOnStart = true;
        [SerializeField] private MusicType _backgroundMusicOnStart = MusicType.Background;

        [Header("Setup")]
        [SerializeField] private AudioServiceSetup _setup;

        private bool _isMusicCurrentlyPlaying;
        private Tween _musicFade;
        private Coroutine _musicPlaylistCoroutine;

        private AbstractPool<AudioPlayer> _sfxPlayerPool;

        private Dictionary<MusicType, AudioRandomContainer> _musicContainers;
        private Dictionary<LoopSoundType, LoopRuntime> _loops;

        [Inject] private EventBus _eventBus;

        private class LoopRuntime
        {
            public AudioSource Source;
            public LoopProperty Property;
            public Tween Fade;
        }

        public void Awake()
        {
            _musicPlayer?.Stop();
        }

        public void Initialize()
        {
            if (!gameObject.scene.IsValid())
            {
                Debug.LogError("[AudioService.Initialize] AudioService must live in a scene, not as a prefab asset. Move it into the scene hierarchy.");
                return;
            }

            if (_playersContainer == null || !_playersContainer.gameObject.scene.IsValid())
            {
                Debug.LogError("[AudioService.Initialize] _playersContainer is null or not a scene transform. Assign a scene Transform.");
                return;
            }

            InitializePool();
            BuildMusicContainers();
            BuildLoopRuntimes();
            SubscribeToEvents();

            if (_isMusicPlayingOnStart && !_isMusicCurrentlyPlaying)
            {
                PlayMusic(_backgroundMusicOnStart);
            }
        }

        private static bool IsSceneTransform(Transform t)
        {
            return t != null && t.gameObject.scene.IsValid();
        }

        private static void SafeSetParent(Transform child, Transform parent)
        {
            if (IsSceneTransform(parent))
            {
                child.SetParent(parent, worldPositionStays: false);
            }
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
            _musicFade.Stop();
            if (_loops != null)
            {
                foreach (var loop in _loops.Values)
                {
                    loop.Fade.Stop();
                }
            }
        }

        public void PlaySfx(SoundType soundType)
        {
            PlaySfxInternal(soundType, null);
        }

        public void PlaySfx(SoundType soundType, Transform target)
        {
            PlaySfxInternal(soundType, target);
        }

        public void PlayMusic(MusicType musicType)
        {
            if (_musicContainers == null || !_musicContainers.ContainsKey(musicType))
            {
                Debug.LogWarning($"[AudioService] No music container found for type {musicType}");
                return;
            }

            if (_musicPlaylistCoroutine != null)
            {
                StopCoroutine(_musicPlaylistCoroutine);
                _musicPlaylistCoroutine = null;
            }

            _musicPlaylistCoroutine = StartCoroutine(MusicPlaylistRoutine(musicType));
        }

        public void StopMusic()
        {
            if (_musicPlaylistCoroutine != null)
            {
                StopCoroutine(_musicPlaylistCoroutine);
                _musicPlaylistCoroutine = null;
            }

            if (_musicPlayer == null)
            {
                return;
            }

            FadeMusic(0f, _setup.MusicFadeOutDuration, () =>
            {
                _musicPlayer.Stop();
                _isMusicCurrentlyPlaying = false;
            });
        }

        public void StartLoop(LoopSoundType loopType, Transform followTarget = null)
        {
            if (_loops == null || !_loops.TryGetValue(loopType, out var loop))
            {
                Debug.LogWarning($"[AudioService] No loop runtime for {loopType}");
                return;
            }

            if (loop.Source == null || loop.Property?.Clip == null)
            {
                return;
            }

            if (IsSceneTransform(followTarget))
            {
                SafeSetParent(loop.Source.transform, followTarget);
                loop.Source.transform.localPosition = Vector3.zero;
                loop.Source.spatialBlend = 1f;
            }
            else
            {
                SafeSetParent(loop.Source.transform, _playersContainer);
                loop.Source.transform.localPosition = Vector3.zero;
                loop.Source.spatialBlend = 0f;
            }

            var targetVolume = _setup.VolumeLoop * loop.Property.VolumeScale;
            var loopPitchRange = _setup.LoopPitchRange;
            loop.Source.pitch = Random.Range(loopPitchRange.x, loopPitchRange.y);

            if (!loop.Source.isPlaying)
            {
                loop.Source.volume = 0f;
                loop.Source.Play();
            }

            loop.Fade.Stop();
            loop.Fade = Tween.AudioVolume(loop.Source, targetVolume, _setup.LoopFadeDuration);
        }

        public void StopLoop(LoopSoundType loopType)
        {
            if (_loops == null || !_loops.TryGetValue(loopType, out var loop))
            {
                return;
            }

            if (loop.Source == null || !loop.Source.isPlaying)
            {
                return;
            }

            loop.Fade.Stop();
            var source = loop.Source;
            loop.Fade = Tween.AudioVolume(source, 0f, _setup.LoopFadeDuration)
                .OnComplete(target: source, target => target.Stop());
        }

        private void PlaySfxInternal(SoundType soundType, Transform target)
        {
            if (_setup == null || _sfxPlayerPool == null)
            {
                return;
            }

            var effectProperty = _setup.Sfx.FirstOrDefault(sfx => sfx.Type == soundType);

            if (effectProperty == null || effectProperty.Clips == null || effectProperty.Clips.Length == 0)
            {
                Debug.LogWarning($"[AudioService] Property with sound type {soundType} not found or has no clips");
                return;
            }

            var sfxPlayer = _sfxPlayerPool.Get();

            var pitchRange = _setup.PitchRange;
            var pitch = Random.Range(pitchRange.x, pitchRange.y);
            var volume = _setup.VolumeSfx;

            sfxPlayer.OnReleased += ReleaseSfxPlayer;

            var spatialBlend = target ? 1.0f : 0.0f;
            var clip = effectProperty.Clips[Random.Range(0, effectProperty.Clips.Length)];

            sfxPlayer.Play(clip, volume, pitch, false, spatialBlend, target);
        }

        private void InitializePool()
        {
            if (_sfxPlayerPool != null)
            {
                return;
            }

            _sfxPlayerPool = new AbstractPool<AudioPlayer>(
                _sfxPlayerPrefab, _playersContainer, true, _minPoolSize, _maxPoolSize);
        }

        private void BuildMusicContainers()
        {
            _musicContainers = new Dictionary<MusicType, AudioRandomContainer>();

            if (_setup?.Music == null)
            {
                return;
            }

            foreach (var property in _setup.Music)
            {
                _musicContainers[property.Type] = new AudioRandomContainer(property.Clips);
            }
        }

        private void BuildLoopRuntimes()
        {
            _loops = new Dictionary<LoopSoundType, LoopRuntime>();

            if (_setup?.Loops == null)
            {
                return;
            }

            foreach (var property in _setup.Loops)
            {
                if (property?.Clip == null)
                {
                    continue;
                }

                var go = new GameObject($"Loop_{property.Type}");
                SafeSetParent(go.transform, _playersContainer);

                var source = go.AddComponent<AudioSource>();
                source.clip = property.Clip;
                source.loop = true;
                source.playOnAwake = false;
                source.volume = 0f;
                source.spatialBlend = 0f;
                source.outputAudioMixerGroup = _sfxMixerGroup;

                _loops[property.Type] = new LoopRuntime
                {
                    Source = source,
                    Property = property
                };
            }
        }

        private IEnumerator MusicPlaylistRoutine(MusicType musicType)
        {
            if (_musicPlayer == null)
            {
                yield break;
            }

            if (_isMusicCurrentlyPlaying && _musicPlayer.isPlaying)
            {
                var fadeOutComplete = false;
                FadeMusic(0f, _setup.MusicFadeOutDuration, () => fadeOutComplete = true);

                yield return new WaitUntil(() => fadeOutComplete);

                _musicPlayer.Stop();
            }

            _isMusicCurrentlyPlaying = true;
            var container = _musicContainers[musicType];

            while (_isMusicCurrentlyPlaying)
            {
                var nextClip = container.GetNextClip();

                if (nextClip == null)
                {
                    Debug.LogWarning($"[AudioService] No clips in container for {musicType}");
                    yield break;
                }

                _musicPlayer.clip = nextClip;
                _musicPlayer.volume = 0f;
                _musicPlayer.loop = false;
                _musicPlayer.outputAudioMixerGroup = _musicMixerGroup;
                _musicPlayer.Play();

                FadeMusic(_setup.VolumeMusic, _setup.MusicFadeInDuration);

                var waitDuration = nextClip.length - _setup.MusicFadeOutDuration;
                yield return new WaitForSeconds(waitDuration > 0f ? waitDuration : nextClip.length);

                var fadeOutFinished = false;
                FadeMusic(0f, _setup.MusicFadeOutDuration, () => fadeOutFinished = true);

                yield return new WaitUntil(() => fadeOutFinished);
            }
        }

        private void FadeMusic(float targetVolume, float duration, System.Action onComplete = null)
        {
            if (_musicPlayer == null)
            {
                onComplete?.Invoke();
                return;
            }

            _musicFade.Stop();
            _musicFade = Tween.AudioVolume(_musicPlayer, targetVolume, duration)
                .OnComplete(() => onComplete?.Invoke());
        }

        private void ReleaseSfxPlayer(AudioPlayer player)
        {
            player.Release();
            player.OnReleased -= ReleaseSfxPlayer;
            _sfxPlayerPool?.Release(player);
        }

        private void SubscribeToEvents()
        {
            if (_eventBus == null)
            {
                return;
            }

            _eventBus.Subscribe<OnPlugStateChanged>(HandlePlug);
            _eventBus.Subscribe<OnFilamentStateChanged>(HandleFilament);
            _eventBus.Subscribe<OnInteractableStateChangedEvent<PrinterElement>>(HandlePrinterElement);
            _eventBus.Subscribe<OnPrintProcessFinished>(HandlePrintFinished);
            _eventBus.Subscribe<OnPrintHeadMovementStateChanged>(HandlePrintHeadMovement);
            _eventBus.Subscribe<OnSpoolRotationStateChanged>(HandleSpoolRotation);
            _eventBus.Subscribe<OnHeatPanelMovementStateChanged>(HandleHeatPanelMovement);
        }

        private void UnsubscribeFromEvents()
        {
            if (_eventBus == null)
            {
                return;
            }

            _eventBus.Unsubscribe<OnPlugStateChanged>(HandlePlug);
            _eventBus.Unsubscribe<OnFilamentStateChanged>(HandleFilament);
            _eventBus.Unsubscribe<OnInteractableStateChangedEvent<PrinterElement>>(HandlePrinterElement);
            _eventBus.Unsubscribe<OnPrintProcessFinished>(HandlePrintFinished);
            _eventBus.Unsubscribe<OnPrintHeadMovementStateChanged>(HandlePrintHeadMovement);
            _eventBus.Unsubscribe<OnSpoolRotationStateChanged>(HandleSpoolRotation);
            _eventBus.Unsubscribe<OnHeatPanelMovementStateChanged>(HandleHeatPanelMovement);
        }

        private void HandlePlug(OnPlugStateChanged evt)
        {
            // PlugOut is fired directly from PrinterPlug at the start of the unplug animation.
            if (evt.IsPlugged)
            {
                PlaySfx(SoundType.PlugIn);
            }
        }

        private void HandleFilament(OnFilamentStateChanged evt)
        {
            PlaySfx(evt.IsPlaced ? SoundType.FilamentPlace : SoundType.FilamentRemove);
        }

        private void HandlePrinterElement(OnInteractableStateChangedEvent<PrinterElement> evt)
        {
            if (evt.Element != PrinterElement.Door)
            {
                return;
            }

            PlaySfx(evt.State ? SoundType.DoorOpen : SoundType.DoorClose);
        }

        private void HandlePrintFinished(OnPrintProcessFinished evt)
        {
            PlaySfx(SoundType.PrintFinished);
        }

        private void HandlePrintHeadMovement(OnPrintHeadMovementStateChanged evt)
        {
            if (evt.IsMoving)
            {
                StartLoop(LoopSoundType.PrintHeadMoving);
            }
            else
            {
                StopLoop(LoopSoundType.PrintHeadMoving);
            }
        }

        private void HandleSpoolRotation(OnSpoolRotationStateChanged evt)
        {
            if (evt.IsRotating)
            {
                StartLoop(LoopSoundType.SpoolRotating);
            }
            else
            {
                StopLoop(LoopSoundType.SpoolRotating);
            }
        }

        private void HandleHeatPanelMovement(OnHeatPanelMovementStateChanged evt)
        {
            if (evt.IsMoving)
            {
                StartLoop(LoopSoundType.HeatPanelMoving);
            }
            else
            {
                StopLoop(LoopSoundType.HeatPanelMoving);
            }
        }
    }
}
