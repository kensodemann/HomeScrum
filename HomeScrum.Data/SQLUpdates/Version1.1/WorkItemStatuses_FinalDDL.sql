alter table WorkItemStatuses
drop constraint CK_WorkItemStatus_IsOpen_IsBoolean;

alter table WorkItemStatuses
alter column Category integer not null;

alter table WorkItemStatuses
drop column IsOpenStatus;