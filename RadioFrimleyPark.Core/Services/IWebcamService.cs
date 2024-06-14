using System;
using System.Collections.Generic;
using System.Text;

namespace RadioFrimleyPark.Core.Services
{
    public interface IWebcamService
    {
        event EventHandler<ImageEventArgs> ImageReady;
        void Start();
        void Stop();
    }
}
