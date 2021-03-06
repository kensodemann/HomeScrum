/****** Object:  Table [dbo].[SprintCalendar]    Script Date: 12/23/2013 9:38:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SprintCalendar](
	[Id] [uniqueidentifier] NOT NULL,
	[SprintRid] [uniqueidentifier] NOT NULL,
	[HistoryDate] [date] NOT NULL,
	[PointsRemaining] [int] NOT NULL,
 CONSTRAINT [PK_SprintCalendar] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[SprintCalendar]  WITH CHECK ADD  CONSTRAINT [FK_SprintCalendar_Sprints] FOREIGN KEY([SprintRid])
REFERENCES [dbo].[Sprints] ([ID])
GO
ALTER TABLE [dbo].[SprintCalendar] CHECK CONSTRAINT [FK_SprintCalendar_Sprints]
GO