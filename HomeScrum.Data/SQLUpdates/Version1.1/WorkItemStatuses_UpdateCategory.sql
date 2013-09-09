update WorkItemStatuses
   set Category = 2
 where IsOpenStatus = 'N';

 update WorkItemStatuses
    set Category = 0
  where IsOpenStatus = 'Y'
    and Name = 'New';

update WorkItemStatuses
   set Category = 1
 where IsOpenStatus = 'Y'
   and Category is null;