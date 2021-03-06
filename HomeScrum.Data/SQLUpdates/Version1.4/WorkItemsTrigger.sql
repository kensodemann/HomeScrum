/****** Object:  Trigger [dbo].[AppendWorkItemHistory]    Script Date: 12/22/2013 7:38:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER TRIGGER [dbo].[AppendWorkItemHistory] 
   ON  [dbo].[WorkItems] 
   AFTER INSERT, UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here
	declare @sequenceNumber       integer;
	declare @id                   uniqueidentifier;
	declare @workItemRID          uniqueidentifier;
	declare @historyTimestamp     datetime;
	declare @name                 nvarchar(50);
	declare @description          nvarchar(max);
	declare @workItemTypeRID      uniqueidentifier;
	declare @workItemStatusRID    uniqueidentifier;
	declare @assignedToUserRID    uniqueidentifier;
	declare @projectRID           uniqueidentifier;
	declare @parentWorkItemRID    uniqueidentifier;
	declare @createdByUserRID     uniqueidentifier;
	declare @sprintRID            uniqueidentifier;
	declare @lastModifiedUserRID  uniqueidentifier;
	declare @points               integer;
	declare @pointsRemaining      integer;
	
	select @id = NEWID(),
	       @workItemRID = inserted.ID,
		   @historyTimestamp = SYSDATETIME(),
	       @name = inserted.Name,
	       @description = inserted.Description,
	       @workItemTypeRID = inserted.WorkItemTypeRID,
	       @workItemStatusRID = inserted.WorkItemStatusRID,
	       @assignedToUserRID = inserted.AssignedToUserRID,
	       @projectRID = inserted.ProjectRID,
	       @parentWorkItemRID = inserted.ParentWorkItemRID,
	       @createdByUserRID = inserted.CreatedByUserRID,
	       @sprintRID = inserted.SprintRID,
	       @lastModifiedUserRID = inserted.LastModifiedUserRID,
	       @sequenceNumber = coalesce( max( WorkItemHistory.SequenceNumber ), 0 ) + 1,
		   @points = inserted.Points,
		   @pointsRemaining = inserted.PointsRemaining
	  from inserted
	       left outer join WorkItemHistory
		     on WorkItemHistory.WorkItemRID = inserted.ID
	 group by inserted.ID,
	       inserted.Name,
	       inserted.Description,
	       inserted.WorkItemTypeRID,
	       inserted.WorkItemStatusRID,
	       inserted.AssignedToUserRID,
	       inserted.ProjectRID,
	       inserted.ParentWorkItemRID,
	       inserted.CreatedByUserRID,
	       inserted.SprintRID,
	       inserted.LastModifiedUserRID,
		   inserted.Points,
		   inserted.PointsRemaining;

	insert into WorkItemHistory( ID, WorkItemRID, HistoryTimestamp, SequenceNumber, Name, Description,
	                             WorkItemStatusRID, AssignedToUserRID, WorkItemTypeRID, CreatedByUserRID,
								 LastModifiedUserRID, Points, PointsRemaining )
	values( @id, @workItemRID, @historyTimestamp, @sequenceNumber, @name, @description,
	        @workItemStatusRID, @assignedToUserRID, @workItemTypeRID, @createdByUserRID,
			@lastModifiedUserRID, @points, @pointsRemaining );

END



