IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[WorkItemDailySnapshot]'))
    EXEC sp_executesql N'CREATE VIEW [dbo].[WorkItemDailySnapshot] AS SELECT ''This is a code stub which will be replaced by an Alter Statement'' as [code_stub]'
GO

ALTER VIEW [dbo].[WorkItemDailySnapshot]
AS
with lastRow as
   (select WorkItemRID,
           cast(HistoryTimestamp as Date) historyDate,
           max( sequenceNumber ) as maxSequenceNumber
      from WorkItemHistory
     group by WorkItemRID,
           cast(HistoryTimestamp as Date))
select workItems.Id as workItemRid,
       lastRow.HistoryDate,
	   dense_rank()
	      over( partition by lastRow.workItemRid
		        order by lastRow.HistoryDate desc ) as SortSequenceNumber,
       workItems.Points,
       workItems.PointsRemaining
  from lastRow
       join workItems
	     on workItems.id = lastRow.workItemRid;
GO