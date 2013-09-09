alter table WorkItemTypes
  add Category integer;

update WorkItemTypes
   set Category = 0
 where IsTask = 'N';

update WorkItemTypes
   set Category = 2
 where IsTask = 'Y'
   and Name <> 'SBI';

update WorkItemTypes
   set Category = 1
 where IsTask = 'Y'
   and Name = 'SBI';