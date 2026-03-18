using TMPro;
using UnityEngine;

namespace Yujanggi.Runtime.UI
{
    public class TurnUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _moveText;

        public void UpdateMoveText(int count)
        {
            _moveText.text = $"{count}수";
        }

        private void OnDestroy()
        {

        }
    }
}