using System;
using _Project.Scripts.Player.Interaction;
using PrimeTween;
using Game.Scripts.Utilities.Events;
using UnityEngine;

namespace _Project.Scripts.Printer.Core
{
    public class PrintedModelCollectable : MonoBehaviour, IClickable
    {
        private EventBus _eventBus;
        private float _duration;
        private float _moveY;
        private float _endScale;
        private Action _onCollected;
        private bool _isCollected;
        private bool _isCollectable;
        private Sequence _collectSequence;

        public void Init(EventBus eventBus, float duration, float moveY, float endScale, Action onCollected, bool isCollectable)
        {
            _eventBus = eventBus;
            _duration = duration;
            _moveY = moveY;
            _endScale = endScale;
            _onCollected = onCollected;
            _isCollectable = isCollectable;
        }

        public void OnBeginHover()
        {
        }

        public void OnEndHover()
        {
        }

        public void OnClick()
        {
            if (_isCollected || !_isCollectable)
            {
                return;
            }

            _isCollected = true;

            var targetPosition = transform.localPosition + Vector3.up * _moveY;
            var targetScale = Vector3.one * _endScale;

            _collectSequence = Sequence.Create()
                .Group(Tween.LocalPosition(transform, targetPosition, _duration, Ease.InOutSine))
                .Group(Tween.Scale(transform, targetScale, _duration, Ease.InOutSine))
                .OnComplete(() =>
                {
                    _onCollected?.Invoke();
                    Destroy(gameObject);
                });
        }

        public void SetCollectable(bool isCollectable)
        {
            _isCollectable = isCollectable;
        }

        private void OnDestroy()
        {
            _collectSequence.Stop();
        }
    }
}
