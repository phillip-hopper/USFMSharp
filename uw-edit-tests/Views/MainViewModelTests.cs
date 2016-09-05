using NUnit.Framework;
using System;
using uw_edit.Views;

namespace uw_edit_tests
{
	[TestFixture()]
	public class MainViewModelTests
	{
		[Test()]
		public void CreateInstance()
		{
			var mainViewModel = new MainViewModel();
			Assert.IsInstanceOf<MainViewModel>(mainViewModel);
		}
	}
}

