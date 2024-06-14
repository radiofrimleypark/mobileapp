using MvvmCross.IoC;
using MvvmCross.ViewModels;
using RadioFrimleyPark.Core.ViewModels.Main;

namespace RadioFrimleyPark.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            RegisterAppStart<MainViewModel>();
        }
    }
}
