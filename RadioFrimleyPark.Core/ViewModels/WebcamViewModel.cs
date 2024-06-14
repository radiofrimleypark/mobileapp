
using System;
using System.Collections.Generic;
using System.Text;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using RadioFrimleyPark.Core.Services;
using RadioFrimleyPark.Core.ViewModels.Base;

namespace RadioFrimleyPark.Core.ViewModels
{
    public class WebcamViewModel : BaseViewModel
    {
        private IWebcamService _webcamService;
        public WebcamViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService, IWebcamService webcamService)
            : base(logProvider, navigationService)
        {
            _webcamService = webcamService;
            _webcamService.ImageReady += (s, e) => { ImageReady?.Invoke(s, e); };
        }
        public event EventHandler<ImageEventArgs> ImageReady;

        public void Start()
        {
            _webcamService.Start();
        }
        public void Stop()
        {
            _webcamService.Stop();
        }
    }
}
