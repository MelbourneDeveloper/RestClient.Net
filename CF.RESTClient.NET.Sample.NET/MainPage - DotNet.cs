using Atlassian;
using CF.RESTClientDotNet;
using System;
using System.Threading.Tasks;
using System.Windows;
using ThomasBayer;
using restclientdotnet = CF.RESTClientDotNet;

namespace CF.RESTClient.NET.Sample
{
    public partial class MainPage
    {
        #region Private Methods
        private void ToggleBusy(bool isBusy)
        {
            TheActivityIndicator.IsIndeterminate = isBusy;
        }

        private async Task DisplayAlert(string title, string message)
        {
            MessageBox.Show(message, title);
        }

        private void AttachEventHandlers()
        {
            GetReposButton.Click += GetReposButton_Clicked;
            SaveButton.Click += SaveButton_Click;
            ReposBox.SelectionChanged += ReposBox_SelectionChanged;
            TheTabControl.SelectionChanged += TheTabControl_SelectionChanged;
            GetCustomersButton.Click += GetCustomersButton_Click;
        }

        private string GetPassword()
        {
            return ThePasswordBox.Password;
        }
        #endregion

        #region Event Handlers

        private async void GetCustomersButton_Click(object sender, RoutedEventArgs e)
        {
            CustomerGrid.DataContext = await GetCustomer(int.Parse(CustomerIDBox.Text));
        }

        private void ReposBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ReposPage.DataContext = ReposBox.SelectedItem as Repository;
        }

        private void GetReposButton_Clicked(object sender, EventArgs e)
        {
            OnGetReposClick();
        }
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            await OnSavedClicked();
        }

        private void TheTabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (TheTabControl.SelectedItem == CountryCodesPage)
            {
                OnCountryCodeGridLoaded();
            }
        }

        #endregion
    }
}
