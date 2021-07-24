public int RateEvent(long UID, long ID, long Rating) {
	try {
		SqlConnection cn = null;
		if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
		SqlCommand cm = new SqlCommand("UPDATE_EVENT_RATING", cn);
		int intReturnValue = -1;

		SetParameter(ref cm, "@rating_id", null, SqlDbType.BigInt, Direction: ParameterDirection.Output);
		SetParameter(ref cm, "@uid", UID, SqlDbType.BigInt);
		SetParameter(ref cm, "@event_id", ID, SqlDbType.BigInt);
		SetParameter(ref cm, "@rating", Rating, SqlDbType.TinyInt);

		SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

		cm.ExecuteReader();

		//1 = new rate added
		//2 = existing rate updated
		intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
		CloseDBConnection(ref cn);
		return intReturnValue;
	}
	catch (Exception ex) { throw new Exception(ex.Message); }
}

public List<Rating> GetEventRatings(long UID) {
	try {
		DataSet ds = new DataSet();
		SqlConnection cn = new SqlConnection();
		if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
		SqlDataAdapter da = new SqlDataAdapter("SELECT_USER_EVENT_RATINGS", cn);
		List<Rating> ratings = new List<Rating>();

		da.SelectCommand.CommandType = CommandType.StoredProcedure;

		SetParameter(ref da, "@uid", UID, SqlDbType.BigInt);

		try {
			da.Fill(ds);
		}
		catch (Exception ex2) {
			//SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex2.Message);
		}
		finally { CloseDBConnection(ref cn); }

		if (ds.Tables[0].Rows.Count != 0) {
			foreach (DataRow dr in ds.Tables[0].Rows) {
				Rating r = new Rating();
				r.Type = Rating.Types.Event;
				r.ID = (long)dr["EventID"];
				r.Rate = (byte)dr["Rating"];
				ratings.Add(r);
			}
		}
		return ratings;
	}
	catch (Exception ex) { throw new Exception(ex.Message); }
}

public List<Event> GetActiveEvents() {
	try {
		DataSet ds = new DataSet();
		SqlConnection cn = new SqlConnection();
		if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
		SqlDataAdapter da = new SqlDataAdapter("SELECT_EVENTS_ACTIVE", cn);
		List<Event> events = new List<Event>();

		da.SelectCommand.CommandType = CommandType.StoredProcedure;

		try {
			da.Fill(ds);
		}
		catch (Exception ex2) {
			//SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex2.Message);
		}
		finally { CloseDBConnection(ref cn); }

		if (ds.Tables[0].Rows.Count != 0) {
			foreach (DataRow dr in ds.Tables[0].Rows) {
				Event e = new Event();
				e.ID = (long)dr["EventID"];
				e.Title = (string)dr["Title"];
				e.Description = (string)dr["Desc"];
				if (dr["StartDate"] != null) e.Start = (DateTime)dr["StartDate"];
				if (dr["EndDate"] != null) e.End = (DateTime)dr["EndDate"];
				e.TotalLikes = (int)dr["TotalLikes"];
				if (dr["IsActive"].ToString() == "N") e.IsActive = false;
				e.AverageRating = (int)dr["AvgRating"];
				e.Location = new Location();
				e.Location.Title = (string)dr["LocationTitle"];
				e.Location.Description = (string)dr["LocationDesc"];

				e.Location.Address = new Address();
				e.Location.Address.Address1 = (string)dr["Address1"];
				e.Location.Address.Address2 = (string)dr["Address2"];
				e.Location.Address.City = (string)dr["City"];
				e.Location.Address.State = (string)dr["State"];
				e.Location.Address.Zip = (string)dr["Zip"];

				e.User = new User();
				e.User.UID = (long)dr["OwnerUID"];
				e.User.FirstName = (string)dr["FirstName"];
				e.User.LastName = (string)dr["LastName"];

				List<Image> images = GetEventImages(e.ID, 0, true);
				if (images.Count > 0) e.EventImage = images[0];

				events.Add(e);
			}
		}
		return events;
	}
	catch (Exception ex) { throw new Exception(ex.Message); }
}

public int ToggleEventLike(long UID, long ID) {
	try {
		SqlConnection cn = null;
		if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
		SqlCommand cm = new SqlCommand("TOGGLE_EVENT_LIKE", cn);
		int intReturnValue = -1;

		SetParameter(ref cm, "@uid", UID, SqlDbType.BigInt);
		SetParameter(ref cm, "@event_id", ID, SqlDbType.BigInt);

		SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

		cm.ExecuteReader();

		//1 = added
		//0 = removed
		intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
		CloseDBConnection(ref cn);
		return intReturnValue;
	}
	catch (Exception ex) { throw new Exception(ex.Message); }
}

public List<Like> GetEventLikes(long UID) {
	try {
		DataSet ds = new DataSet();
		SqlConnection cn = new SqlConnection();
		if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
		SqlDataAdapter da = new SqlDataAdapter("SELECT_USER_EVENT_LIKES", cn);
		List<Like> likes = new List<Like>();

		da.SelectCommand.CommandType = CommandType.StoredProcedure;

		SetParameter(ref da, "@uid", UID, SqlDbType.BigInt);

		try {
			da.Fill(ds);
		}
		catch (Exception ex2) {
			//SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex2.Message);
		}
		finally { CloseDBConnection(ref cn); }

		if (ds.Tables[0].Rows.Count != 0) {
			foreach (DataRow dr in ds.Tables[0].Rows) {
				Like l = new Like();
				l.Type = Like.Types.Event;
				l.ID = (long)dr["EventID"];
				likes.Add(l);
			}
		}
		return likes;
	}
	catch (Exception ex) { throw new Exception(ex.Message); }
}

