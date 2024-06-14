using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using RadioFrimleyPark.Appz.Models;
using RadioFrimleyPark.Core.Services;
using RadioFrimleyPark.Core.ViewModels.Base;

namespace RadioFrimleyPark.Core.ViewModels
{
    public class GalleryViewModel : BaseViewModel
    {
        private IGalleryService _galleryService;
        private ObservableCollection<Event> _items;
        public ObservableCollection<Event> Items { get => _items; set => SetProperty(ref _items, value); }

        public GalleryViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService, IGalleryService galleryService)
            : base(logProvider, navigationService)
        {
            _galleryService = galleryService;
        }

        public override async Task Initialize()
        {
            await base.Initialize();
            _items = new ObservableCollection<Event>();
            foreach (Event item in await _galleryService.GetGalleryAsync())
                _items.Add(item);
        }
    }
}
