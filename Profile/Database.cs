		public long InsertUserImage(User u) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("INSERT_USER_IMAGE", cn);

				SetParameter(ref cm, "@user_image_id", null, SqlDbType.BigInt, Direction: ParameterDirection.Output);
				SetParameter(ref cm, "@uid", u.UID, SqlDbType.BigInt);
				if (u.UserImage.Primary)
					SetParameter(ref cm, "@primary_image", "Y", SqlDbType.Char);
				else
					SetParameter(ref cm, "@primary_image", "N", SqlDbType.Char);

				SetParameter(ref cm, "@image", u.UserImage.ImageData, SqlDbType.VarBinary);
				SetParameter(ref cm, "@file_name", u.UserImage.FileName, SqlDbType.NVarChar);
				SetParameter(ref cm, "@image_size", u.UserImage.Size, SqlDbType.BigInt);

				cm.ExecuteReader();
				CloseDBConnection(ref cn);
				return (long)cm.Parameters["@user_image_id"].Value;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public long UpdateUserImage(User u) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("UPDATE_USER_IMAGE", cn);

				SetParameter(ref cm, "@user_image_id", u.UserImage.ImageID, SqlDbType.BigInt);
				if (u.UserImage.Primary)
					SetParameter(ref cm, "@primary_image", "Y", SqlDbType.Char);
				else
					SetParameter(ref cm, "@primary_image", "N", SqlDbType.Char);

				SetParameter(ref cm, "@image", u.UserImage.ImageData, SqlDbType.VarBinary);
				SetParameter(ref cm, "@file_name", u.UserImage.FileName, SqlDbType.NVarChar);
				SetParameter(ref cm, "@image_size", u.UserImage.Size, SqlDbType.BigInt);

				cm.ExecuteReader();
				CloseDBConnection(ref cn);

				return 0; //success	
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public List<Image> GetUserImages(long UID = 0, long UserImageID = 0, bool PrimaryOnly = false) {
			try {
				DataSet ds = new DataSet();
				SqlConnection cn = new SqlConnection();
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlDataAdapter da = new SqlDataAdapter("SELECT_USER_IMAGES", cn);
				List<Image> imgs = new List<Image>();

				da.SelectCommand.CommandType = CommandType.StoredProcedure;

				if (UID > 0) SetParameter(ref da, "@uid", UID, SqlDbType.BigInt);
				if (UserImageID > 0) SetParameter(ref da, "@user_image_id", UserImageID, SqlDbType.BigInt);
				if (PrimaryOnly) SetParameter(ref da, "@primary_only", "Y", SqlDbType.Char);

				try {
					da.Fill(ds);
				}
				catch (Exception ex2) {
					throw new Exception(ex2.Message);
				}
				finally { CloseDBConnection(ref cn); }

				if (ds.Tables[0].Rows.Count != 0) {
					foreach (DataRow dr in ds.Tables[0].Rows) {
						Image i = new Image();
						i.ImageID = (long)dr["UserImageID"];
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

		public bool DeleteUserImage(long ID) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("DELETE_USER_IMAGE", cn);
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

