using System;
using System.Collections.Generic;
using System.Text;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using RadioFrimleyPark.Core.ViewModels.Base;

namespace RadioFrimleyPark.Core.ViewModels
{
    public class ListenViewModel : BaseViewModel<Uri>
    {
        public IMvxCommand OkCommand => new MvxAsyncCommand(() => NavigationService.Close(this));

        public static Uri StreamUri { get; set; }

        public ListenViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
            : base(logProvider, navigationService)
        {
            StreamUri = new Uri("http://bedside.radiofrimleypark.co.uk/stream");
        }
        public override void Prepare(Uri parameter)
        {
            StreamUri = new Uri("http://bedside.radiofrimleypark.co.uk/stream");
            //StreamUri = new Uri("http://public.radiofrimleypark.co.uk/stream");
            //StreamUri = parameter;
        }
    }
}
