using System;
using System.Collections.Generic;
using System.Text;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace RadioFrimleyPark.Core.ViewModels.Base
{
    public abstract class BaseViewModel : MvxNavigationViewModel
    {
        public BaseViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
            : base(logProvider, navigationService)
        {
        }
    }
}
