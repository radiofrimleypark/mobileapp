using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using GoogleCast;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using RadioFrimleyPark.Core.ViewModels.Base;

namespace RadioFrimleyPark.Core.ViewModels
{
    public class GoogleCastViewModel : BaseViewModel<Uri>
    {
        public IMvxCommand OkCommand => new MvxAsyncCommand(() => NavigationService.Close(this));

        private IMvxAsyncCommand _refreshCommand;
        public IMvxAsyncCommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new MvxAsyncCommand(ExecuteRefreshCommandAsync));

        private bool _isBusy;
        public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }

        private ObservableCollection<IReceiver> _items;
        public ObservableCollection<IReceiver> Items { get => _items; set => SetProperty(ref _items, value); }

        public GoogleCastViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
            : base(logProvider, navigationService)
        { }

        public override Task Initialize()
        {
            return base.Initialize();
        }
        public override void Prepare(Uri parameter)
        { }

        private async Task ExecuteRefreshCommandAsync()
        {
            IsBusy = true;
            await Initialize();
            IsBusy = false;
        }

    }
}
