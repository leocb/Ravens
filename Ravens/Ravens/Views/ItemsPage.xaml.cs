using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Ravens.Models;
using Ravens.Views;
using Ravens.ViewModels;
using Firebase.Auth;
using Firebase.Database;
using Ravens.Helpers;
using Xamarin.Auth;

namespace Ravens.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel viewModel;

        public ItemsPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new ItemsViewModel();
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as Item;
            if (item == null)
                return;

            await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Request");
            requestFirebaseLogin();
        }

        private async void requestFirebaseLogin()
        {
            // TODO Refactor this into a class
            System.Diagnostics.Debug.WriteLine("Begin");
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Secrets.FirebaseApiKey));

            // TODO enable third party login
            // see https://medium.com/step-up-labs/firebase-authentication-c-library-8e5e1c30acc2
            // and https://docs.microsoft.com/en-us/xamarin/xamarin-forms/data-cloud/authentication/oauth
            // var token = "";
            // var auth = await authProvider.SignInWithOAuthAsync(FirebaseAuthType.Google, token);

            // For now, just login with email and password
            System.Diagnostics.Debug.WriteLine("Logging in...");
            var auth = await authProvider.SignInWithEmailAndPasswordAsync("userEmail", "userPassword");

            var firebase = new FirebaseClient(
              "https://project-ravens.firebaseio.com/",
              new FirebaseOptions
              {
                  AuthTokenAsyncFactory = () => Task.FromResult(auth.FirebaseToken)
              });

            /*System.Diagnostics.Debug.WriteLine("Request data");
            var data = await firebase
              .Child("test")
              .OnceAsync<TestData>();

            foreach (var testData in data)
            {
                System.Diagnostics.Debug.WriteLine($"{testData.Key} says {testData.Object.text}");
            }
            */
            System.Diagnostics.Debug.WriteLine("Done");

        }
    }

    internal class TestData
    {
        public string text;
    }
}