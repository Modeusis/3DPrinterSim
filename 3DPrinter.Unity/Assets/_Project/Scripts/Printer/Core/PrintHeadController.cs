using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using _Project.Scripts.Setups.Printer;

namespace _Project.Scripts.Printer.Core
{
    public class PrinterHeadController : MonoBehaviour
    {
        [Header("Элементы принтера")]
        [Tooltip("Балка (ось), которая двигается по локальной оси Y")]
        [SerializeField] private Transform _yAxis;
        [Tooltip("Сам экструдер, который двигается по локальной оси X (дочерний объект балки или независимый)")]
        [SerializeField] private Transform _extruderX;
    
        [Header("Настройки скорости (ед. Unity в секунду)")]
        [SerializeField] private SpeedProfilesSetup _speedProfiles;
        
        public bool IsMoving { get; private set; }
        public Vector2 StartPoint { get; private set; }

        private void Awake()
        {
            StartPoint = new Vector2(_extruderX.localPosition.x, _yAxis.localPosition.y);
        }
        
        public async UniTask MoveToPointAsync(Vector2 targetLocalPoint, SpeedType speedType, CancellationToken token = default)
        {
            if (IsMoving)
            {
                Debug.LogWarning("Головка уже находится в движении! Дождитесь завершения или отмените текущую задачу.");
                return;
            }
    
            IsMoving = true;
    
            try
            {
                var speed = _speedProfiles.GetSpeed(speedType);
                
                var startX = _extruderX.localPosition.x;
                var startY = _yAxis.localPosition.y;
    
                var targetX = targetLocalPoint.x;
                var targetY = targetLocalPoint.y;

                var dx = targetX - startX;
                var dy = targetY - startY;
                var totalDistance = Mathf.Sqrt(dx * dx + dy * dy);

                if (totalDistance < 0.001f) return;

                var duration = totalDistance / speed;
                var elapsedTime = 0f;

                while (elapsedTime < duration)
                {
                    token.ThrowIfCancellationRequested();
    
                    elapsedTime += Time.deltaTime;
                    
                    var t = Mathf.Clamp01(elapsedTime / duration); 
                    
                    var currentX = Mathf.Lerp(startX, targetX, t);
                    var currentY = Mathf.Lerp(startY, targetY, t);
                    
                    _extruderX.localPosition = new Vector3(currentX, _extruderX.localPosition.y, _extruderX.localPosition.z);
                    _yAxis.localPosition = new Vector3(_yAxis.localPosition.x, currentY, _yAxis.localPosition.z);
                    
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }
                
                _extruderX.localPosition = new Vector3(targetX, _extruderX.localPosition.y, _extruderX.localPosition.z);
                _yAxis.localPosition = new Vector3(_yAxis.localPosition.x, targetY, _yAxis.localPosition.z);
            }
            finally
            {
                IsMoving = false;
            }
        }

        public UniTask MoveToStartAsync(SpeedType speedType, CancellationToken token = default)
        {
            return MoveToPointAsync(StartPoint, speedType, token);
        }
    }
}