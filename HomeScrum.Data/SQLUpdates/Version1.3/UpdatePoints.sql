update WorkItems
   set Points = 1
 where Points is null;

update WorkItems
   set PointsRemaining = 0
 where PointsRemaining is null
   and exists (select 'x'
                 from WorkItemStatuses
				where WorkItemStatuses.id = WorkItems.WorkItemStatusRID
				  and WorkItemStatuses.Category = 2);


update WorkItems
   set PointsRemaining = Points
 where PointsRemaining is null
   and Points is not null;