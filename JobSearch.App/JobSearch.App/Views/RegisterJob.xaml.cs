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
using JobSearch.App.Utility.Converter;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace JobSearch.App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterJob : ContentPage
    {
        private JobService _services;
        public RegisterJob()
        {
            InitializeComponent();
            _services = new JobService();
        }

        private void GoBack(object sender, EventArgs e)
        {
            Navigation.PopAsync();  
        }

        private async void Save(object sender, EventArgs e)
        {
            try
            {
                TxtMessages.Text = String.Empty;
                await ValidacaoCampos();
                User user = JsonConvert.DeserializeObject<User>(App.Current.Properties["User"].ToString());

                Job job = new Job()
                {
                    Company = TxtCompany.Text,
                    JobTitle = TxtJobTitle.Text,
                    CityState = TxtCityState.Text,
                    InitialSalary = TextToDoubleConverter.ToDouble(TxtInitialSalary.Text),
                    FinalSalary = TextToDoubleConverter.ToDouble(TxtFinalSalary.Text),
                    ContractType = (RBCLT.IsChecked) ? "CLT" : "PJ",
                    TecnologyTools = TxtTecnologyTools.Text,
                    CompanyDescription = TxtCompanyDescription.Text,
                    JobDescription = TxtJobDescription.Text,
                    Benefits = TxtBenefits.Text,
                    InterestedSendEmailTo = TxtInterestedSendEmailTo.Text,
                    PublicationDate = DateTime.Now,
                    UserId = user.Id
                };

                await Navigation.PushPopupAsync(new Loading());
                
                ResponseService<Job> responseService = await _services.AddJob(job);

                if (responseService.IsSuccess)
                {
                    await Navigation.PopAllPopupAsync();
                    await DisplayAlert("Vaga cadastrada!", "Vaga cadastrada com sucesso", "OK");
                    await Navigation.PopAsync();
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
                    {
                        await Navigation.PopAllPopupAsync();
                        await DisplayAlert("Erro!", "Ocorreu um erro inesperado", "OK");
                    }
                }   
            }
            catch (Exception ex)
            {
                await DisplayAlert("Aviso!", ex.Message, "Ok");                
            }
        }

        private async Task ValidacaoCampos()
        {
            if (string.IsNullOrWhiteSpace(TxtCompany.Text) || string.IsNullOrWhiteSpace(TxtJobTitle.Text) || string.IsNullOrWhiteSpace(TxtCityState.Text) || string.IsNullOrWhiteSpace(TxtInitialSalary.Text) || string.IsNullOrWhiteSpace(TxtFinalSalary.Text)
                || string.IsNullOrWhiteSpace(TxtTecnologyTools.Text) || string.IsNullOrWhiteSpace(TxtJobDescription.Text) || string.IsNullOrWhiteSpace(TxtInterestedSendEmailTo.Text))
            {
                throw new Exception("Campos obrigatórios");
            }  
            if (!TxtInterestedSendEmailTo.Text.Contains("@")){
                throw new Exception("Email inválido");
            }
        }
    }
}