
		public List<Like> Likes;
		public List<Rating> Ratings;

		public bool DoesUserLike(Like.Types LikeType, long EventID) {
			try {
				foreach (Like l in this.Likes) {
					if (l.Type == LikeType && l.ID == EventID) return true;
				}
				return false;
			}
			catch (Exception) { return false; }
		}

		public byte GetUserRating(Rating.Types RatingType, long EventID) {
			try {
				foreach (Rating r in this.Ratings) {
					if (r.Type == RatingType && r.ID == EventID) return r.Rate;
				}
				return 0;
			}
			catch (Exception) { return 0; }
		}

