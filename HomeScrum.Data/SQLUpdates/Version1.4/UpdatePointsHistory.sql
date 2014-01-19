update WorkItemHistory
   set Points = 
       (select points from WorkItems where id = WorkItemHistory.WorkItemRID)
 where points is null;

update WorkItemHistory
   set PointsRemaining = 
       (select PointsRemaining from WorkItems where id = WorkItemHistory.WorkItemRID)
 where PointsRemaining is null;