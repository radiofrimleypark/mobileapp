using System;
using System.Collections.Generic;
using System.Text;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using RadioFrimleyPark.Core.ViewModels.Base;

namespace RadioFrimleyPark.Core.ViewModels.Main
{
    public class MainContainerViewModel : BaseViewModel
    {
        public MainContainerViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
            : base(logProvider, navigationService)
        {
        }
    }
}
