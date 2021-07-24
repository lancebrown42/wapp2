
		public bool DeleteEvent(long ID) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("DELETE_EVENT", cn);
				int intReturnValue = -1;

				SetParameter(ref cm, "@id", ID, SqlDbType.BigInt);
				SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

				cm.ExecuteReader();

				intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
				CloseDBConnection(ref cn);

				if (intReturnValue == 1) return true;
				return false;

			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public bool DeleteEventImage(long ID) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("DELETE_EVENT_IMAGE", cn);
				int intReturnValue = -1;

				SetParameter(ref cm, "@id", ID, SqlDbType.BigInt);
				SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

				cm.ExecuteReader();

				intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
				CloseDBConnection(ref cn);

				if (intReturnValue == 1) return true;
				return false;

			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public Event.ActionTypes InsertEvent(Event e) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("INSERT_EVENTS", cn);
				int intReturnValue = -1;

				SetParameter(ref cm, "@id", e.ID, SqlDbType.BigInt, Direction: ParameterDirection.Output);
				SetParameter(ref cm, "@owner_uid", e.User.UID, SqlDbType.BigInt);
				SetParameter(ref cm, "@title", e.Title, SqlDbType.NVarChar);
				SetParameter(ref cm, "@desc", e.Description, SqlDbType.NVarChar);
				SetParameter(ref cm, "@start_date", e.Start, SqlDbType.DateTime);
				SetParameter(ref cm, "@end_date", e.End, SqlDbType.DateTime);
				SetParameter(ref cm, "@location_title", e.Location.Title, SqlDbType.NVarChar);
				SetParameter(ref cm, "@location_desc", e.Location.Description, SqlDbType.NVarChar);
				SetParameter(ref cm, "@address1", e.Location.Address.Address1, SqlDbType.NVarChar);
				SetParameter(ref cm, "@address2", e.Location.Address.Address2, SqlDbType.NVarChar);
				SetParameter(ref cm, "@city", e.Location.Address.City, SqlDbType.NVarChar);
				SetParameter(ref cm, "@state", e.Location.Address.State, SqlDbType.NVarChar);
				SetParameter(ref cm, "@zip", e.Location.Address.Zip, SqlDbType.NVarChar);

				if (e.IsActive)
					SetParameter(ref cm, "@is_active", "Y", SqlDbType.Char);
				else
					SetParameter(ref cm, "@is_active", "N", SqlDbType.Char);

				SetParameter(ref cm, "ReturnValue", 0, SqlDbType.TinyInt, Direction: ParameterDirection.ReturnValue);

				cm.ExecuteReader();

				intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
				CloseDBConnection(ref cn);

				switch (intReturnValue) {
					case 1: // new event created
						e.ID = (long)cm.Parameters["@id"].Value;
						return Event.ActionTypes.InsertSuccessful;
					default:
						return Event.ActionTypes.Unknown;
				}
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public List<Image> GetEventImages(long EventID = 0, long EventImageID = 0, bool PrimaryOnly = false) {
			try {
				DataSet ds = new DataSet();
				SqlConnection cn = new SqlConnection();
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlDataAdapter da = new SqlDataAdapter("SELECT_EVENT_IMAGES", cn);
				List<Image> imgs = new List<Image>();

				da.SelectCommand.CommandType = CommandType.StoredProcedure;

				if (EventID > 0) SetParameter(ref da, "@event_id", EventID, SqlDbType.BigInt);
				if (EventImageID > 0) SetParameter(ref da, "@event_image_id", EventImageID, SqlDbType.BigInt);
				if (PrimaryOnly) SetParameter(ref da, "@primary_only", "Y", SqlDbType.Char);

				try {
					da.Fill(ds);
				}
				catch (Exception ex2) {
					//SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex2.Message);
				}
				finally { CloseDBConnection(ref cn); }

				if (ds.Tables[0].Rows.Count != 0) {
					foreach (DataRow dr in ds.Tables[0].Rows) {
						Image i = new Image();
						i.ImageID = (long)dr["EventImageID"];
						i.ImageData = (byte[])dr["Image"];
						i.FileName = (string)dr["FileName"];
						i.Size = (long)dr["ImageSize"];
						if (dr["PrimaryImage"].ToString() == "Y")
							i.Primary = true;
						else
							i.Primary = false;
						imgs.Add(i);
					}
				}
				return imgs;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public List<Event> GetEvents(long ID = 0, long UID = 0, string LocationTitle = "") {
			try {
				DataSet ds = new DataSet();
				SqlConnection cn = new SqlConnection();
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlDataAdapter da = new SqlDataAdapter("SELECT_EVENTS", cn);
				List<Event> events = new List<Event>();

				da.SelectCommand.CommandType = CommandType.StoredProcedure;

				if (ID > 0) SetParameter(ref da, "@id", ID, SqlDbType.BigInt);
				if (UID > 0) SetParameter(ref da, "@uid", UID, SqlDbType.BigInt);
				if (LocationTitle != "") SetParameter(ref da, "@location_title", LocationTitle, SqlDbType.NVarChar);

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
						if(dr["StartDate"] != null) e.Start = (DateTime)dr["StartDate"];
						if (dr["EndDate"] != null) e.End = (DateTime)dr["EndDate"];

						if (dr["IsActive"].ToString() == "N") e.IsActive = false;

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
						e.User.UID = (long)dr["UID"];
						e.User.UserID = (string)dr["UserID"];
						e.User.FirstName = (string)dr["FirstName"];
						e.User.LastName = (string)dr["LastName"];
						e.User.Email = (string)dr["Email"];

						List<Image> images = GetEventImages(e.ID, 0, true);
						if (images.Count > 0) e.EventImage = images[0];

						events.Add(e);
					}
				}
				return events;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public long InsertEventImage(Event e) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("INSERT_EVENT_IMAGE", cn);

				SetParameter(ref cm, "@event_image_id", null, SqlDbType.BigInt, Direction: ParameterDirection.Output);
				SetParameter(ref cm, "@event_id", e.ID, SqlDbType.BigInt);
				if (e.EventImage.Primary)
					SetParameter(ref cm, "@primary_image", "Y", SqlDbType.Char);
				else
					SetParameter(ref cm, "@primary_image", "N", SqlDbType.Char);

				SetParameter(ref cm, "@image", e.EventImage.ImageData, SqlDbType.VarBinary);
				SetParameter(ref cm, "@file_name", e.EventImage.FileName, SqlDbType.NVarChar);
				SetParameter(ref cm, "@image_size", e.EventImage.Size, SqlDbType.BigInt);

				cm.ExecuteReader();
				CloseDBConnection(ref cn);
				return (long)cm.Parameters["@event_image_id"].Value;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public Event.ActionTypes UpdateEvent(Event e) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("UPDATE_EVENT", cn);
				int intReturnValue = -1;

				SetParameter(ref cm, "@id", e.ID, SqlDbType.BigInt);
				SetParameter(ref cm, "@owner_uid", e.User.UID, SqlDbType.BigInt);
				SetParameter(ref cm, "@title", e.Title, SqlDbType.NVarChar);
				SetParameter(ref cm, "@desc", e.Description, SqlDbType.NVarChar);
				SetParameter(ref cm, "@start", e.Start, SqlDbType.DateTime);
				SetParameter(ref cm, "@end", e.End, SqlDbType.DateTime);
				SetParameter(ref cm, "@location_title", e.Location.Title, SqlDbType.NVarChar);
				SetParameter(ref cm, "@location_desc", e.Location.Description, SqlDbType.NVarChar);
				SetParameter(ref cm, "@address1", e.Location.Address.Address1, SqlDbType.NVarChar);
				SetParameter(ref cm, "@address2", e.Location.Address.Address2, SqlDbType.NVarChar);
				SetParameter(ref cm, "@city", e.Location.Address.City, SqlDbType.NVarChar);
				SetParameter(ref cm, "@state", e.Location.Address.State, SqlDbType.NVarChar);
				SetParameter(ref cm, "@zip", e.Location.Address.Zip, SqlDbType.NVarChar);

				if (e.IsActive)
					SetParameter(ref cm, "@is_active", "Y", SqlDbType.Char);
				else
					SetParameter(ref cm, "@is_active", "N", SqlDbType.Char);

				SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

				cm.ExecuteReader();

				intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
				CloseDBConnection(ref cn);

				switch (intReturnValue) {
					case 1: //new updated
						return Event.ActionTypes.UpdateSuccessful;
					default:
						return Event.ActionTypes.Unknown;
				}
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public long UpdateEventImage(Event e) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("UPDATE_EVENT_IMAGE", cn);

				SetParameter(ref cm, "@event_image_id", e.EventImage.ImageID, SqlDbType.BigInt);
				if (e.EventImage.Primary)
					SetParameter(ref cm, "@primary_image", "Y", SqlDbType.Char);
				else
					SetParameter(ref cm, "@primary_image", "N", SqlDbType.Char);

				SetParameter(ref cm, "@image", e.EventImage.ImageData, SqlDbType.VarBinary);
				SetParameter(ref cm, "@file_name", e.EventImage.FileName, SqlDbType.NVarChar);
				SetParameter(ref cm, "@image_size", e.EventImage.Size, SqlDbType.BigInt);

				cm.ExecuteReader();
				CloseDBConnection(ref cn);

				return 0; //success	
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

