using HomeScrum.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models
{
   public class EditAcceptanceCriteriaStatusViewModel : EditDomainObjectViewModel<AcceptanceCriteriaStatus> { }

   public class EditProjectStatusViewModel : EditDomainObjectViewModel<ProjectStatus> { }

   public class EditSprintStatusViewModel : EditDomainObjectViewModel<SprintStatus> { }

   public class EditWorkItemStatusViewModel : EditDomainObjectViewModel<WorkItemStatus> { }

   public class EditWorkItemTypeViewModel : EditDomainObjectViewModel<WorkItemType> { }
}