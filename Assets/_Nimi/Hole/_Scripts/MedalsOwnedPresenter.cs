using EMR.Core;
using UnityEngine;

namespace EMR.Medal.Hole
{
    public class MedalsOwnedPresenter : MonoBehaviour
    {
        [SerializeField] CollectionHole[] _collectionHole;

        void OnEnable()
        {
            foreach (var hole in _collectionHole)
            {
                hole.OnCollected += OnMedalCollected;
            }
        }

        void OnDisable()
        {
            foreach (var hole in _collectionHole)
            {
                hole.OnCollected -= OnMedalCollected;
            }
        }


        void OnMedalCollected(ICollectable collectable)
        {
            GameState.Instance.OwnedModel.AddMedal(collectable.Count);
        }
    }
};