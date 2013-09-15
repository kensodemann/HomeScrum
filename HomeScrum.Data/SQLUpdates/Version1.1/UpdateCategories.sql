--
-- Acceptance Criteria Statuses
--
update AcceptanceCriteriaStatuses
   set Category = 0
 where Name in ('Unverified', 'Ready for Verification');

update AcceptanceCriteriaStatuses
   set Category = 1
 where Name in ('Passed');

update AcceptanceCriteriaStatuses
   set Category = 2
 where Name in ('Failed');

update AcceptanceCriteriaStatuses
   set Category = 3
 where Name in ('Cancelled');

--
-- Project Statuses
--
update ProjectStatuses
   set Category = 0
 where Name = 'Open';

update ProjectStatuses
   set Category = 1
 where Name = 'On Hold';

update ProjectStatuses
   set Category = 2
 where Name = 'Closed';

--
-- Sprint Statuses
--
update SprintStatuses
   set Category = 0
 where name in ('In Process');

update SprintStatuses
   set Category = 1
 where Name in ('Future', 'Planning', 'Retrospective');

update SprintStatuses
   set Category = 2
 where Name in ('Complete');

--
-- Work Item Statuses
--
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

--
-- Work Item Types
--
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