
		public bool InsertReport(long UID, long IDToReport, int ProblemID) {
			try {
				SqlConnection cn = null;
				if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
				SqlCommand cm = new SqlCommand("INSERT_REPORTS", cn);

				SetParameter(ref cm, "@uid", UID, SqlDbType.BigInt);
				SetParameter(ref cm, "@id_to_report", IDToReport, SqlDbType.BigInt);
				SetParameter(ref cm, "@problem_id", ProblemID, SqlDbType.TinyInt);

				SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

				cm.ExecuteReader();

				return true;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

