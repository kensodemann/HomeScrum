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
select WorkItemHistory.WorkItemRID,
       lastRow.HistoryDate,
	   dense_rank()
	      over( partition by lastRow.workItemRid
		        order by lastRow.HistoryDate desc ) as SortSequenceNumber,
       WorkItemHistory.Points,
       WorkItemHistory.PointsRemaining
  from lastRow
       join WorkItemHistory
	     on WorkItemHistory.WorkItemRID = lastRow.workItemRid and
		    WorkItemHistory.SequenceNumber = lastRow.maxSequenceNumber;
GO