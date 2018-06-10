namespace Zara.Main.Ui.Header
{
    public class HeaderCaller
    {
        public IHeaderUi Ui => ui ?? emptyHeaderUi;
        HeaderUi ui;
        bool isLoading = false;
        bool shouldShow = false;
        IHeaderUi emptyHeaderUi = new EmptyHeaderUi();

        public void Prepare()
        {
            if (ui != null && isLoading)
                return;

            isLoading = true;
            Game.Scene.LoadHeader(ss =>
            {
                ui = ss.Ui;
                if (shouldShow)
                    ui.Show();
                isLoading = false;
            });
        }

        public void Show()
        {
            shouldShow = true;
            Prepare();
        }

        public void Hide()
        {
            shouldShow = false;
            ui?.Hide();
        }
    }
}