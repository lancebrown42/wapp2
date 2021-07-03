
		public sbyte AddGalleryImage(HttpPostedFileBase f) {
			try {
				this.UserImage = new Image();
				this.UserImage.Primary = false;
				this.UserImage.FileName = Path.GetFileName(f.FileName);

				if (this.UserImage.IsImageFile()) {
					this.UserImage.Size = f.ContentLength;
					Stream stream = f.InputStream;
					BinaryReader binaryReader = new BinaryReader(stream);
					this.UserImage.ImageData = binaryReader.ReadBytes((int)stream.Length);
					this.UpdatePrimaryImage();
				}
				return 0;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public sbyte UpdatePrimaryImage() {
			try {
				Models.Database db = new Database();
				long NewUID;
				if (this.UserImage.ImageID == 0) {
					NewUID = db.InsertUserImage(this);
					if (NewUID > 0) UserImage.ImageID = NewUID;
				}
				else {
					db.UpdateUserImage(this);
				}
				return 0;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

