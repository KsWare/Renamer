namespace KsWare.Renamer {
	using BootstrapperBase = KsWare.CaliburnMicro.Common.BootstrapperBase;
	public class AppBootstrapper : BootstrapperBase {

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e) {
			base.OnStartup(sender, e);
            DisplayRootViewFor<IShell>();
        }
    }
}