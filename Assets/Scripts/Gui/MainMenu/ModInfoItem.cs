using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gui.MainMenu
{
    public class ModInfoItem : MonoBehaviour
    {
        [SerializeField] private Text _name;
        [SerializeField] private GameObject _activeModMark;
        private ModCheckBox _modCheckBox;
        private bool _active;
        public string Id { get; private set; }

        public void Initialize(string name, string id, bool active)
        {
            _active = active;
            _name.text = name;
            Id = id;
            _modCheckBox = new ModCheckBox(_activeModMark);
            
            _modCheckBox.Check(active);
        }
        public bool IsEnabled() => _active;
    }
}
