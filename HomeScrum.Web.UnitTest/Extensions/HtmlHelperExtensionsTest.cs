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

      [TestMethod]
      public void AllNewLinesConvertedToHtmlBreaks()
      {
         string testData = "line 1" + System.Environment.NewLine +
                           "line 2" + System.Environment.NewLine + System.Environment.NewLine +
                           "line 3";

         var result = _htmlHelper.DisplayFormattedText( testData );

         Assert.AreEqual( "line 1<br/>line 2<br/><br/>line 3", result.ToHtmlString() );
      }

      [TestMethod]
      public void SpecialCharactersInStringsAreEncoded()
      {
         string testData = "line <1>" + System.Environment.NewLine + "& line <2>";

         var result = _htmlHelper.DisplayFormattedText( testData );

         Assert.AreEqual( "line &lt;1&gt;<br/>&amp; line &lt;2&gt;", result.ToHtmlString() );
      }
   }
}
