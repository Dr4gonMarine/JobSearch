using JobSearch.App.Models;
using JobSearch.App.Services;
using JobSearch.App.Utility.Loading;
using JobSearch.Domain.Models;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace JobSearch.App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Register : ContentPage
    {
        private UserService _services;
        public Register()
        {
            InitializeComponent();
            _services = new UserService();
        }

        private void GoBack(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private async void SaveAction(object sender, EventArgs e)
        {
            try
            {
                TxtMessages.Text = String.Empty;

                await ValidaCampo();

                string name = TxtName.Text;
                string email = TxtEmail.Text;
                string password = TxtPassword.Text;

                User user = new User() { Name = name, Email = email, Password = password };

                await Navigation.PushPopupAsync(new Loading());
                ResponseService<User> responseService = await _services.AddUser(user);

                if (responseService.IsSuccess)
                {
                    App.Current.Properties.Add("User", JsonConvert.SerializeObject(responseService.Data));
                    await App.Current.SavePropertiesAsync();
                    App.Current.MainPage = new NavigationPage(new Start());

                }
                else
                {
                    if (responseService.StatusCode == 400)
                    {
                        StringBuilder sb = new StringBuilder();

                        foreach (var dickey in responseService.Errors)
                        {
                            foreach (var message in dickey.Value)
                            {
                                sb.AppendLine(message);
                            }
                        }
                        TxtMessages.Text = sb.ToString();
                    }
                    else
                        await DisplayAlert("Erro!", "Ocorreu um erro inesperado", "OK");
                }
                await Navigation.PopAllPopupAsync();
            }
            catch (Exception ex)
            {

                await DisplayAlert("Ops!", ex.Message, "Ok");
            }
        }
        private async Task ValidaCampo()
        {
            if(string.IsNullOrEmpty(TxtEmail.Text)|| string.IsNullOrEmpty(TxtName.Text) || string.IsNullOrEmpty(TxtPassword.Text))
            {
                throw new Exception("Campos obrigatórios estão vazios");
            }
            if (!TxtEmail.Text.Contains("@"))
            {
                throw new Exception("Email inválido");
            }
        }
    }
}