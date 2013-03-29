using HomeScrum.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models
{
   public class AcceptanceCriteriaStatusEditorViewModel : DomainObjectEditorViewModel<AcceptanceCriteriaStatus> { }

   public class ProjectStatusEditorViewModel : DomainObjectEditorViewModel<ProjectStatus> { }

   public class SprintStatusEditorViewModel : DomainObjectEditorViewModel<SprintStatus> { }

   public class WorkItemStatusEditorViewModel : DomainObjectEditorViewModel<WorkItemStatus> { }

   public class WorkItemTypeEditorViewModel : DomainObjectEditorViewModel<WorkItemType> { }
}