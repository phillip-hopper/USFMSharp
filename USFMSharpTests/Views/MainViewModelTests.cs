using NUnit.Framework;
using USFMSharp.Views;

namespace USFMSharp_tests.Views
{
	[TestFixture]
	public class MainViewModelTests
	{
		[Test]
		public void CreateInstance()
		{
			var mainViewModel = new MainViewModel();
			Assert.IsInstanceOf<MainViewModel>(mainViewModel);
		}
	}
}

