using Atlassian;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        }

        private void GetBitBucketClient()
        {
            throw new NotImplementedException();
        }

        private string GetPassword()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Event Handlers
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
