using JobSearch.Domain.Models;
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
    public partial class Visualize : ContentPage
    {
        public Visualize()
        {
            InitializeComponent();
        }

        private void GoBack(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        protected override void OnAppearing()
        {                                             
            base.OnAppearing();

            Job job = ((Job)BindingContext);
            if (string.IsNullOrEmpty(job.CompanyDescription))
            {
                HeaderCompanyDesciption.IsVisible = false;
                TxtCompanyDesciption.IsVisible = false;
            }
            if (string.IsNullOrEmpty(job.Benefits))
            {
                HeaderBenefits.IsVisible = false;
                TxtBenefits.IsVisible = false;
            }
        }
    }
}