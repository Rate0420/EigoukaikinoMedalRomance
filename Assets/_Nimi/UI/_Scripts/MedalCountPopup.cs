using System.Collections;
using TMPro;
using UnityEngine;

namespace EMR.Medal.UI
{
    public class MedalCountPopup : MonoBehaviour
    {
        [SerializeField] TMP_Text _countText;
        [SerializeField] CanvasGroup _canvasGroup;
        [SerializeField] float _duration = 0.8f;
        [SerializeField] Vector2 _moveOffset = new Vector2(0f, 80f);
        [SerializeField] AnimationCurve _moveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] AnimationCurve _alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

        RectTransform _rectTransform;
        Coroutine _playingCoroutine;

        void Awake()
        {
            Initialize();
        }

        public void Play(int count, Vector2 anchoredPosition)
        {
            Initialize();

            if (_countText == null || _rectTransform == null)
            {
                Destroy(gameObject);
                return;
            }

            _countText.text = $"+{count}";
            _rectTransform.anchoredPosition = anchoredPosition;

            if (_playingCoroutine != null)
            {
                StopCoroutine(_playingCoroutine);
            }

            _playingCoroutine = StartCoroutine(PlayAnimation(anchoredPosition));
        }

        void Initialize()
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }

            if (_countText == null)
            {
                _countText = GetComponentInChildren<TMP_Text>();
            }

            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }
        }

        IEnumerator PlayAnimation(Vector2 startPosition)
        {
            var elapsed = 0f;

            while (elapsed < _duration)
            {
                elapsed += Time.deltaTime;
                var progress = Mathf.Clamp01(elapsed / _duration);

                _rectTransform.anchoredPosition = Vector2.LerpUnclamped(
                    startPosition,
                    startPosition + _moveOffset,
                    _moveCurve.Evaluate(progress));

                if (_canvasGroup != null)
                {
                    _canvasGroup.alpha = _alphaCurve.Evaluate(progress);
                }

                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
