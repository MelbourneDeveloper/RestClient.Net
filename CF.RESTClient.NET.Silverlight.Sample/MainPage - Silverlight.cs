﻿using Atlassian;
using System;
using System.Threading.Tasks;
using System.Windows;
using restclientdotnet = CF.RESTClientDotNet;

namespace CF.RESTClient.NET.Sample
{
    public partial class MainPage
    {
        #region Event Handlers
        private void GetRepos_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            GetReposClick();
        }
        #endregion

        #region Private Methods
        private void AttachEventHandlers()
        {
        }

        private async Task DisplayAlert(string message)
        {
            MessageBox.Show(message);
        }

        private void GetBitBucketClient()
        {
            string url = "http://localhost:49902/api/BitBucketRepository/" + UsernameBox.Text + "-" + ThePasswordBox.Password;
            _BitbucketClient = new restclientdotnet.RESTClient(new restclientdotnet.NewtonsoftSerializationAdapter(), new Uri(url));
            _BitbucketClient.ErrorType = typeof(ErrorModel);
        }

        private void ToggleReposBusy(bool isBusy)
        {
            ReposActivityIndicator.IsIndeterminate = isBusy;
        }

        #endregion
    }
}