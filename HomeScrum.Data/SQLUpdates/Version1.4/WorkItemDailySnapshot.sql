with lastRow as
   (select WorkItemRID,
           cast(HistoryTimestamp as Date) historyDate,
           max( sequenceNumber ) as maxSequenceNumber
      from WorkItemHistory
     group by WorkItemRID,
           cast(HistoryTimestamp as Date))
select *
  from lastRow
 order by WorkItemRID;