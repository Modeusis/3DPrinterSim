using UnityEngine;

namespace _Project.Scripts.Audio
{
    public class AudioRandomContainer
    {
        private readonly AudioClip[] _clips;
        private AudioClip _lastPlayedClip;

        public AudioRandomContainer(AudioClip[] clips)
        {
            _clips = clips;
        }

        public AudioClip GetNextClip()
        {
            if (_clips == null || _clips.Length == 0)
                return null;

            if (_clips.Length == 1)
                return _clips[0];

            AudioClip nextClip;

            do
            {
                var randomIndex = Random.Range(0, _clips.Length);
                nextClip = _clips[randomIndex];
            }
            while (nextClip == _lastPlayedClip);

            _lastPlayedClip = nextClip;
            return nextClip;
        }
    }
}
