using System;
using System.Collections.Generic;
using UnityEngine;

namespace EMR.Round
{
    /// <summary>
    /// ラウンド進行に必要な条件をInspectorから設定するコンポーネント。
    /// </summary>
    public class RoundProgressionSettings : MonoBehaviour
    {
        private const int DefaultRoundCount = 8;

        /// <summary>
        /// ラウンドごとの進行条件。
        /// リストの0番目はラウンド1、1番目はラウンド2に対応する。
        /// </summary>
        [SerializeField] private List<RoundRequirement> _roundRequirements = new();

        private void Reset()
        {
            _roundRequirements = new List<RoundRequirement>(DefaultRoundCount);

            for (int i = 0; i < DefaultRoundCount; i++)
            {
                _roundRequirements.Add(new RoundRequirement());
            }
        }

        /// <summary>
        /// 指定したラウンドの進行条件を取得する。
        /// </summary>
        /// <param name="roundNumber">取得するラウンド番号。1から開始する。</param>
        /// <returns>対象ラウンドの進行条件</returns>
        public RoundRequirement GetRequirement(int roundNumber)
        {
            if (roundNumber <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(roundNumber),
                    "ラウンド番号は1以上にしてください。");
            }

            int index = roundNumber - 1;

            if (index >= _roundRequirements.Count)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(roundNumber),
                    $"ラウンド{roundNumber}の進行条件が設定されていません。");
            }

            return _roundRequirements[index];
        }
    }

    /// <summary>
    /// 1ラウンドを進行するために必要な条件。
    /// </summary>
    [Serializable]
    public class RoundRequirement
    {
        /// <summary>
        /// ラウンド進行に必要なボール数。
        /// </summary>
        [field: SerializeField, Min(0)]
        public int RequiredBallCount { get; private set; } = 2;

        /// <summary>
        /// ラウンド進行に必要なメダル数。
        /// </summary>
        [field: SerializeField, Min(0)]
        public int RequiredMedalCount { get; private set; } = 75;
    }
}