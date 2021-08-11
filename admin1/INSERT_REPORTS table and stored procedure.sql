
/****** Object:  Table [dbo].[REPORTS]    Script Date: 7/15/2021 12:39:41 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[REPORTS](
	[RowID] [bigint] IDENTITY(1,1) NOT NULL,
	[UID] [bigint] NOT NULL,
	[IDToReport] [bigint] NOT NULL,
	[ProblemID] [tinyint] NOT NULL,
	[Resolved] [char](1) NOT NULL,
 CONSTRAINT [PK_REPORTS] PRIMARY KEY CLUSTERED 
(
	[RowID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[REPORTS] ADD  CONSTRAINT [DF_REPORTS_UID]  DEFAULT ((0)) FOR [UID]
GO

ALTER TABLE [dbo].[REPORTS] ADD  CONSTRAINT [DF_REPORTS_ReportID]  DEFAULT ((0)) FOR [IDToReport]
GO

ALTER TABLE [dbo].[REPORTS] ADD  CONSTRAINT [DF_REPORTS_ProblemID]  DEFAULT ((0)) FOR [ProblemID]
GO

ALTER TABLE [dbo].[REPORTS] ADD  CONSTRAINT [DF_REPORTS_Resolved]  DEFAULT ('N') FOR [Resolved]
GO

/****** Object:  StoredProcedure [dbo].[INSERT_REPORTS]    Script Date: 7/15/2021 12:00:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[INSERT_REPORTS] 
@uid bigint
,@id_to_report bigint
,@problem_id tinyint
AS
BEGIN
	SET NOCOUNT ON;

	--delete the report if it already exists
	--if the record doesn't exist, nothing will be deleted
	delete from REPORTS
	where	[UID]		= @uid
	and		IDToReport	= @id_to_report

	INSERT INTO [dbo].[REPORTS]
			   ([UID]
			   ,[IDToReport]
			   ,[ProblemID])
		 VALUES
			   (@uid
			   ,@id_to_report
			   ,@problem_id)
END
GO