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
                hole.OnMedalCollected += OnMedalCollected;
            }
        }

        void OnDisable()
        {
            foreach (var hole in _collectionHole)
            {
                hole.OnMedalCollected -= OnMedalCollected;
            }
        }


        void OnMedalCollected(Medal medal)
        {
            GameState.Instance.OwnedModel.AddMedal(medal.Count);
        }
    }
};