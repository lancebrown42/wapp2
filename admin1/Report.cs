

		public string ProblemText {
			get {
				switch (ProblemType) {
					case ProblemTypes.MissleadingorScam:
						return "Missleading or Scam";
					case ProblemTypes.SexuallyInappropriate:
						return "Sexually Inappropriate";
					case ProblemTypes.Offensive:
						return "Offensive";
					case ProblemTypes.Violent:
						return "Violent";
					case ProblemTypes.Spam:
						return "Spam";
					default: //NoType or anything else
						return "Not set";
				}
			}
		}

		public static string GetProblemText(ProblemTypes pt) {
			switch (pt) {
				case ProblemTypes.MissleadingorScam:
					return "Missleading or Scam";
				case ProblemTypes.SexuallyInappropriate:
					return "Sexually Inappropriate";
				case ProblemTypes.Offensive:
					return "Offensive";
				case ProblemTypes.Violent:
					return "Violent";
				case ProblemTypes.Spam:
					return "Spam";
				default: //NoType or anything else
					return "Not set";
			}
		}

