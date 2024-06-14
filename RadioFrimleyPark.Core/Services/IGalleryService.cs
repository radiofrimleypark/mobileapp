using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RadioFrimleyPark.Appz.Models;

namespace RadioFrimleyPark.Core.Services
{
    public interface IGalleryService
    {
        Task<Gallery1> GetGalleryAsync();
    }
}
