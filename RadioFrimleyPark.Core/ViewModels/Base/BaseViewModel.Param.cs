using System;
using System.Collections.Generic;
using System.Text;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace RadioFrimleyPark.Core.ViewModels.Base
{
    public abstract class BaseViewModel<TParameter> : BaseViewModel, IMvxViewModel<TParameter>
        where TParameter : notnull
    {
        public BaseViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
            : base(logProvider, navigationService)
        {
        }
        public abstract void Prepare(TParameter parameter);
    }
}
