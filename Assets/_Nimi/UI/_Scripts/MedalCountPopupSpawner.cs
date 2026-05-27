using EMR.Medal.Hole;
using TMPro;
using UnityEngine;

namespace EMR.Medal.UI
{
    public class MedalCountPopupSpawner : MonoBehaviour
    {
        [SerializeField] CollectionHole[] _collectionHoles;
        [SerializeField] MedalCountPopup _popupPrefab;
        [SerializeField] RectTransform _popupRoot;
        [SerializeField] Camera _worldCamera;
        [SerializeField] Vector2 _screenOffset = new Vector2(0f, 24f);

        Canvas _canvas;

        void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();

            if (_popupRoot == null)
            {
                _popupRoot = transform as RectTransform;
            }

            if (_worldCamera == null)
            {
                _worldCamera = Camera.main;
            }
        }

        void OnEnable()
        {
            if (_collectionHoles == null)
            {
                return;
            }

            foreach (var hole in _collectionHoles)
            {
                if (hole != null)
                {
                    hole.OnMedalCollectedAt += Spawn;
                }
            }
        }

        void OnDisable()
        {
            if (_collectionHoles == null)
            {
                return;
            }

            foreach (var hole in _collectionHoles)
            {
                if (hole != null)
                {
                    hole.OnMedalCollectedAt -= Spawn;
                }
            }
        }

        void Spawn(Medal medal, Vector3 worldPosition)
        {
            if (_popupRoot == null || medal == null)
            {
                return;
            }

            var screenPosition = RectTransformUtility.WorldToScreenPoint(_worldCamera, worldPosition);
            screenPosition += _screenOffset;

            var uiCamera = _canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? _canvas.worldCamera
                : null;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _popupRoot,
                    screenPosition,
                    uiCamera,
                    out var anchoredPosition))
            {
                return;
            }

            var popup = _popupPrefab != null
                ? Instantiate(_popupPrefab, _popupRoot)
                : CreateDefaultPopup();

            popup.Play(medal.Count, anchoredPosition);
        }

        MedalCountPopup CreateDefaultPopup()
        {
            var popupObject = new GameObject("Medal Count Popup", typeof(RectTransform), typeof(CanvasGroup), typeof(MedalCountPopup));
            popupObject.transform.SetParent(_popupRoot, false);

            var textObject = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(popupObject.transform, false);

            var textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            var text = textObject.GetComponent<TextMeshProUGUI>();
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 36f;
            text.fontStyle = FontStyles.Bold;
            text.color = Color.white;
            text.raycastTarget = false;

            var popupRect = popupObject.GetComponent<RectTransform>();
            popupRect.sizeDelta = new Vector2(120f, 60f);

            return popupObject.GetComponent<MedalCountPopup>();
        }
    }
}
