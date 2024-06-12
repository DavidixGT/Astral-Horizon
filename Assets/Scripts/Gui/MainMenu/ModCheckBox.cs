using UnityEngine;

namespace Gui.MainMenu
{
    public class ModCheckBox
    {
        
        private GameObject _activeModMark;
        public ModCheckBox(GameObject activeModMark)
        {
             _activeModMark = activeModMark;
             _activeModMark.SetActive(false);
        }
        public void Check(bool check)
        {
            _activeModMark.SetActive(check);


            //Debug.LogError(_totalEnabledMods);          
        }
    }
}