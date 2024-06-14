using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using RadioFrimleyPark.Appz.Models;
using RadioFrimleyPark.Core.Services;
using RadioFrimleyPark.Core.ViewModels.Base;

namespace RadioFrimleyPark.Core.ViewModels
{
    public class ScheduleViewModel : BaseViewModel
    {
        public Uri Home { get; } = new Uri("http://www.radiofrimleypark.co.uk/schedule.php");
        private IScheduleService _scheduleService;
        public ScheduleViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService, IScheduleService scheduleService)
            : base(logProvider, navigationService)
        {
            _scheduleService = scheduleService;
        }
        public async Task<Schedule> TodaysScheduleAsync()
        {
            return await _scheduleService.GetTodaysScheduleAsync();
        }
    }
}
