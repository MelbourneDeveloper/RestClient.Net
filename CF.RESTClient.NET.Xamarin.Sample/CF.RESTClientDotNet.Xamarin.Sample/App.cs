using groupkt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace CF.RESTClientDotNet.Xamarin.Sample
{
    public class App : Application
    {
        Label _Label;

        public App()
        {
            _Label = new Label
            {
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "..."
            };

            // The root page of your application
            MainPage = new ContentPage
            {
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        _Label
                    }
                }
            };
        }

        protected async override void OnStart()
        {
            var countryCodeClient = new RESTClient(new NewtonsoftSerializationAdapter(), new Uri("http://services.groupkt.com/country/get/all"));

            var countryData = await countryCodeClient.GetAsync<groupktResult<CountriesResult>>();
            _Label.Text = countryData.RestResponse.result.FirstOrDefault().name;
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
