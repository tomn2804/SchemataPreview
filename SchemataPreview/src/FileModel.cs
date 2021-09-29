﻿using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace SchemataPreview
{
	public class FileModel : Model
	{
		public FileModel(ImmutableSchema schema)
			: base(schema)
		{
		}

		public override bool Exists => File.Exists(FullName);

		public override void Create()
		{
			File.Create(FullName);
		}

		public override void Delete()
		{
			FileSystem.DeleteFile(FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
		}
	}
}
