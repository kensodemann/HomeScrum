using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HomeScrum.Web.Extensions;

namespace HomeScrum.Web.UnitTest.Extensions
{
   [TestClass]
   public class HtmlHelperExtensionsTest
   {
      [TestInitialize]
      public void InitializeTest()
      {
         _viewDataContainer = new Mock<IViewDataContainer>();
         _htmlHelper = new HtmlHelper(new ViewContext(), _viewDataContainer.Object);
      }

      private Mock<IViewDataContainer> _viewDataContainer;
      private HtmlHelper _htmlHelper;

      [TestMethod]
      public void NullStringReturnsNothing()
      {
         Assert.IsNull( _htmlHelper.DisplayFormattedText( null ) );
      }
   }
}
