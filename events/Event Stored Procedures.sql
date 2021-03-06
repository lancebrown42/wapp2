SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DELETE_EVENT]
@id bigint
AS
BEGIN
	SET NOCOUNT ON;

	delete from EVENT_IMAGES where EventID=@id
	delete from [EVENTS] where EventID=@id
	return @@rowcount
END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DELETE_EVENT_IMAGE]
@id bigint
AS
BEGIN
	SET NOCOUNT ON;

	delete from EVENT_IMAGES where EventImageID=@id

	return @@rowcount
END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[INSERT_EVENT_IMAGE] 
@event_image_id bigint = null output
,@event_id bigint
,@primary_image nchar(1)
,@image varbinary(max)
,@file_name nvarchar(1000)
,@image_size bigint
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[EVENT_IMAGES]
				([EventID]
				,[PrimaryImage]
				,[Image]
				,[FileName]
				,[ImageSize])
			VALUES
				(@event_id
				,@primary_image
				,@image
				,@file_name
				,@image_size)

	select @event_image_id=@@identity
	return 1


END


GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[INSERT_EVENTS] 
@id bigint = null output
,@owner_uid bigint
,@title nvarchar(1000)
,@desc nvarchar(max)
,@start_date datetime
,@end_date datetime
,@location_title nvarchar(1000)
,@Location_desc nvarchar(max)
,@address1 nvarchar(500)
,@address2 nvarchar(500)
,@city nvarchar(100)
,@state nvarchar(100)
,@zip nvarchar(20)
,@is_active char(1)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[EVENTS]
		([OwnerUID]
		,[Title]
		,[Desc]
		,[StartDate]
		,[EndDate]
		,[LocationTitle]
		,[LocationDesc]
		,[Address1]
		,[Address2]
		,[City]
		,[State]
		,[Zip]
		,[IsActive])
	VALUES
		(@owner_uid
		,@title
		,@desc
		,@start_date
		,@end_date
		,@location_title
		,@location_desc
		,@address1
		,@address2
		,@city
		,@state
		,@zip
		,@is_active)

	select @id=@@IDENTITY
	return 1 --success

END
GO

SET ANSI_NULLS ON

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SELECT_EVENT_IMAGES]
@event_id bigint = null
,@event_image_id bigint = null
,@primary_only char(1) = 'N'
AS
BEGIN
	SET NOCOUNT ON;

	if @event_id is not null
	begin
		if @primary_only = 'Y'
		begin
			select *
			from EVENT_IMAGES
			where [EventID] = @event_id
			and PrimaryImage = 'Y'
			order by DateAdded desc
		end else
		begin
			select *
			from EVENT_IMAGES
			where [EventID] = @event_id
			and PrimaryImage = 'N'
			order by DateAdded desc
		end
	end
	else
	begin
		select *
		from EVENT_IMAGES
		where EventImageID = @event_image_id
	end
END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SELECT_EVENTS]
@id bigint = null
,@uid bigint = null
,@location_title nvarchar(1000) = null
AS
BEGIN
	SET NOCOUNT ON;

	if @id is not null --a specific event
		select e.*, u.[UID], u.UserID, u.FirstName, u.LastName, u.Email
		from [EVENTS] e, USERS u
		where EventID=@id
		and e.[OwnerUID] = u.[UID]
	else if @uid is not null
		select e.*, u.[UID], u.UserID, u.FirstName, u.LastName, u.Email
		from [EVENTS] e, USERS u
		where [OwnerUID] = @uid
		and e.[OwnerUID] = u.[UID]
		and e.StartDate >= getdate()
		order by e.StartDate
	else if @location_title is not null --get all events for a specific location
		select e.*, u.[UID], u.UserID, u.FirstName, u.LastName, u.Email
		from [EVENTS] e, USERS u
		where LocationTitle = @location_title
		and e.[OwnerUID] = u.[UID]
		and e.StartDate >= getdate()
		order by e.StartDate
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SELECT_EVENTS_ACTIVE]
AS
BEGIN
	SET NOCOUNT ON;

	create table #t (EventID bigint, Rating int)

	insert into #t (EventID, Rating)
	select e.EventID, 0
	from [EVENTS] e
	where e.IsActive='Y' --only active events
	and getdate() < e.StartDate --only events are are in the future

	declare @avg int, @event_id bigint

	declare c cursor for
	select EventID from #t
	open c

	fetch next from c into @event_id

	while @@fetch_status = 0
		begin
			select @avg = avg(Rating) from EVENT_RATINGS where EventID = @event_id
			if @avg is not null and @avg > 0
				update #t set Rating = @avg where EventID = @event_id
			fetch next from c into @event_id
		end
	close c
	deallocate c

	select e.*, u.FirstName, u.LastName, #t.Rating as AvgRating
	from [EVENTS] e, USERS u, #t
	where e.EventID = #t.EventID
	and e.OwnerUID = u.[UID]
	and e.IsActive='Y' --only active events
	and getdate() < e.StartDate --only events are are in the future
	order by StartDate

	drop table #t

END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UPDATE_EVENT] 
@id bigint
,@owner_uid bigint
,@title nvarchar(1000)
,@desc nvarchar(max)
,@start datetime
,@end datetime
,@location_title nvarchar(1000)
,@location_desc nvarchar(max)
,@address1 nvarchar(500)
,@address2 nvarchar(500)
,@city nvarchar(100)
,@state nvarchar(100)
,@zip nvarchar(20)
,@is_active char(1)
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE [dbo].[EVENTS]
	   SET [OwnerUID] = @owner_uid
		  ,[Title] = @title
		  ,[Desc] = @desc
		  ,[StartDate] = @start
		  ,[EndDate] = @end
		  ,[LocationTitle] = @location_title
		  ,[LocationDesc] = @location_desc
		  ,[Address1] = @address1
		  ,[Address2] = @address2
		  ,[City] = @city
		  ,[State] = @state
		  ,[Zip] = @zip
		  ,[IsActive] = @is_active
	WHERE EventID = @id

	 return 1
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UPDATE_EVENT_IMAGE] 
@event_image_id bigint = null output
,@primary_image nchar(1)
,@image varbinary(max)
,@file_name nvarchar(1000)
,@image_size bigint
AS
BEGIN
	SET NOCOUNT ON;

	update [dbo].[EVENT_IMAGES]
		set [FileName] = @file_name
			,[PrimaryImage] = @primary_image
			,[Image] = @image
			,[ImageSize] = @image_size
	where EventImageID = @event_image_id
	return 1

END

GO
