alter table WorkItemTypes
alter column Category integer not null;

alter table WorkItemTypes
drop column IsTask;