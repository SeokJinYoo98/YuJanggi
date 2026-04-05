
using TMPro;

using UnityEngine;

namespace Yujanggi.Runtime.UI
{
    public class AIPanelView : UIVisible
    {
        [SerializeField] private TMP_Dropdown _teamDropdown;
        [SerializeField] private TMP_Dropdown _timeDropdown;
        [SerializeField] private TMP_Dropdown _formationDropdown;


        public int LocalPlayer          => _teamDropdown.value;
        public int TurnTime             => _timeDropdown.value;
        public int LocalPlayerFormation => _formationDropdown.value;
    }

}
