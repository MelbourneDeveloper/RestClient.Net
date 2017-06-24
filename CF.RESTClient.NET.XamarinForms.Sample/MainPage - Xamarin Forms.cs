using Atlassian;
using CF.RESTClientDotNet;
using groupkt;
using System;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using restclientdotnet = CF.RESTClientDotNet;

namespace CF.RESTClient.NET.Sample
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage
    {
        #region Event Handlers
        private void MainPage_CurrentPageChanged(object sender, EventArgs e)
        {
            if (CurrentPage == CountryCodesPage)
            {
                OnCountryCodeGridLoaded();
            }
        }

        private void ReposBox_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ReposPage.BindingContext = ReposBox.SelectedItem as Repository;
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            await OnSavedClicked();
        }

        private void GetReposButton_Clicked(object sender, EventArgs e)
        {
            OnGetReposClick();
        }

        #endregion

        #region Private Methods

        private string GetPassword()
        {
            return ThePasswordBox.Text;
        }

        private async Task DisplayAlert(string title, string message)
        {
            await DisplayAlert(title, message, "OK");
        }

        private void ToggleBusy(bool isBusy)
        {
            ReposActivityIndicator.IsVisible = true;
            CountryCodesActivityIndicator.IsVisible = true;
            ReposActivityIndicator.IsRunning = isBusy;
            CountryCodesActivityIndicator.IsRunning = isBusy;
        }

        private void AttachEventHandlers()
        {
            SaveButton.Clicked += SaveButton_Clicked;
            CurrentPageChanged += MainPage_CurrentPageChanged;
            GetReposButton.Clicked += GetReposButton_Clicked;
            ReposBox.ItemSelected += ReposBox_ItemSelected;
        }
        #endregion
    }
}
