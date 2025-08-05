
using UnityEngine;

namespace EnigmaCore.UI {
    public class CUIButtonMenuOpener : CUIButton {

        [SerializeField] View _menuToOpen;


        protected override void OnEnable()
        {
            base.OnEnable();
            ClickEvent += OnOnClick;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ClickEvent -= OnOnClick;
        }

        void OnOnClick()
        {
            Instantiate(_menuToOpen).gameObject.SetActive(true);
        }
    }
}