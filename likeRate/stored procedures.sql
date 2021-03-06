
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SELECT_USER_EVENT_LIKES]
@uid bigint = null
AS
BEGIN
	SET NOCOUNT ON;

	select *
	from EVENT_LIKES
	where [UID] = @uid
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SELECT_USER_EVENT_RATINGS]
@uid bigint = null
AS
BEGIN
	SET NOCOUNT ON;

	select *
	from EVENT_RATINGS
	where [UID] = @uid
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[TOGGLE_EVENT_LIKE]
@uid bigint
,@event_id bigint
AS
BEGIN
	SET NOCOUNT ON;

	declare @count int

	--check to see if the like is already there
	select @count=count(*)
	from EVENT_LIKES 
	where [UID] = @uid 
	and EventID = @event_id 

	if @count=0 --the user has not already liked this event; like it now
	begin
		insert into EVENT_LIKES ([UID], EventID)
		values(@uid, @event_id)
		return 1 --inserted
	end
	else --the user already liked this event; remove the like
	begin
		delete from EVENT_LIKES 
		where [UID] = @uid 
		and EventID = @event_id 
		return 0 --removed
	end
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UPDATE_EVENT_RATING]
@rating_id bigint output
,@uid bigint
,@event_id bigint
,@rating tinyint
AS
BEGIN
	SET NOCOUNT ON;

	declare @count int

	--check to see if the like is already there
	select @count=count(*)
	from EVENT_RATINGS 
	where [UID] = @uid 
	and EventID = @event_id 

	if @count=0 --the user has not rated this event; rate it now
	begin
		insert into EVENT_RATINGS ([UID], EventID, Rating)
		values(@uid, @event_id, @rating)
		set @rating_id = @@IDENTITY
		return 1 --new rating added
	end
	else --the user previously rated this event; update it now
	begin
		update [dbo].[EVENT_RATINGS]
		set [Rating] = @rating
		where [UID] = @uid
		and EventID = @event_id

		select @rating_id = RateID from EVENT_RATINGS where [UID]=@uid and EventID=@event_id
		return 2 --existing rating updated
	end
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SELECT_EVENTS]
@id bigint = null
,@uid bigint = null
,@location_title nvarchar(1000) = null
AS
BEGIN
	SET NOCOUNT ON;

	if @id is not null --a specific event
		select e.*, u.[UID], u.UserID, u.FirstName, u.LastName, u.Email, TotalLikes=(select count(*) from EVENT_LIKES where EventID=@id)
		from [EVENTS] e, USERS u
		where EventID=@id
		and e.[OwnerUID] = u.[UID]
	else if @uid is not null
		select e.*, u.[UID], u.UserID, u.FirstName, u.LastName, u.Email, TotalLikes=0
		from [EVENTS] e, USERS u
		where [OwnerUID] = @uid
		and e.[OwnerUID] = u.[UID]
		and e.StartDate >= getdate()
		order by e.StartDate
	else if @location_title is not null --get all events for a specific location
		select e.*, u.[UID], u.UserID, u.FirstName, u.LastName, u.Email, TotalLikes=0
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
ALTER PROCEDURE [dbo].[SELECT_EVENTS_ACTIVE]
AS
BEGIN
	SET NOCOUNT ON;

	create table #t (EventID bigint, Rating int, Likes int)

	insert into #t (EventID, Rating)
	select e.EventID, 0
	from [EVENTS] e
	where e.IsActive='Y' --only active events
	and getdate() < e.StartDate --only events are are in the future

	declare @avg int, @event_id bigint, @count bigint

	declare c cursor for
	select EventID from #t
	open c

	fetch next from c into @event_id

	while @@fetch_status = 0
		begin
			select @avg = avg(Rating) from EVENT_RATINGS where EventID = @event_id
			if @avg is not null and @avg > 0
				update #t set Rating = @avg where EventID = @event_id
			select @count=count(*) from EVENT_LIKES where EventID=@event_id
			update #t set Likes = @count where EventID = @event_id
			fetch next from c into @event_id
		end
	close c
	deallocate c

	select e.*, u.FirstName, u.LastName, #t.Rating as AvgRating, #t.Likes as TotalLikes
	from [EVENTS] e, USERS u, #t
	where e.EventID = #t.EventID
	and e.OwnerUID = u.[UID]
	and e.IsActive='Y' --only active events
	and getdate() < e.StartDate --only events are are in the future
	order by StartDate

	drop table #t

END

GO