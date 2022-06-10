using JobSearch.App.Models;
using JobSearch.App.Services;
using JobSearch.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace JobSearch.App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Start : ContentPage
    {
        private JobService _services;
        private ObservableCollection<Job> _ListOfJobs;
        private SearchParams _SearchParams;
        private int _ListOfJobsFirstRequest;

        public Start()
        {
            InitializeComponent();
            _services = new JobService();
        }

        private void GoVisualize(object sender, EventArgs e)
        {
            var eventArgs = (TappedEventArgs)e;

            var page = new Visualize();
            page.BindingContext = eventArgs.Parameter;
            Navigation.PushAsync(page);
        }

        private void GoRegisterJob(object sender, EventArgs e)
        {
            Navigation.PushAsync(new RegisterJob());
        }

        private void FocusPesquisa(object sender, EventArgs e)
        {
            TxtPesquisa.Focus();
        }

        private void FocusCityState(object sender, EventArgs e)
        {
            TxtCityState.Focus();
        }

        private void LogOut(object sender, EventArgs e)
        {
            App.Current.Properties.Remove("User");
            App.Current.SavePropertiesAsync();
            App.Current.MainPage = new NavigationPage(new Login());
        }

        private async void Search(object sender, EventArgs e)
        {
            TxtResultCount.Text = String.Empty;
            NoResult.IsVisible = false;
            Loading.IsVisible = true;
            Loading.IsRunning = true;            

            string word = TxtPesquisa.Text; 
            string CityState = TxtCityState.Text;   
            
            _SearchParams = new SearchParams() { Word = word, CityState = CityState, NumberOfPage = 1 };

            ResponseService<List<Job>> responseService = await _services.GetJobs(_SearchParams.Word, _SearchParams.CityState, _SearchParams.NumberOfPage);

            if (responseService.IsSuccess)
            {
                //colocar dentro da collection
                _ListOfJobs = new ObservableCollection<Job>(responseService.Data);
                _ListOfJobsFirstRequest = _ListOfJobs.Count();
                ListOfJobs.ItemsSource = _ListOfJobs;
                ListOfJobs.RemainingItemsThreshold = 1;

                TxtResultCount.Text = $"{responseService.Pagination.TotalItems} resultado(s)";
            }
            else
            {
               await DisplayAlert("Erro", "Erro" ,"OK");
            }

            if(_ListOfJobs.Count == 0)
                NoResult.IsVisible = true;
            else
                NoResult.IsVisible = false;

            Loading.IsVisible = false;
            Loading.IsRunning = false;
        }
        
        private async void InfinitySearch(object sender, EventArgs e)
        {
            ListOfJobs.RemainingItemsThreshold = -1;
            _SearchParams.NumberOfPage++;           

            ResponseService<List<Job>> responseService = await _services.GetJobs(_SearchParams.Word, _SearchParams.CityState, _SearchParams.NumberOfPage);

            if (responseService.IsSuccess)
            {
                //colocar dentro da collection
                var listOfJobsFromService = responseService.Data;
                foreach(var item in listOfJobsFromService)
                {
                    _ListOfJobs.Add(item);
                }      
                if(_ListOfJobsFirstRequest == listOfJobsFromService.Count)
                    ListOfJobs.RemainingItemsThreshold = 0;
                else
                    ListOfJobs.RemainingItemsThreshold = -1;
            }
            else
            {
                await DisplayAlert("Erro", "Erro", "OK");
                ListOfJobs.RemainingItemsThreshold = 0;
            }            
        }
    }
}