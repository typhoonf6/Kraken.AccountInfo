using System;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Kraken.AccountInfo
{
    /// <summary>
    /// ViewModel for displaying list of all available coins
    /// from the API ticker
    /// </summary>
    public class SettingsViewModel : BaseViewModel
    {
        /// <summary>
        /// Command that runs when the submit button is pressed
        /// </summary>
        public AsyncCommand SubmitCommand { get; set; }

        /// <summary>
        /// Command that runs whent the delete button is pressed
        /// </summary>
        public AsyncCommand DeleteCommand { get; set; }

        /// <summary>
        /// Probes the database initially for an entry
        /// </summary>
        public AsyncCommand FillDataCommand { get; set; }

        public string PublicEntryText { get; set; }

        public  string PrivateEntryText { get; set; }

        /// <summary>
        /// Holds the API Keys
        /// </summary>
        public Keys Keys { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SettingsViewModel()
        {
            SubmitCommand = new AsyncCommand(Submit);
            DeleteCommand = new AsyncCommand(Delete);
            FillDataCommand = new AsyncCommand(FillData);
        }

        /// <summary>
        /// Checks database for existing data
        /// </summary>
        /// <returns></returns>
        private async Task FillData()
        {
            var result = await DatabaseService.GetKeysAsync();
            if (result == null)
                return;

            Keys = result;

            IsBusy = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task Delete()
        {
            await DatabaseService.DeleteKeysAsync();

            IsBusy = false;
        }

        private async Task Submit()
        {
            await DatabaseService.AddKeysAsync(PrivateEntryText, PublicEntryText);
        }
    }
}
