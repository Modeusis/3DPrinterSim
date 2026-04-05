using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace _Project.Scripts.Printer.Core
{
    public class HeatPanelController : MonoBehaviour
    {
        [SerializeField] private Transform _panelTransform;

        [field: SerializeField] public float MoveSpeed { get; private set; } = 5f;

        public async UniTask MoveToHeightAsync(float targetZ, CancellationToken token)
        {
            Vector3 startPos = _panelTransform.localPosition;
            Vector3 targetPos = new Vector3(startPos.x, startPos.y, targetZ);
        
            float distance = Mathf.Abs(targetZ - startPos.z);
            float duration = distance / MoveSpeed;
            float elapsed = 0;

            while (elapsed < duration)
            {
                token.ThrowIfCancellationRequested();
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
            
                _panelTransform.localPosition = Vector3.Lerp(startPos, targetPos, t);
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            _panelTransform.localPosition = targetPos;
        }
    }
}