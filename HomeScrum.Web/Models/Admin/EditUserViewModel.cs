
namespace HomeScrum.Web.Models.Admin
{
   public class EditUserViewModel : UserEditorViewModel
   {
      public override bool IsNewUser
      {
         get { return false; }
      }
   }
}