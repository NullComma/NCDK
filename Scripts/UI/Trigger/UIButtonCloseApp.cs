namespace NullCore.UI
{
    public class UIButtonCloseApp : UIButton
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            ClickEvent += OnClickEvent;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ClickEvent -= OnClickEvent;
        }

        void OnClickEvent()
        {
            EApplication.Quit();
        }
    }
}