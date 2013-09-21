alter table AcceptanceCriteriaStatuses
alter column Category integer not null;

alter table AcceptanceCriteriaStatuses
drop column IsAccepted;


alter table ProjectStatuses
drop constraint CK_ProjectStatus_IsActive_IsBoolean;

alter table ProjectStatuses
alter column Category integer not null;

alter table ProjectStatuses
drop column IsActive;


alter table SprintStatuses
alter column Category integer not null;

alter table SprintStatuses
drop column IsOpenStatus;


alter table WorkItemStatuses
drop constraint CK_WorkItemStatus_IsOpen_IsBoolean;

alter table WorkItemStatuses
alter column Category integer not null;

alter table WorkItemStatuses
drop column IsOpenStatus;


alter table WorkItemTypes
alter column Category integer not null;

alter table WorkItemTypes
drop column IsTask;