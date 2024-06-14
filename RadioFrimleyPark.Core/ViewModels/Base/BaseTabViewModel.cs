using System;
using System.Collections.Generic;
using System.Text;
using MvvmCross.Logging;
using MvvmCross.Navigation;

namespace RadioFrimleyPark.Core.ViewModels.Base
{
    public class BaseTabViewModel : BaseViewModel
    {
        public string TabName { get; protected set; }
        public string TabId { get; protected set; }

        public BaseTabViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
            : base(logProvider, navigationService)
        { }
    }
}
