using EMR.Medal.Hole;
using EMR.Medal.UI;
using EMR.Medal;
using UnityEngine;

namespace EMR.Medal
{
    public class MedalsOwnedPresenter : MonoBehaviour
    {
        [SerializeField] CollectionHole _collectionHole;
        [SerializeField] MedalsOwnedText _medalsOwnedView;

        MedalsOwnedModel _model;

        void Awake()
        {
            _model = new MedalsOwnedModel();
        }

        void OnEnable()
        {
            _collectionHole.OnMedalCollected += OnMedalCollected;
            _model.OnCountChanged += _medalsOwnedView.SetMedalsOwnedCount;
        }

        void OnDisable()
        {
            _collectionHole.OnMedalCollected -= OnMedalCollected;
            _model.OnCountChanged -= _medalsOwnedView.SetMedalsOwnedCount;
        }

        void Start()
        {
            _medalsOwnedView.SetMedalsOwnedCount(_model.Count);
        }

        void OnMedalCollected(Medal medal)
        {
            _model.AddMedal(medal.Count);
        }
    }
}