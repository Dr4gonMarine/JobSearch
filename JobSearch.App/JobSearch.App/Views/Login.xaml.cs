using JobSearch.App.Models;
using JobSearch.App.Services;
using JobSearch.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Extensions;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using JobSearch.App.Utility.Loading;

namespace JobSearch.App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        private UserService _services;
        public Login()
        {
            InitializeComponent();

            _services = new UserService();
        }

        private void GoRegister(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Register());
        }

        private async void GoStart(object sender, EventArgs e)
        {
            string email = TxtEmail.Text;
            string password = TxtPassword.Text;

           await Navigation.PushPopupAsync(new Loading());
           ResponseService<User> responseService = await _services.GetUser(email, password);

            if (responseService.IsSuccess)
            {
                App.Current.Properties.Add("User", JsonConvert.SerializeObject(responseService.Data));
                await App.Current.SavePropertiesAsync();
                App.Current.MainPage = new NavigationPage(new Start());
                
            }
            else
            {
                if(responseService.StatusCode == 404)
                    await DisplayAlert("Erro!", "Nenhum usuário encontrado!", "OK");
                else
                    await DisplayAlert("Ops!", "Nenhum usuário Ocorreu um erro inesperado!", "OK");
            }   
            await Navigation.PopAllPopupAsync();
        }
    }
}