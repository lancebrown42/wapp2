
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[INSERT_USER_IMAGE] 
@user_image_id bigint = null output
,@uid bigint
,@primary_image nchar(1)
,@image varbinary(max)
,@file_name nvarchar(1000)
,@image_size bigint
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[USER_IMAGES]
				([UID]
				,[PrimaryImage]
				,[Image]
				,[FileName]
				,[ImageSize])
			VALUES
				(@uid
				,@primary_image
				,@image
				,@file_name
				,@image_size)

	select @user_image_id=@@identity
	return 1


END
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UPDATE_USER_IMAGE] 
@user_image_id bigint = null output
,@primary_image nchar(1)
,@image varbinary(max)
,@file_name nvarchar(1000)
,@image_size bigint
AS
BEGIN
	SET NOCOUNT ON;

	update [dbo].[USER_IMAGES]
		set [FileName] = @file_name
			,[PrimaryImage] = @primary_image
			,[Image] = @image
			,[ImageSize] = @image_size
	where UserImageID = @user_image_id
	return 1

END
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SELECT_USER_IMAGES]
@uid bigint = null
,@user_image_id bigint = null
,@primary_only char(1) = 'N'
AS
BEGIN
	SET NOCOUNT ON;

	if @uid is not null
	begin
		if @primary_only = 'Y'
		begin
			select *
			from USER_IMAGES
			where [UID] = @uid
			and PrimaryImage = 'Y'
			order by DateAdded desc
		end else
		begin
			select *
			from USER_IMAGES
			where [UID] = @uid
			and PrimaryImage = 'N'
			order by DateAdded desc
		end
	end
	else
	begin
		select *
		from USER_IMAGES
		where UserImageID = @user_image_id
	end
END
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DELETE_USER_IMAGE]
@id bigint
AS
BEGIN
	SET NOCOUNT ON;

	delete from USER_IMAGES where UserImageID=@id

	return @@rowcount
END
GO
